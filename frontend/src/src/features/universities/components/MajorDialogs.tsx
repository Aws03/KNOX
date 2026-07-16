import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import type { Major } from "../types";

interface AddMajorDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onAdd: (name: string) => Promise<void>;
  facultyName?: string;
}

export const AddMajorDialog = ({
  isOpen,
  onClose,
  onAdd,
  facultyName,
}: AddMajorDialogProps) => {
  const [name, setName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!name.trim()) return;
    setIsSubmitting(true);
    try {
      await onAdd(name);
      setName("");
      onClose();
    } catch {
      // Error handled in hook
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setName("");
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add New Major</DialogTitle>
          <DialogDescription>
            Enter the name of the major you want to add
            {facultyName && ` to ${facultyName}`}.
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="major-name">Major Name</Label>
            <Input
              id="major-name"
              placeholder="Enter major name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSubmit()}
              disabled={isSubmitting}
            />
          </div>
        </div>
        <DialogFooter>
          <Button
            variant="outline"
            onClick={handleClose}
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isSubmitting}>
            Add Major
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

interface EditMajorDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onEdit: (majorId: number, name: string) => Promise<void>;
  major: Major | null;
}

export const EditMajorDialog = ({
  isOpen,
  onClose,
  onEdit,
  major,
}: EditMajorDialogProps) => {
  const [name, setName] = useState(major?.name || "");
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Update name when major changes
  useEffect(() => {
    if (major?.name) {
      setName(major.name);
    }
  }, [major?.name]);

  const handleSubmit = async () => {
    if (!major || !name.trim()) return;
    setIsSubmitting(true);
    try {
      await onEdit(major.id, name);
      onClose();
    } catch {
      // Error handled in hook
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setName(major?.name || "");
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Major</DialogTitle>
          <DialogDescription>Update the name of the major.</DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="edit-major-name">Major Name</Label>
            <Input
              id="edit-major-name"
              placeholder="Enter major name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSubmit()}
              disabled={isSubmitting}
            />
          </div>
        </div>
        <DialogFooter>
          <Button
            variant="outline"
            onClick={handleClose}
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isSubmitting}>
            Save Changes
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

interface DeleteMajorDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onDelete: (majorId: number) => Promise<void>;
  major: Major | null;
}

export const DeleteMajorDialog = ({
  isOpen,
  onClose,
  onDelete,
  major,
}: DeleteMajorDialogProps) => {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!major) return;
    setIsSubmitting(true);
    try {
      await onDelete(major.id);
      onClose();
    } catch {
      // Error handled in hook
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Confirm Deletion</DialogTitle>
          <DialogDescription>
            Are you sure you want to inactivate "{major?.name}"? This action
            will soft delete the major from the system.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={onClose} disabled={isSubmitting}>
            Cancel
          </Button>
          <Button
            variant="destructive"
            onClick={handleSubmit}
            disabled={isSubmitting}
          >
            Inactivate
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
