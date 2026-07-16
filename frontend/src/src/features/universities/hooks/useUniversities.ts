import { useState, useEffect } from "react";
import type { University, PaginationInfo } from "../types";
import { fetchUniversities, createUniversity, updateUniversity } from "../api";

interface UseUniversitiesResult {
  universities: University[];
  loading: boolean;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
  paginationInfo: PaginationInfo;
  handleAddUniversity: (name: string) => Promise<void>;
  handleEditUniversity: (id: number, name: string) => Promise<void>;
  refreshUniversities: () => Promise<void>;
}

export const useUniversities = (): UseUniversitiesResult => {
  const [universities, setUniversities] = useState<University[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(10);
  const [paginationInfo, setPaginationInfo] = useState<PaginationInfo>({
    hasPreviousPage: false,
    hasNextPage: false,
  });

  const loadUniversities = async (page: number, search?: string) => {
    setLoading(true);
    try {
      const response = await fetchUniversities({
        pageNumber: page,
        pageSize,
        name: search,
      });

      setUniversities(response.items);
      setTotalPages(response.totalPages);
      setPaginationInfo({
        hasPreviousPage: response.hasPreviousPage,
        hasNextPage: response.hasNextPage,
      });
    } catch (error) {
      console.error("Failed to fetch universities:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUniversities(currentPage, searchTerm);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage]);

  useEffect(() => {
    const timer = setTimeout(() => {
      setCurrentPage(1);
      loadUniversities(1, searchTerm);
    }, 300);
    return () => clearTimeout(timer);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchTerm]);

  const handleAddUniversity = async (name: string) => {
    if (!name.trim()) return;

    try {
      await createUniversity({ name });
      await loadUniversities(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to add university:", error);
      throw error;
    }
  };

  const handleEditUniversity = async (id: number, name: string) => {
    if (!name.trim()) return;

    try {
      await updateUniversity(id, { name });
      await loadUniversities(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to edit university:", error);
      throw error;
    }
  };

  // const handleDeleteUniversity = async (id: number) => {
  //   try {
  //     await deleteUniversity(id);
  //     await loadUniversities(currentPage, searchTerm);
  //   } catch (error) {
  //     console.error("Failed to delete university:", error);
  //     throw error;
  //   }
  // };

  const refreshUniversities = async () => {
    await loadUniversities(currentPage, searchTerm);
  };

  return {
    universities,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleAddUniversity,
    handleEditUniversity,
    // handleDeleteUniversity,
    refreshUniversities,
  };
};
