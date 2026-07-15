import { useState, useEffect } from "react";
import {
  getCourseContents,
  GetMyCourseContent,
  deleteFolder,
  deleteMaterial,
} from "../api";
import type {
  FolderContentsResponse,
  FolderItem,
  MaterialItem,
} from "../types";

interface UseFolderContentsProps {
  courseId: string | undefined;
  folderId: string | undefined;
  isManagementMode?: boolean;
}

interface UseFolderContentsReturn {
  contents: FolderContentsResponse | null;
  loading: boolean;
  error: string | null;
  handleDeleteFolder: (folderId: number) => Promise<void>;
  handleDeleteMaterial: (materialId: number) => Promise<void>;
  handleEditFolder: (folder: FolderItem) => void;
  handleEditMaterial: (material: MaterialItem) => void;
}

export function useFolderContents({
  courseId,
  folderId,
  isManagementMode = false,
}: UseFolderContentsProps): UseFolderContentsReturn {
  const [loading, setLoading] = useState(true);
  const [contents, setContents] = useState<FolderContentsResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadFolderContents() {
      if (!courseId) return;

      setLoading(true);
      setError(null);

      try {
        const data = isManagementMode
          ? await GetMyCourseContent(courseId, folderId)
          : await getCourseContents(courseId, folderId);
        setContents(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load folder");
        console.error("Error loading folder contents:", err);
      } finally {
        setLoading(false);
      }
    }

    loadFolderContents();
  }, [courseId, folderId, isManagementMode]);

  const handleDeleteFolder = async (folderIdToDelete: number) => {
    if (!confirm("Are you sure you want to delete this folder?")) return;
    if (!courseId) return;

    try {
      await deleteFolder(folderIdToDelete);
      const data = isManagementMode
        ? await GetMyCourseContent(courseId, folderId)
        : await getCourseContents(courseId, folderId);
      setContents(data);
    } catch (err) {
      console.error("Error deleting folder:", err);
      alert("Failed to delete folder");
    }
  };

  const handleDeleteMaterial = async (materialId: number) => {
    if (!confirm("Are you sure you want to delete this material?")) return;
    if (!courseId) return;

    try {
      await deleteMaterial(materialId);
      const data = isManagementMode
        ? await GetMyCourseContent(courseId, folderId)
        : await getCourseContents(courseId, folderId);
      setContents(data);
    } catch (err) {
      console.error("Error deleting material:", err);
      alert("Failed to delete material");
    }
  };

  const handleEditFolder = (_folder: FolderItem) => {
    // TODO: Implement edit folder dialog
  };

  const handleEditMaterial = (_material: MaterialItem) => {
    // TODO: Implement edit material dialog
  };

  return {
    contents,
    loading,
    error,
    handleDeleteFolder,
    handleDeleteMaterial,
    handleEditFolder,
    handleEditMaterial,
  };
}
