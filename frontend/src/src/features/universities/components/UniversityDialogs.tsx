import { useState } from "react";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { EntityDialog, DeleteConfirmDialog } from "@/shared/components/crud";
import type { University } from "../types";

interface AddUniversityDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onAdd: (name: string) => Promise<void>;
}

export const AddUniversityDialog = ({
  isOpen,
  onClose,
  onAdd,
}: AddUniversityDialogProps) => {
  const [name, setName] = useState("");

  const handleSubmit = async () => {
    try {
      await onAdd(name);
      setName("");
      onClose();
    } catch {
      // Error is handled in the hook
    }
  };

  const handleCancel = () => {
    setName("");
    onClose();
  };

  return (
    <EntityDialog
      open={isOpen}
      onOpenChange={onClose}
      title="Add New University"
      description="Enter the name of the university you want to add."
      onSubmit={handleSubmit}
      onCancel={handleCancel}
      submitLabel="Add University"
    >
      <div className="grid gap-2">
        <Label htmlFor="university-name">University Name</Label>
        <Input
          id="university-name"
          placeholder="Enter university name"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
      </div>
    </EntityDialog>
  );
};

interface EditUniversityDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onEdit: (id: number, name: string) => Promise<void>;
  university: University | null;
}

export const EditUniversityDialog = ({
  isOpen,
  onClose,
  onEdit,
  university,
}: EditUniversityDialogProps) => {
  const [name, setName] = useState(university?.name || "");

  const handleSubmit = async () => {
    if (!university) return;
    try {
      await onEdit(university.id, name);
      onClose();
    } catch {
      // Error is handled in the hook
    }
  };

  const handleOpenChange = (open: boolean) => {
    if (open && university) {
      setName(university.name);
    }
    if (!open) {
      onClose();
    }
  };

  return (
    <EntityDialog
      open={isOpen}
      onOpenChange={handleOpenChange}
      title="Edit University"
      description="Update the name of the university."
      onSubmit={handleSubmit}
      onCancel={onClose}
      submitLabel="Save Changes"
    >
      <div className="grid gap-2">
        <Label htmlFor="edit-university-name">University Name</Label>
        <Input
          id="edit-university-name"
          placeholder="Enter university name"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
      </div>
    </EntityDialog>
  );
};

interface DeleteUniversityDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onDelete: (id: number) => Promise<void>;
  university: University | null;
}

export const DeleteUniversityDialog = ({
  isOpen,
  onClose,
  onDelete,
  university,
}: DeleteUniversityDialogProps) => {
  const handleConfirm = async () => {
    if (!university) return;
    try {
      await onDelete(university.id);
      onClose();
    } catch {
      // Error is handled in the hook
    }
  };

  return (
    <DeleteConfirmDialog
      open={isOpen}
      onOpenChange={onClose}
      onConfirm={handleConfirm}
      title="Confirm Deletion"
      description={`Are you sure you want to inactivate "${university?.name}"? This action will soft delete the university from the system.`}
      entityName="university"
    />
  );
};
