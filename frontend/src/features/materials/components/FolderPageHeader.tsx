import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/button";
import { ArrowLeft, Plus } from "lucide-react";
import ShareButton from "@/shared/ui/ShareButton";

interface FolderPageHeaderProps {
  courseId: string;
  folderId: string | undefined;
  isManagementMode: boolean;
  onAddFolder: () => void;
  onAddMaterial: () => void;
}

export function FolderPageHeader({
  courseId,
  folderId,
  isManagementMode,
  onAddFolder,
  onAddMaterial,
}: FolderPageHeaderProps) {
  const navigate = useNavigate();

  return (
    <div className="mb-6 flex items-center justify-between gap-4">
      <div className="flex items-center gap-4">
        <Button
          variant="outline"
          size="sm"
          onClick={() => navigate(-1)}
          className="gap-2"
        >
          <ArrowLeft className="h-4 w-4" />
          Back
        </Button>
        <h1 className="text-2xl font-bold">
          {folderId ? `Folder Contents` : "Course Materials"}
        </h1>
        {!isManagementMode && (
          <ShareButton
            url={
              folderId
                ? `/courses/${courseId}/folder/${folderId}`
                : `/courses/${courseId}/materials`
            }
            title={
              folderId ? `Check out this folder` : `Check out Course Materials`
            }
            showText={false}
          />
        )}
      </div>
      {isManagementMode && (
        <div className="flex gap-2">
          <Button onClick={onAddFolder} size="sm" className="gap-2">
            <Plus className="h-4 w-4" />
            Add Folder
          </Button>
          <Button onClick={onAddMaterial} size="sm" className="gap-2">
            <Plus className="h-4 w-4" />
            Add Material
          </Button>
        </div>
      )}
    </div>
  );
}
