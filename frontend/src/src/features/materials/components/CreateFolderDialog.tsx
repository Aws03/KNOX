import { useState } from "react";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { Textarea } from "@/shared/ui/textarea";
import { EntityDialog } from "@/shared/components/crud";
import { createFolder } from "../api";

interface CreateFolderDialogProps {
  isOpen: boolean;
  onClose: () => void;
  courseId: string;
  parentFolderId?: number;
  onCreated: () => void;
}

export function CreateFolderDialog({
  isOpen,
  onClose,
  courseId,
  parentFolderId,
  onCreated,
}: CreateFolderDialogProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const reset = () => {
    setName("");
    setDescription("");
    setError(null);
  };

  const handleSubmit = async () => {
    if (!name.trim()) return;

    setIsSubmitting(true);
    setError(null);
    try {
      await createFolder(courseId, {
        name: name.trim(),
        parentFolderId,
        description: description.trim() || undefined,
      });
      reset();
      onClose();
      onCreated();
    } catch (err) {
      console.error("Failed to create folder:", err);
      setError("Failed to create folder. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    reset();
    onClose();
  };

  return (
    <EntityDialog
      open={isOpen}
      onOpenChange={onClose}
      title="New Folder"
      description="Create a folder to organize course materials."
      onSubmit={handleSubmit}
      onCancel={handleCancel}
      submitLabel="Create Folder"
      isLoading={isSubmitting}
      disabled={!name.trim()}
    >
      <div className="grid gap-4">
        <div className="grid gap-2">
          <Label htmlFor="folder-name">Folder Name</Label>
          <Input
            id="folder-name"
            placeholder="Enter folder name"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <div className="grid gap-2">
          <Label htmlFor="folder-description">Description (optional)</Label>
          <Textarea
            id="folder-description"
            placeholder="Enter description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
        {error && <p className="text-sm text-destructive">{error}</p>}
      </div>
    </EntityDialog>
  );
}
