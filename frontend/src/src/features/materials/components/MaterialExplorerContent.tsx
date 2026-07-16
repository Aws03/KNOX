import { Separator } from "@/shared/ui/separator";
import { Badge } from "@/shared/ui/badge";
import { FolderOpen, File as FileIcon } from "lucide-react";
import { FolderCard } from "./FolderCard";
import { MaterialCard } from "./MaterialCard";
import type { FolderItem, MaterialItem } from "../types";

interface MaterialExplorerContentProps {
  folders: FolderItem[];
  materials: MaterialItem[];
  courseId: string;
  isManagementMode: boolean;
  onEditFolder?: (folder: FolderItem) => void;
  onDeleteFolder?: (folderId: number) => void;
  onEditMaterial?: (material: MaterialItem) => void;
  onDeleteMaterial?: (materialId: number) => void;
}

export function MaterialExplorerContent({
  folders,
  materials,
  courseId,
  isManagementMode,
  onEditFolder = () => {},
  onDeleteFolder = () => {},
  onEditMaterial = () => {},
  onDeleteMaterial = () => {},
}: MaterialExplorerContentProps) {
  const hasFolders = folders.length > 0;
  const hasMaterials = materials.length > 0;
  const isEmpty = !hasFolders && !hasMaterials;

  if (isEmpty) {
    return (
      <div className="rounded-2xl border border-dashed border-border/70 bg-muted/30 p-12 text-center">
        <p className="text-muted-foreground">
          No materials available yet. Check back later!
        </p>
      </div>
    );
  }

  return (
    <>
      {/* Folders Section */}
      {hasFolders && (
        <div className="space-y-3">
          <div className="flex items-center gap-2">
            <FolderOpen className="h-5 w-5 text-primary" />
            <h3 className="text-lg font-semibold">Folders</h3>
            <Badge variant="secondary" className="rounded-full">
              {folders.length}
            </Badge>
          </div>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {folders.map((folder) => (
              <FolderCard
                key={folder.id}
                folder={folder}
                courseId={courseId}
                isManagementMode={isManagementMode}
                onEdit={onEditFolder}
                onDelete={onDeleteFolder}
              />
            ))}
          </div>
        </div>
      )}

      {/* Materials Section */}
      {hasMaterials && (
        <>
          {hasFolders && <Separator className="opacity-60" />}
          <div className="space-y-3">
            <div className="flex items-center gap-2">
              <FileIcon className="h-5 w-5 text-primary" />
              <h3 className="text-lg font-semibold">Files</h3>
              <Badge variant="secondary" className="rounded-full">
                {materials.length}
              </Badge>
            </div>
            <div className="grid gap-3 sm:grid-cols-2">
              {materials.map((material) => (
                <MaterialCard
                  key={material.id}
                  material={material}
                  isManagementMode={isManagementMode}
                  onEdit={onEditMaterial}
                  onDelete={onDeleteMaterial}
                />
              ))}
            </div>
          </div>
        </>
      )}
    </>
  );
}
