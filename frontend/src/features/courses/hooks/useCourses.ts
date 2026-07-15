import { useState, useEffect, useCallback } from "react";
import { fetchCoursesRaw } from "@/features/courses/api";
import type { CourseApiResponse } from "@/features/courses/types";

export const useCourses = (
  majorId: number,
  currentPage: number,
  pageSize: number,
  requirementType: string,
  requirementNature: string
) => {
  const [courses, setCourses] = useState<CourseApiResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [hasPreviousPage, setHasPreviousPage] = useState(false);
  const [hasNextPage, setHasNextPage] = useState(false);

  const fetchCourses = useCallback(async () => {
    if (!majorId) return;

    setLoading(true);
    try {
      const data = await fetchCoursesRaw(
        majorId,
        currentPage,
        pageSize,
        requirementType,
        requirementNature
      );
      setCourses(data.items);
      setTotalCount(data.totalCount);
      setTotalPages(data.totalPages);
      setHasPreviousPage(data.hasPreviousPage);
      setHasNextPage(data.hasNextPage);
    } catch (error) {
      console.error("Failed to fetch courses:", error);
      setCourses([]);
    } finally {
      setLoading(false);
    }
  }, [majorId, currentPage, pageSize, requirementType, requirementNature]);

  // Expose refetch function
  useEffect(() => {
    fetchCourses();
  }, [fetchCourses]);

  const refetch = fetchCourses;

  return {
    courses,
    loading,
    totalCount,
    totalPages,
    hasPreviousPage,
    hasNextPage,
    refetch,
  };
};
