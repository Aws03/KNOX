import { useParams } from "react-router-dom";
import { useUserRole } from "@/hooks/useUserRole";
import { useFolderContents } from "../hooks/useFolderContents";
import { LoadingState, ErrorState } from "../components/PageStates";
import { FolderPageHeader } from "../components/FolderPageHeader";
import { FoldersList } from "../components/FoldersList";
import { MaterialsList } from "../components/MaterialsList";
import { EmptyState } from "../components/EmptyState";

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
    handleDeleteFolder,
    handleDeleteMaterial,
    handleEditFolder,
    handleEditMaterial,
  } = useFolderContents({ courseId, folderId, isManagementMode });

  const handleAddFolder = () => {
    // TODO: Implement add folder dialog
  };

  const handleAddMaterial = () => {
    // TODO: Implement add material dialog
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
    </div>
  );
}
