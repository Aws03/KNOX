import { Button } from "@/shared/ui/button";
import { Separator } from "@/shared/ui/separator";
import { Plus, Sparkles } from "lucide-react";
import ShareButton from "@/shared/ui/ShareButton";

interface MaterialExplorerHeaderProps {
  courseId: string;
  totalItems: number;
  isManagementMode: boolean;
  onAddFolder?: () => void;
  onAddMaterial?: () => void;
}

export function MaterialExplorerHeader({
  courseId,
  totalItems,
  isManagementMode,
  onAddFolder,
  onAddMaterial,
}: MaterialExplorerHeaderProps) {
  return (
    <>
      <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div className="flex-1">
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Sparkles className="h-4 w-4 text-primary" />
            Course #{courseId} · Materials Vault
          </div>
          <div className="flex items-center gap-3">
            <h2 className="text-3xl font-semibold tracking-tight">
              Course Materials
            </h2>
            <ShareButton
              url={`/courses/${courseId}/materials`}
              title={`Check out Course #${courseId} Materials`}
              showText={false}
            />
          </div>
          <p className="text-sm text-muted-foreground">
            Access all course materials, organized by folders and files.
          </p>
        </div>

        {isManagementMode && (
          <div className="flex gap-2">
            <Button onClick={onAddFolder} variant="outline" size="sm">
              <Plus className="mr-2 h-4 w-4" />
              Add Folder
            </Button>
            <Button onClick={onAddMaterial} size="sm">
              <Plus className="mr-2 h-4 w-4" />
              Add Material
            </Button>
          </div>
        )}

        <div className="rounded-2xl border border-border/70 bg-background/60 px-4 py-3 shadow-sm">
          <p className="text-xs uppercase tracking-wide text-muted-foreground">
            Total Items
          </p>
          <p className="text-2xl font-semibold">{totalItems}</p>
        </div>
      </div>

      <Separator className="opacity-60" />
    </>
  );
}
