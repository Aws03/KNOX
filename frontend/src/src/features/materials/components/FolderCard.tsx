import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/button";
import { Folder, Edit, Trash2 } from "lucide-react";
import ShareButton from "@/shared/ui/ShareButton";
import type { FolderItem } from "../types";

interface FolderCardProps {
  folder: FolderItem;
  courseId: string;
  isManagementMode: boolean;
  onEdit: (folder: FolderItem) => void;
  onDelete: (folderId: number) => void;
}

export function FolderCard({
  folder,
  courseId,
  isManagementMode,
  onEdit,
  onDelete,
}: FolderCardProps) {
  const navigate = useNavigate();

  const handleNavigate = () => {
    const basePath = isManagementMode ? "/dashboard" : "";
    navigate(`${basePath}/courses/${courseId}/folder/${folder.id}`);
  };

  return (
    <div className="group flex items-center gap-3 rounded-2xl border border-border/70 bg-background/80 p-4 transition hover:border-primary/60 hover:bg-primary/5">
      <button
        onClick={handleNavigate}
        className="flex flex-1 items-center gap-3 text-left"
      >
        <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10 text-primary transition group-hover:bg-primary/20">
          <Folder className="h-6 w-6" />
        </div>
        <div className="flex-1 overflow-hidden">
          <p className="truncate font-semibold text-foreground group-hover:text-primary">
            {folder.name}
          </p>
          {folder.description && (
            <p className="truncate text-xs text-muted-foreground">
              {folder.description}
            </p>
          )}
        </div>
      </button>

      <div className="flex items-center gap-1">
        {!isManagementMode && (
          <ShareButton
            url={`/courses/${courseId}/folder/${folder.id}`}
            title={`Check out this folder: ${folder.name}`}
            variant="ghost"
            size="icon"
          />
        )}

        {isManagementMode && (
          <>
            <Button
              variant="ghost"
              size="icon"
              onClick={(e) => {
                e.stopPropagation();
                onEdit(folder);
              }}
            >
              <Edit className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              onClick={(e) => {
                e.stopPropagation();
                onDelete(folder.id);
              }}
            >
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </>
        )}
      </div>
    </div>
  );
}
