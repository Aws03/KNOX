import { useAuth } from "@/app/providers/useAuth";

export const useUserRole = () => {
  const { user } = useAuth();

  /**
   * Check if user has one or more specific roles
   * @param role - Single role string or array of role strings
   * @returns true if user has at least one of the specified roles
   */
  const hasRole = (role: string | string[]): boolean => {
    if (!user) return false;
    const roles = Array.isArray(role) ? role : [role];
    return roles.some((r) => user.roles.includes(r));
  };

  /**
   * Check if user can manage content (Writer, Admin, or SuperAdmin)
   * @returns true if user has content management permissions
   */
  const canManageContent = (): boolean => {
    return hasRole(["Writer", "Admin", "SuperAdmin"]);
  };

  /**
   * Check if user is a Writer
   */
  const isWriter = hasRole("Writer");

  /**
   * Check if user is an Admin or SuperAdmin
   */
  const isAdmin = hasRole(["Admin", "SuperAdmin"]);

  /**
   * Check if user is a SuperAdmin
   */
  const isSuperAdmin = hasRole("SuperAdmin");

  /**
   * Get user's display name (fullName preferred, fallback to name)
   */
  const getDisplayName = (): string => {
    return user?.fullName || user?.name || "User";
  };

  /**
   * Get user's academic information
   */
  const getAcademicInfo = () => {
    return {
      university: user?.universityName,
      universityId: user?.universityId,
      faculty: user?.facultyName,
      facultyId: user?.facultyId,
      major: user?.majorName,
      majorId: user?.majorId,
    };
  };

  /**
   * Check if user account is verified
   */
  const isVerified = (): boolean => {
    return user?.isVerified === true;
  };

  /**
   * Check if user account is active
   */
  const isActive = (): boolean => {
    return user?.isActive === true;
  };

  return {
    user,
    hasRole,
    canManageContent,
    isWriter,
    isAdmin,
    isSuperAdmin,
    getDisplayName,
    getAcademicInfo,
    isVerified,
    isActive,
  };
};
