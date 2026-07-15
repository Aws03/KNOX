import { apiClient } from "@/lib/api/apiClient";
import type { UserResponse, UserFilters, UserRole } from "./types";

/**
 * Fetches paginated user list with optional filters
 */
export const fetchUsers = async (
  page: number,
  pageSize: number,
  filters: UserFilters = {}
): Promise<UserResponse> => {
  try {
    const queryParams = new URLSearchParams({
      pageNumber: page.toString(),
      pageSize: pageSize.toString(),
    });

    // Add filters to query params
    if (filters.universityId) {
      queryParams.append("universityId", filters.universityId.toString());
    }
    if (filters.facultyId) {
      queryParams.append("facultyId", filters.facultyId.toString());
    }
    if (filters.majorId) {
      queryParams.append("majorId", filters.majorId.toString());
    }
    if (filters.email) {
      queryParams.append("email", filters.email);
    }
    if (filters.id) {
      queryParams.append("id", filters.id.toString());
    }
    if (filters.isActive !== undefined) {
      queryParams.append("isActive", filters.isActive.toString());
    }
    if (filters.isVerified !== undefined) {
      queryParams.append("isVerified", filters.isVerified.toString());
    }

    const response = await apiClient.get<UserResponse>(
      `/Users/details?${queryParams}`
    );
    return response.data;
  } catch (error) {
    console.error("Failed to fetch users:", error);
    throw new Error("Failed to fetch users. Please try again later.");
  }
};

/**
 * Fetches available user roles
 */
export const fetchUserRoles = async (): Promise<UserRole[]> => {
  try {
    const response = await apiClient.get<UserRole[]>("/Users/roles");
    return response.data;
  } catch (error) {
    console.error("Failed to fetch roles:", error);
    throw new Error("Failed to fetch roles. Please try again later.");
  }
};

/**
 * Assigns a role to a user
 */
export const assignRoleToUser = async (
  userId: number,
  role: UserRole
): Promise<void> => {
  try {
    await apiClient.post(`/Users/${userId}/assign-role`, role, {
      headers: { "Content-Type": "application/json" },
    });
  } catch (error) {
    console.error("Failed to assign role:", error);
    throw new Error("Failed to assign role. Please try again later.");
  }
};

/**
 * Blocks a user account
 */
export const blockUser = async (userId: number): Promise<void> => {
  try {
    await apiClient.post(`/Users/${userId}/block`);
  } catch (error) {
    console.error("Failed to block user:", error);
    throw new Error("Failed to block user. Please try again later.");
  }
};

/**
 * Activates a user account
 */
export const activateUser = async (userId: number): Promise<void> => {
  try {
    await apiClient.post(`/Users/${userId}/activate`);
  } catch (error) {
    console.error("Failed to activate user:", error);
    throw new Error("Failed to activate user. Please try again later.");
  }
};

/**
 * Toggles user active status (block/activate)
 */
export const toggleUserStatus = async (
  userId: number,
  isActive: boolean
): Promise<void> => {
  if (isActive) {
    await blockUser(userId);
  } else {
    await activateUser(userId);
  }
};
