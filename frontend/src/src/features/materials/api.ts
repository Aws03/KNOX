import { apiClient } from "@/lib/api/apiClient";
import type {
  FolderContentsResponse,
  CourseMaterialsResponse,
  FolderItem,
  MaterialItem,
} from "./types";

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

/**
 * Create a new folder within a course, optionally nested under a parent folder
 */
export async function createFolder(
  courseId: string,
  data: { name: string; parentFolderId?: number; description?: string }
): Promise<FolderItem> {
  const response = await apiClient.post<FolderItem>(`/courses/${courseId}/folders`, {
    name: data.name,
    parentFolderId: data.parentFolderId ?? null,
    description: data.description || null,
  });
  return response.data;
}

/**
 * Upload a material file to permanent storage, returning its content URL
 */
export async function uploadMaterialFile(file: File): Promise<string> {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("fileCategory", "material");

  const response = await apiClient.post<{ fileUrl: string }>(
    "/files/upload/permanent",
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return response.data.fileUrl;
}

/**
 * Create a course material record pointing at an already-uploaded file
 */
export async function createMaterial(
  courseId: string,
  data: {
    title: string;
    contentUrl: string;
    folderId?: number;
    description?: string;
  }
): Promise<MaterialItem> {
  const response = await apiClient.post<MaterialItem>(`/courses/${courseId}/materials`, {
    title: data.title,
    contemtUrl: data.contentUrl,
    folderId: data.folderId ?? null,
    description: data.description || null,
  });
  return response.data;
}
