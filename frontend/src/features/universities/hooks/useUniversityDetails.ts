import { useState, useEffect } from "react";
import type { University, Faculty, PaginationInfo } from "../types";
import {
  fetchUniversityById,
  fetchFacultiesByUniversity,
  createFaculty,
  updateFaculty,
  // deleteFaculty,
} from "../api";

interface UseUniversityDetailsParams {
  universityId: number;
}

interface UseUniversityDetailsResult {
  university: University | null;
  faculties: Faculty[];
  loading: boolean;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
  paginationInfo: PaginationInfo;
  handleSearch: () => void;
  handleClearSearch: () => void;
  handleAddFaculty: (name: string) => Promise<void>;
  handleEditFaculty: (id: number, name: string) => Promise<void>;
  // handleDeleteFaculty: (id: number) => Promise<void>;
}

export const useUniversityDetails = ({
  universityId,
}: UseUniversityDetailsParams): UseUniversityDetailsResult => {
  const [university, setUniversity] = useState<University | null>(null);
  const [faculties, setFaculties] = useState<Faculty[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(10);
  const [paginationInfo, setPaginationInfo] = useState<PaginationInfo>({
    hasPreviousPage: false,
    hasNextPage: false,
  });

  const loadUniversity = async () => {
    try {
      const data = await fetchUniversityById(universityId);
      setUniversity(data);
    } catch (error) {
      console.error("Failed to fetch university:", error);
    }
  };

  const loadFaculties = async (page: number, search?: string) => {
    setLoading(true);
    try {
      const response = await fetchFacultiesByUniversity(universityId, {
        pageNumber: page,
        pageSize,
        name: search,
      });

      setFaculties(response.items);
      setTotalPages(response.totalPages);
      setPaginationInfo({
        hasPreviousPage: response.hasPreviousPage,
        hasNextPage: response.hasNextPage,
      });
    } catch (error) {
      console.error("Failed to fetch faculties:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUniversity();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [universityId]);

  useEffect(() => {
    loadFaculties(currentPage, searchTerm);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, universityId]);

  const handleSearch = () => {
    setCurrentPage(1);
    loadFaculties(1, searchTerm);
  };

  const handleClearSearch = () => {
    setSearchTerm("");
    setCurrentPage(1);
    loadFaculties(1, "");
  };

  const handleAddFaculty = async (name: string) => {
    if (!name.trim()) return;

    try {
      await createFaculty({ name, universityId });
      await loadFaculties(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to add faculty:", error);
      throw error;
    }
  };

  const handleEditFaculty = async (id: number, name: string) => {
    if (!name.trim()) return;

    try {
      await updateFaculty(id, { name, universityId });
      await loadFaculties(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to edit faculty:", error);
      throw error;
    }
  };

  // const handleDeleteFaculty = async (id: number) => {
  //   try {
  //     await deleteFaculty(id);
  //     await loadFaculties(currentPage, searchTerm);
  //   } catch (error) {
  //     console.error("Failed to delete faculty:", error);
  //     throw error;
  //   }
  // };

  return {
    university,
    faculties,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleClearSearch,
    handleAddFaculty,
    handleEditFaculty,
    // handleDeleteFaculty,
  };
};
