import { useState, useEffect, useCallback } from "react";
import type { PaginatedResponse } from "@/lib/api/types";
import type { QuizListItem } from "../types";
import { fetchQuizzesByCourse } from "../api";

interface UseQuizzesListProps {
  courseId: string | undefined;
}

interface UseQuizzesListReturn {
  loading: boolean;
  error: string | null;
  quizzesData: PaginatedResponse<QuizListItem> | null;
  currentPage: number;
  pageSize: number;
  setCurrentPage: (page: number) => void;
  setPageSize: (size: number) => void;
  loadQuizzes: (page: number, size: number) => Promise<void>;
  currentQuizzes: QuizListItem[];
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const useQuizzesList = ({
  courseId,
}: UseQuizzesListProps): UseQuizzesListReturn => {
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(12);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [quizzesData, setQuizzesData] =
    useState<PaginatedResponse<QuizListItem> | null>(null);

  const loadQuizzes = useCallback(
    async (page: number, size: number) => {
      if (!courseId) return;

      setLoading(true);
      setError(null);

      try {
        const response = await fetchQuizzesByCourse(courseId, page, size);
        setQuizzesData(response);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load quizzes");
        setQuizzesData(null);
      } finally {
        setLoading(false);
      }
    },
    [courseId]
  );

  useEffect(() => {
    loadQuizzes(currentPage, pageSize);
  }, [currentPage, pageSize, loadQuizzes]);

  return {
    loading,
    error,
    quizzesData,
    currentPage,
    pageSize,
    setCurrentPage,
    setPageSize,
    loadQuizzes,
    currentQuizzes: quizzesData?.items || [],
    totalCount: quizzesData?.totalCount || 0,
    totalPages: quizzesData?.totalPages || 0,
    hasPreviousPage: quizzesData?.hasPreviousPage || false,
    hasNextPage: quizzesData?.hasNextPage || false,
  };
};
