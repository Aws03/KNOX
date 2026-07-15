// Core entities
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

// Pagination info
export interface PaginationInfo {
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// API Response types
export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export type UniversityResponse = PaginatedResponse<University>;
export type FacultyResponse = PaginatedResponse<Faculty>;
export type MajorResponse = PaginatedResponse<Major>;

// API request types
export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  name?: string;
}

export interface CreateUniversityRequest {
  name: string;
}

export interface UpdateUniversityRequest {
  name: string;
}

export interface CreateFacultyRequest {
  name: string;
  universityId: number;
}

export interface UpdateFacultyRequest {
  name: string;
  universityId: number;
}

export interface CreateMajorRequest {
  name: string;
  facultyId: number;
}

export interface UpdateMajorRequest {
  name: string;
  facultyId: number;
}
