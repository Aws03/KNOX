import { apiClient } from "@/lib/api/apiClient";
import type {
  UniversityResponse,
  FacultyResponse,
  MajorResponse,
  University,
  Faculty,
  Major,
  PaginationParams,
  CreateUniversityRequest,
  UpdateUniversityRequest,
  CreateFacultyRequest,
  UpdateFacultyRequest,
  CreateMajorRequest,
  UpdateMajorRequest,
} from "./types";

// ==================== Universities API ====================

/**
 * Fetch paginated list of universities
 */
export const fetchUniversities = async (
  params: PaginationParams
): Promise<UniversityResponse> => {
  const queryParams = new URLSearchParams({
    pageNumber: params.pageNumber.toString(),
    pageSize: params.pageSize.toString(),
    ...(params.name && { name: params.name }),
  });
  const response = await apiClient.get<UniversityResponse>(
    `/universities?${queryParams}`
  );
  return response.data;
};

/**
 * Fetch single university by ID
 */
export const fetchUniversityById = async (id: number): Promise<University> => {
  const response = await apiClient.get<University>(`/universities/${id}`);
  return response.data;
};

/**
 * Create a new university
 */
export const createUniversity = async (
  data: CreateUniversityRequest
): Promise<University> => {
  const response = await apiClient.post<University>("/universities", data);
  return response.data;
};

/**
 * Update an existing university
 */
export const updateUniversity = async (
  id: number,
  data: UpdateUniversityRequest
): Promise<University> => {
  const response = await apiClient.put<University>(`/universities/${id}`, data);
  return response.data;
};

// /**
//  * Delete (inactivate) a university
//  */
// export const deleteUniversity = async (id: number): Promise<void> => {
//   await apiClient.delete(`/universities/${id}`);
// };

// ==================== Faculties API ====================

/**
 * Fetch paginated list of faculties by university
 */
export const fetchFacultiesByUniversity = async (
  universityId: number,
  params: PaginationParams
): Promise<FacultyResponse> => {
  const queryParams = new URLSearchParams({
    pageNumber: params.pageNumber.toString(),
    pageSize: params.pageSize.toString(),
    ...(params.name && { name: params.name }),
  });
  const response = await apiClient.get<FacultyResponse>(
    `/faculties/by-university/${universityId}?${queryParams}`
  );
  return response.data;
};

/**
 * Fetch single faculty by ID
 */
export const fetchFacultyById = async (id: number): Promise<Faculty> => {
  const response = await apiClient.get<Faculty>(`/faculties/${id}`);
  return response.data;
};

/**
 * Create a new faculty
 */
export const createFaculty = async (
  data: CreateFacultyRequest
): Promise<Faculty> => {
  const response = await apiClient.post<Faculty>("/faculties", data);
  return response.data;
};

/**
 * Update an existing faculty
 */
export const updateFaculty = async (
  id: number,
  data: UpdateFacultyRequest
): Promise<Faculty> => {
  const response = await apiClient.put<Faculty>(`/faculties/${id}`, data);
  return response.data;
};

/**
 * Delete (inactivate) a faculty
 */
// export const deleteFaculty = async (id: number): Promise<void> => {
//   await apiClient.delete(`/faculties/${id}`);
// };

// ==================== Majors API ====================

/**
 * Fetch paginated list of majors by faculty
 */
export const fetchMajorsByFaculty = async (
  facultyId: number,
  params: PaginationParams
): Promise<MajorResponse> => {
  const queryParams = new URLSearchParams({
    pageNumber: params.pageNumber.toString(),
    pageSize: params.pageSize.toString(),
    ...(params.name && { name: params.name }),
  });
  const response = await apiClient.get<MajorResponse>(
    `/majors/by-faculty/${facultyId}?${queryParams}`
  );
  return response.data;
};

/**
 * Create a new major
 */
export const createMajor = async (data: CreateMajorRequest): Promise<Major> => {
  const response = await apiClient.post<Major>("/majors", data);
  return response.data;
};

/**
 * Update an existing major
 */
export const updateMajor = async (
  id: number,
  data: UpdateMajorRequest
): Promise<Major> => {
  const response = await apiClient.put<Major>(`/majors/${id}`, data);
  return response.data;
};

/**
 * Delete (inactivate) a major
 */
// export const deleteMajor = async (id: number): Promise<void> => {
//   await apiClient.delete(`/majors/${id}`);
// };
