import { apiClient } from "@/lib/api/apiClient";
import type { FolderContentsResponse, CourseMaterialsResponse } from "./types";

/**
 * Fetch course contents (folders and materials) at root level or within a specific folder
 */
export async function getCourseContents(
  courseId: string,
  folderId?: string
): Promise<FolderContentsResponse> {
  const url = `/courses/${courseId}/contents${
    folderId ? `?folderId=${folderId}` : ""
  }`;
  const response = await apiClient.get<FolderContentsResponse>(url);
  return response.data;
}

//get the course content that the user writed it
export async function GetMyCourseContent(
  courseId: string,
  folderId?: string
): Promise<FolderContentsResponse> {
  const url = `/courses/${courseId}/my-contents${
    folderId ? `?folderId=${folderId}` : ""
  }`;
  const response = await apiClient.get<FolderContentsResponse>(url);
  return response.data;
}

/**
 * Fetch root level course materials (no folder)
 */
export async function getCourseMaterials(
  courseId: string
): Promise<CourseMaterialsResponse> {
  const response = await apiClient.get<CourseMaterialsResponse>(
    `/courses/${courseId}/contents`
  );
  return response.data;
}

/**
 * Delete a folder by ID
 */
export async function deleteFolder(folderId: number): Promise<void> {
  await apiClient.delete(`/folders/${folderId}`);
}

/**
 * Delete a material by ID
 */
export async function deleteMaterial(materialId: number): Promise<void> {
  await apiClient.delete(`/materials/${materialId}`);
}
