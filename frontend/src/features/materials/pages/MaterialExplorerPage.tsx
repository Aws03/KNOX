import { useParams } from "react-router-dom";
import { Card } from "@/shared/ui/card";
import { Loader2 } from "lucide-react";
import { useUserRole } from "@/hooks/useUserRole";
import { useCourseMaterials } from "../hooks/useCourseMaterials";
import { MaterialExplorerHeader } from "../components/MaterialExplorerHeader";
import { MaterialExplorerContent } from "../components/MaterialExplorerContent";
import type { FolderItem, MaterialItem } from "../types";

interface MaterialExplorerProps {
  mode?: "public" | "manage";
}

export default function MaterialExplorerPage({
  mode = "public",
}: MaterialExplorerProps) {
  const { courseId } = useParams<{ courseId: string }>();
  const { canManageContent } = useUserRole();

  // Check if user can see management UI
  const isManagementMode = mode === "manage" && canManageContent();

  const { loading, contents, error } = useCourseMaterials({
    courseId,
    isManagementMode,
  });

  // Management action handlers
  const handleAddFolder = () => {
    // TODO: Open add folder dialog
  };

  const handleAddMaterial = () => {
    // TODO: Open add material dialog
  };

  const handleEditFolder = (_folder: FolderItem) => {
    // TODO: Open edit folder dialog
  };

  const handleDeleteFolder = (_folderId: number) => {
    // TODO: Confirm and delete folder
  };

  const handleEditMaterial = (_material: MaterialItem) => {
    // TODO: Open edit material dialog
  };

  const handleDeleteMaterial = (_materialId: number) => {
    // TODO: Confirm and delete material
  };

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center gap-3 py-16">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
        <p className="text-sm text-muted-foreground">
          Loading course materials…
        </p>
      </div>
    );
  }

  if (error) {
    return (
      <Card className="p-6 text-center">
        <p className="text-red-500">{error}</p>
      </Card>
    );
  }

  if (!contents || !courseId) return null;

  const totalItems = contents.folders.length + contents.materials.length;

  return (
    <Card className="relative overflow-hidden border-border/50 bg-card/90 p-6">
      <div className="absolute inset-0 -z-10 bg-linear-to-br from-primary/5 via-transparent to-purple-500/10" />
      <div className="absolute right-8 top-6 hidden h-32 w-32 rounded-full bg-primary/20 blur-3xl sm:block" />

      <div className="flex flex-col gap-6">
        <MaterialExplorerHeader
          courseId={courseId}
          totalItems={totalItems}
          isManagementMode={isManagementMode}
          onAddFolder={handleAddFolder}
          onAddMaterial={handleAddMaterial}
        />

        <MaterialExplorerContent
          folders={contents.folders}
          materials={contents.materials}
          courseId={courseId}
          isManagementMode={isManagementMode}
          onEditFolder={handleEditFolder}
          onDeleteFolder={handleDeleteFolder}
          onEditMaterial={handleEditMaterial}
          onDeleteMaterial={handleDeleteMaterial}
        />
      </div>
    </Card>
  );
}
