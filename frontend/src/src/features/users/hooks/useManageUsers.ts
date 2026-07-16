import { useState, useEffect, useCallback } from "react";
import { toast } from "sonner";
import type { User, UserFilters, PaginationInfo, SearchType } from "../types";
import { fetchUsers as fetchUsersApi } from "../api";

export const useManageUsers = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [searchType, setSearchType] = useState<SearchType>("email");
  const [filters, setFilters] = useState<UserFilters>({});
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(10);
  const [paginationInfo, setPaginationInfo] = useState<PaginationInfo>({
    hasPreviousPage: false,
    hasNextPage: false,
  });

  const loadUsers = useCallback(
    async (page: number, appliedFilters: UserFilters = {}) => {
      setLoading(true);
      try {
        const response = await fetchUsersApi(page, pageSize, appliedFilters);

        setUsers(response.items);
        setTotalPages(response.totalPages);
        setPaginationInfo({
          hasPreviousPage: response.hasPreviousPage,
          hasNextPage: response.hasNextPage,
        });
      } catch (error) {
        toast.error("Failed to load users", {
          description:
            error instanceof Error ? error.message : "An error occurred",
        });
        setUsers([]);
      } finally {
        setLoading(false);
      }
    },
    [pageSize]
  );

  useEffect(() => {
    loadUsers(currentPage, filters);
  }, [currentPage, filters, loadUsers]);

  const handleSearch = useCallback(() => {
    const newFilters: UserFilters = { ...filters };

    delete newFilters.email;
    delete newFilters.id;

    if (searchTerm.trim()) {
      if (searchType === "email") {
        newFilters.email = searchTerm.trim();
      } else {
        const userId = parseInt(searchTerm.trim(), 10);
        if (!isNaN(userId)) {
          newFilters.id = userId;
        } else {
          toast.error("Invalid user ID", {
            description: "Please enter a valid numeric ID",
          });
          return;
        }
      }
    }

    setFilters(newFilters);
    setCurrentPage(1);
  }, [searchTerm, searchType, filters]);

  const handleApplyFilters = useCallback((newFilters: UserFilters) => {
    setFilters(newFilters);
    setCurrentPage(1);
  }, []);

  const handleResetFilters = useCallback(() => {
    setFilters({});
    setCurrentPage(1);
  }, []);

  const refreshUsers = useCallback(() => {
    loadUsers(currentPage, filters);
  }, [currentPage, filters, loadUsers]);

  return {
    users,
    loading,
    searchTerm,
    setSearchTerm,
    searchType,
    setSearchType,
    filters,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleApplyFilters,
    handleResetFilters,
    refreshUsers,
  };
};
