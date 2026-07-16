import { useState } from "react";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { Textarea } from "@/shared/ui/textarea";
import { EntityDialog } from "@/shared/components/crud";
import { createMaterial, uploadMaterialFile } from "../api";

interface UploadMaterialDialogProps {
  isOpen: boolean;
  onClose: () => void;
  courseId: string;
  folderId?: number;
  onCreated: () => void;
}

export function UploadMaterialDialog({
  isOpen,
  onClose,
  courseId,
  folderId,
  onCreated,
}: UploadMaterialDialogProps) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const reset = () => {
    setTitle("");
    setDescription("");
    setFile(null);
    setError(null);
  };

  const handleSubmit = async () => {
    if (!title.trim() || !file) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const contentUrl = await uploadMaterialFile(file);
      await createMaterial(courseId, {
        title: title.trim(),
        contentUrl,
        folderId,
        description: description.trim() || undefined,
      });
      reset();
      onClose();
      onCreated();
    } catch (err) {
      console.error("Failed to upload material:", err);
      setError("Failed to upload material. Please try again.");
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
      title="Upload Material"
      description="Upload a file to share with students."
      onSubmit={handleSubmit}
      onCancel={handleCancel}
      submitLabel="Upload"
      isLoading={isSubmitting}
      disabled={!title.trim() || !file}
    >
      <div className="grid gap-4">
        <div className="grid gap-2">
          <Label htmlFor="material-title">Title</Label>
          <Input
            id="material-title"
            placeholder="Enter material title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
        </div>
        <div className="grid gap-2">
          <Label htmlFor="material-file">File</Label>
          <Input
            id="material-file"
            type="file"
            accept=".pdf,.jpg,.jpeg,.png,.docx,.pptx,.mp4"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          />
        </div>
        <div className="grid gap-2">
          <Label htmlFor="material-description">Description (optional)</Label>
          <Textarea
            id="material-description"
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
