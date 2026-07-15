import { useState, useEffect } from "react";
import type { Faculty, Major, PaginationInfo } from "../types";
import {
  fetchFacultyById,
  fetchMajorsByFaculty,
  createMajor,
  updateMajor,
  // deleteMajor,
} from "../api";

interface UseFacultyDetailsProps {
  facultyId: number;
  pageSize?: number;
}

export const useFacultyDetails = ({
  facultyId,
  pageSize = 10,
}: UseFacultyDetailsProps) => {
  const [faculty, setFaculty] = useState<Faculty | null>(null);
  const [majors, setMajors] = useState<Major[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [paginationInfo, setPaginationInfo] = useState<PaginationInfo>({
    hasPreviousPage: false,
    hasNextPage: false,
  });

  // Fetch faculty details
  const loadFaculty = async () => {
    try {
      const data = await fetchFacultyById(facultyId);
      setFaculty(data);
    } catch (error) {
      console.error("Failed to fetch faculty:", error);
    }
  };

  // Fetch majors
  const loadMajors = async (page: number, search?: string) => {
    setLoading(true);
    try {
      const response = await fetchMajorsByFaculty(facultyId, {
        pageNumber: page,
        pageSize,
        name: search,
      });

      setMajors(response.items);
      setTotalPages(response.totalPages);
      setPaginationInfo({
        hasPreviousPage: response.hasPreviousPage,
        hasNextPage: response.hasNextPage,
      });
    } catch (error) {
      console.error("Failed to fetch majors:", error);
    } finally {
      setLoading(false);
    }
  };

  // Handle search
  const handleSearch = () => {
    setCurrentPage(1);
    loadMajors(1, searchTerm);
  };

  // Handle clear search
  const handleClearSearch = () => {
    setSearchTerm("");
    setCurrentPage(1);
    loadMajors(1, "");
  };

  // Handle add major
  const handleAddMajor = async (name: string) => {
    if (!name.trim()) return;

    try {
      await createMajor({ name, facultyId });
      loadMajors(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to add major:", error);
      throw error;
    }
  };

  // Handle edit major
  const handleEditMajor = async (majorId: number, name: string) => {
    if (!name.trim()) return;

    try {
      await updateMajor(majorId, { name, facultyId });
      loadMajors(currentPage, searchTerm);
    } catch (error) {
      console.error("Failed to edit major:", error);
      throw error;
    }
  };

  // Handle delete major
  // const handleDeleteMajor = async (majorId: number) => {
  //   try {
  //     await deleteMajor(majorId);
  //     loadMajors(currentPage, searchTerm);
  //   } catch (error) {
  //     console.error("Failed to delete major:", error);
  //     throw error;
  //   }
  // };

  useEffect(() => {
    loadFaculty();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [facultyId]);

  useEffect(() => {
    loadMajors(currentPage, searchTerm);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, facultyId]);

  return {
    faculty,
    majors,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleClearSearch,
    handleAddMajor,
    handleEditMajor,
    // handleDeleteMajor,
  };
};
