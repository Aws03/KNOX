import { useState } from "react";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import type { Faculty } from "../types";

interface AddFacultyDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onAdd: (name: string) => Promise<void>;
  universityName?: string;
}

export const AddFacultyDialog = ({
  isOpen,
  onClose,
  onAdd,
  universityName,
}: AddFacultyDialogProps) => {
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
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add New Faculty</DialogTitle>
          <DialogDescription>
            Enter the name of the faculty you want to add to {universityName}.
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="faculty-name">Faculty Name</Label>
            <Input
              id="faculty-name"
              placeholder="Enter faculty name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSubmit()}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={handleCancel}>
            Cancel
          </Button>
          <Button onClick={handleSubmit}>Add Faculty</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

interface EditFacultyDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onEdit: (id: number, name: string) => Promise<void>;
  faculty: Faculty | null;
}

export const EditFacultyDialog = ({
  isOpen,
  onClose,
  onEdit,
  faculty,
}: EditFacultyDialogProps) => {
  const [name, setName] = useState(faculty?.name || "");

  const handleSubmit = async () => {
    if (!faculty) return;
    try {
      await onEdit(faculty.id, name);
      onClose();
    } catch {
      // Error is handled in the hook
    }
  };

  const handleOpenChange = (open: boolean) => {
    if (open && faculty) {
      setName(faculty.name);
    }
    if (!open) {
      onClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Faculty</DialogTitle>
          <DialogDescription>Update the name of the faculty.</DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="edit-faculty-name">Faculty Name</Label>
            <Input
              id="edit-faculty-name"
              placeholder="Enter faculty name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSubmit()}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button onClick={handleSubmit}>Save Changes</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

interface DeleteFacultyDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onDelete: (id: number) => Promise<void>;
  faculty: Faculty | null;
}

export const DeleteFacultyDialog = ({
  isOpen,
  onClose,
  onDelete,
  faculty,
}: DeleteFacultyDialogProps) => {
  const handleConfirm = async () => {
    if (!faculty) return;
    try {
      await onDelete(faculty.id);
      onClose();
    } catch {
      // Error is handled in the hook
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Confirm Deletion</DialogTitle>
          <DialogDescription>
            Are you sure you want to inactivate "{faculty?.name}"? This action
            will soft delete the faculty from the system.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button variant="destructive" onClick={handleConfirm}>
            Inactivate
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
