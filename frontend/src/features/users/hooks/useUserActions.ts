import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import type { User, UserRole } from "../types";
import { fetchUserRoles, assignRoleToUser, toggleUserStatus } from "../api";

export const useUserActions = (onSuccess?: () => void) => {
  const [roles, setRoles] = useState<UserRole[]>([]);
  const [isAssignRoleDialogOpen, setIsAssignRoleDialogOpen] = useState(false);
  const [isBlockDialogOpen, setIsBlockDialogOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [selectedRole, setSelectedRole] = useState<UserRole | "">("");

  useEffect(() => {
    const loadRoles = async () => {
      try {
        const rolesData = await fetchUserRoles();
        setRoles(rolesData);
      } catch (error) {
        toast.error("Failed to load roles", {
          description:
            error instanceof Error ? error.message : "An error occurred",
        });
      }
    };
    loadRoles();
  }, []);

  const openAssignRoleDialog = useCallback((user: User) => {
    setSelectedUser(user);
    setIsAssignRoleDialogOpen(true);
  }, []);

  const closeAssignRoleDialog = useCallback(() => {
    setIsAssignRoleDialogOpen(false);
    setSelectedUser(null);
    setSelectedRole("");
  }, []);

  const openBlockDialog = useCallback((user: User) => {
    setSelectedUser(user);
    setIsBlockDialogOpen(true);
  }, []);

  const closeBlockDialog = useCallback(() => {
    setIsBlockDialogOpen(false);
    setSelectedUser(null);
  }, []);

  const handleAssignRole = useCallback(async () => {
    if (!selectedUser || !selectedRole) return;

    try {
      await assignRoleToUser(selectedUser.id, selectedRole as UserRole);

      const userName = selectedUser.name.value;
      const roleName = selectedRole;
      closeAssignRoleDialog();

      toast.success("Role assigned successfully", {
        description: `${userName} is now assigned the ${roleName} role.`,
      });

      onSuccess?.();
    } catch (error) {
      toast.error("Failed to assign role", {
        description:
          error instanceof Error ? error.message : "An error occurred",
      });
    }
  }, [selectedUser, selectedRole, closeAssignRoleDialog, onSuccess]);

  const handleToggleUserStatus = useCallback(
    async (user: User) => {
      try {
        await toggleUserStatus(user.id, user.isActive);

        const action = user.isActive ? "blocked" : "activated";
        closeBlockDialog();

        toast.success(`User ${action} successfully`, {
          description: `${user.name.value} has been ${action}.`,
        });

        onSuccess?.();
      } catch (error) {
        toast.error("Failed to update user status", {
          description:
            error instanceof Error ? error.message : "An error occurred",
        });
      }
    },
    [closeBlockDialog, onSuccess]
  );

  return {
    roles,
    isAssignRoleDialogOpen,
    isBlockDialogOpen,
    selectedUser,
    selectedRole,
    setSelectedRole,
    openAssignRoleDialog,
    closeAssignRoleDialog,
    openBlockDialog,
    closeBlockDialog,
    handleAssignRole,
    handleToggleUserStatus,
  };
};
