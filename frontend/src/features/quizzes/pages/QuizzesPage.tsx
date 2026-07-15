import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useUserRole } from "@/hooks/useUserRole";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import { useQuizzesList } from "../hooks/useQuizzesList";
import { QuizzesHeader } from "../components/QuizzesHeader";
import { QuizzesToolbar } from "../components/QuizzesToolbar";
import { QuizLoadingState } from "../components/QuizLoadingState";
import { QuizErrorState } from "../components/QuizErrorState";
import { QuizEmptyState } from "../components/QuizEmptyState";
import { QuizCardGrid } from "../components/QuizCardGrid";
import { QuizCardList } from "../components/QuizCardList";

interface QuizzesPageProps {
  mode?: "public" | "manage";
}

const QuizzesPage = ({ mode = "public" }: QuizzesPageProps) => {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const { canManageContent } = useUserRole();
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");

  // Check if user can see management UI
  const isManagementMode = mode === "manage" && canManageContent();

  // Use custom hook for quiz data management
  const {
    loading,
    error,
    currentPage,
    pageSize,
    setCurrentPage,
    setPageSize,
    loadQuizzes,
    currentQuizzes,
    totalCount,
    totalPages,
    hasPreviousPage,
    hasNextPage,
  } = useQuizzesList({ courseId });

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleQuizClick = (quizId: number) => {
    navigate(`/courses/${courseId}/quizzes/${quizId}`);
  };

  const handleAddQuiz = () => {
    navigate(`/courses/${courseId}/quizzes/add`);
  };

  const handleEditQuiz = (_quizId: number) => {
    // TODO: Implement edit functionality
  };

  const handleDeleteQuiz = (_quizId: number) => {
    // TODO: Implement delete functionality
  };

  return (
    <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
      <div className="container mx-auto px-4 py-6 sm:px-6 lg:px-8">
        {/* Header */}
        <QuizzesHeader
          isManagementMode={isManagementMode}
          totalCount={totalCount}
          onBack={() => navigate(-1)}
          onAddQuiz={isManagementMode ? handleAddQuiz : undefined}
        />

        {/* Toolbar */}
        <QuizzesToolbar
          totalCount={totalCount}
          currentPage={currentPage}
          pageSize={pageSize}
          viewMode={viewMode}
          onViewModeChange={setViewMode}
          onPageSizeChange={setPageSize}
        />

        {/* Loading State */}
        {loading && <QuizLoadingState />}

        {/* Error State */}
        {error && !loading && (
          <QuizErrorState
            error={error}
            onRetry={() => loadQuizzes(currentPage, pageSize)}
          />
        )}

        {/* Grid View */}
        {!loading &&
          !error &&
          viewMode === "grid" &&
          currentQuizzes.length > 0 && (
            <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
              {currentQuizzes.map((quiz) => (
                <QuizCardGrid
                  key={quiz.id}
                  quiz={quiz}
                  courseId={courseId!}
                  isManagementMode={isManagementMode}
                  onQuizClick={handleQuizClick}
                  onEdit={handleEditQuiz}
                  onDelete={handleDeleteQuiz}
                />
              ))}
            </div>
          )}

        {/* List View */}
        {!loading &&
          !error &&
          viewMode === "list" &&
          currentQuizzes.length > 0 && (
            <div className="space-y-4">
              {currentQuizzes.map((quiz) => (
                <QuizCardList
                  key={quiz.id}
                  quiz={quiz}
                  courseId={courseId!}
                  isManagementMode={isManagementMode}
                  onQuizClick={handleQuizClick}
                  onEdit={handleEditQuiz}
                  onDelete={handleDeleteQuiz}
                />
              ))}
            </div>
          )}

        {/* Empty State */}
        {!loading && !error && currentQuizzes.length === 0 && (
          <QuizEmptyState />
        )}

        {/* Pagination */}
        {!loading && !error && totalPages > 1 && (
          <div className="mt-8 flex justify-center">
            <SmartPagination
              pageNumber={currentPage}
              totalPages={totalPages}
              hasPreviousPage={hasPreviousPage}
              hasNextPage={hasNextPage}
              onPageChange={handlePageChange}
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default QuizzesPage;
