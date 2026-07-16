import { Card } from "@/shared/ui/card";
import { FolderCard } from "./FolderCard";
import type { FolderItem } from "../types";

interface FoldersListProps {
  folders: FolderItem[];
  courseId: string;
  isManagementMode: boolean;
  onEdit: (folder: FolderItem) => void;
  onDelete: (folderId: number) => void;
}

export function FoldersList({
  folders,
  courseId,
  isManagementMode,
  onEdit,
  onDelete,
}: FoldersListProps) {
  if (folders.length === 0) return null;

  return (
    <Card className="p-6">
      <h2 className="mb-4 text-lg font-semibold">Folders</h2>
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {folders.map((folder) => (
          <FolderCard
            key={folder.id}
            folder={folder}
            courseId={courseId}
            isManagementMode={isManagementMode}
            onEdit={onEdit}
            onDelete={onDelete}
          />
        ))}
      </div>
    </Card>
  );
}
