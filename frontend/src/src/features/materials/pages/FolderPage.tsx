import { useParams } from "react-router-dom";
import { useState } from "react";
import { useUserRole } from "@/hooks/useUserRole";
import { useFolderContents } from "../hooks/useFolderContents";
import { LoadingState, ErrorState } from "../components/PageStates";
import { FolderPageHeader } from "../components/FolderPageHeader";
import { FoldersList } from "../components/FoldersList";
import { MaterialsList } from "../components/MaterialsList";
import { EmptyState } from "../components/EmptyState";
import { CreateFolderDialog } from "../components/CreateFolderDialog";
import { UploadMaterialDialog } from "../components/UploadMaterialDialog";

interface FolderPageProps {
  mode?: "public" | "manage";
}

export default function FolderPage({ mode = "public" }: FolderPageProps) {
  const { courseId, folderId } = useParams<{
    courseId: string;
    folderId: string;
  }>();
  const { canManageContent } = useUserRole();

  // Check if user can see management UI
  const isManagementMode = mode === "manage" && canManageContent();

  const {
    contents,
    loading,
    error,
    refetch,
    handleDeleteFolder,
    handleDeleteMaterial,
    handleEditFolder,
    handleEditMaterial,
  } = useFolderContents({ courseId, folderId, isManagementMode });

  const [isFolderDialogOpen, setIsFolderDialogOpen] = useState(false);
  const [isMaterialDialogOpen, setIsMaterialDialogOpen] = useState(false);

  const handleAddFolder = () => {
    setIsFolderDialogOpen(true);
  };

  const handleAddMaterial = () => {
    setIsMaterialDialogOpen(true);
  };

  if (loading) return <LoadingState />;
  if (error) return <ErrorState error={error} />;
  if (!contents) return null;

  const isEmpty =
    contents.folders.length === 0 && contents.materials.length === 0;

  return (
    <div className="container mx-auto px-4 py-6">
      <FolderPageHeader
        courseId={courseId!}
        folderId={folderId}
        isManagementMode={isManagementMode}
        onAddFolder={handleAddFolder}
        onAddMaterial={handleAddMaterial}
      />

      <div className="space-y-6">
        <FoldersList
          folders={contents.folders}
          courseId={courseId!}
          isManagementMode={isManagementMode}
          onEdit={handleEditFolder}
          onDelete={handleDeleteFolder}
        />

        <MaterialsList
          materials={contents.materials}
          isManagementMode={isManagementMode}
          onEdit={handleEditMaterial}
          onDelete={handleDeleteMaterial}
        />

        {isEmpty && <EmptyState />}
      </div>

      <CreateFolderDialog
        isOpen={isFolderDialogOpen}
        onClose={() => setIsFolderDialogOpen(false)}
        courseId={courseId!}
        parentFolderId={folderId ? parseInt(folderId, 10) : undefined}
        onCreated={refetch}
      />
      <UploadMaterialDialog
        isOpen={isMaterialDialogOpen}
        onClose={() => setIsMaterialDialogOpen(false)}
        courseId={courseId!}
        folderId={folderId ? parseInt(folderId, 10) : undefined}
        onCreated={refetch}
      />
    </div>
  );
}
