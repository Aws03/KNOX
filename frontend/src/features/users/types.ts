// User related types
export interface User {
  id: number;
  name: {
    value: string;
  };
  email: {
    address: string;
  };
  isActive: boolean;
  isVerified: boolean;
  majorId: number;
  majorName: string;
  facultyId: number;
  facultyName: string;
  universityId: number;
  universityName: string;
}

export interface UserResponse {
  items: User[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PaginationInfo {
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface UserFilters {
  universityId?: number;
  facultyId?: number;
  majorId?: number;
  email?: string;
  id?: number;
  isActive?: boolean;
  isVerified?: boolean;
}

export type SearchType = "email" | "id";

// Academic structure types
export interface University {
  id: number;
  name: string;
}

export interface Faculty {
  id: number;
  name: string;
  universityId: number;
}

export interface Major {
  id: number;
  name: string;
  facultyId: number;
}

// Role types
export type UserRole = "SuperAdmin" | "Admin" | "Writer" | "User";
