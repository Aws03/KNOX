import { Button } from "@/shared/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { Label } from "@/shared/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import type { User, UserRole } from "../types";

type AssignRoleDialogProps = {
  isOpen: boolean;
  onClose: () => void;
  onAssign: () => void;
  user: User | null;
  roles: UserRole[];
  selectedRole: UserRole | "";
  onRoleChange: (role: UserRole | "") => void;
};

export const AssignRoleDialog = ({
  isOpen,
  onClose,
  onAssign,
  user,
  roles,
  selectedRole,
  onRoleChange,
}: AssignRoleDialogProps) => {
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Assign Role</DialogTitle>
          <DialogDescription>
            Select a role to assign to {user?.name.value}
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="role">Role</Label>
            <Select
              value={selectedRole}
              onValueChange={(value) => onRoleChange(value as UserRole | "")}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                {roles.map((role) => (
                  <SelectItem key={role} value={role}>
                    {role}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button onClick={onAssign} disabled={!selectedRole}>
            Assign Role
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

type ToggleStatusDialogProps = {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  user: User | null;
};

export const ToggleStatusDialog = ({
  isOpen,
  onClose,
  onConfirm,
  user,
}: ToggleStatusDialogProps) => {
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {user?.isActive ? "Block User" : "Activate User"}
          </DialogTitle>
          <DialogDescription>
            Are you sure you want to {user?.isActive ? "block" : "activate"}{" "}
            {user?.name.value}?
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Cancel
          </Button>
          <Button
            variant={user?.isActive ? "destructive" : "default"}
            onClick={onConfirm}
          >
            {user?.isActive ? "Block" : "Activate"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
