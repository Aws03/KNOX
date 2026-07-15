import { useState, useEffect, useCallback } from "react";
import { LayoutGrid, List, BookOpen, Loader2 } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card } from "@/shared/ui/card";
import CourseFilter from "@/features/courses/components/CourseFilter";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import { PageSizeSelector } from "@/shared/components/pagination/PageSizeSelector";
import CourseCard from "@/features/courses/components/CourseCard";
import type { PaginatedResponse } from "@/lib/api/types";
import { useAuth } from "@/app/providers/useAuth";
import type { Course, CourseFilterValues } from "@/features/courses/types";
import { fetchCoursesByMajor } from "@/features/courses/api";

const CoursesPage = () => {
  const { isAuthenticated } = useAuth();
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(12);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [coursesData, setCoursesData] =
    useState<PaginatedResponse<Course> | null>(null);
  const [appliedFilters, setAppliedFilters] = useState<CourseFilterValues>({});

  const fetchCourses = useCallback(
    async (filters: CourseFilterValues, page: number, size: number) => {
      // If no major is selected, don't fetch
      if (!filters.majorId) {
        setCoursesData(null);
        setLoading(false);
        return;
      }

      setLoading(true);
      setError(null);

      try {
        const response = await fetchCoursesByMajor(
          filters.majorId,
          page,
          size,
          filters
        );
        setCoursesData(response);
      } catch (err) {
        console.error("Failed to fetch courses:", err);
        setError(err instanceof Error ? err.message : "Failed to load courses");
        setCoursesData(null);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  // Fetch courses when filters or pagination changes
  useEffect(() => {
    fetchCourses(appliedFilters, currentPage, pageSize);
  }, [appliedFilters, currentPage, pageSize, fetchCourses]);

  const handleCourseClick = (_courseId: number) => {
    // TODO: Navigate to course detail page
  };

  const handleApplyFilters = (filters: CourseFilterValues) => {
    setAppliedFilters(filters);
    setCurrentPage(1); // Reset to first page when filters change
  };

  const handleResetFilters = () => {
    setAppliedFilters({});
    setCurrentPage(1);
    setCoursesData(null);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const currentCourses = coursesData?.items || [];
  const totalCount = coursesData?.totalCount || 0;
  const totalPages = coursesData?.totalPages || 0;
  const hasPreviousPage = coursesData?.hasPreviousPage || false;
  const hasNextPage = coursesData?.hasNextPage || false;

  return (
    <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
      {/* Filter */}
      <CourseFilter
        isLoggedIn={isAuthenticated}
        onApply={handleApplyFilters}
        onReset={handleResetFilters}
      />
      <div className="container mx-auto px-4 py-2 sm:px-6 lg:px-8">
        {/* Toolbar */}
        <div className="mb-2 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex items-center gap-4">
            <p className="text-sm text-muted-foreground">
              {totalCount > 0 ? (
                <>
                  Showing{" "}
                  <span className="font-semibold text-foreground">
                    {(currentPage - 1) * pageSize + 1}-
                    {Math.min(currentPage * pageSize, totalCount)}
                  </span>{" "}
                  of{" "}
                  <span className="font-semibold text-foreground">
                    {totalCount}
                  </span>{" "}
                  courses
                </>
              ) : (
                "No courses to display"
              )}
            </p>
            <PageSizeSelector pageSize={pageSize} onChange={setPageSize} />
          </div>

          <div className="flex items-center rounded-lg border p-1 bg-background shadow-sm">
            <Button
              variant={viewMode === "grid" ? "secondary" : "ghost"}
              size="sm"
              onClick={() => setViewMode("grid")}
              className="h-8 px-3"
            >
              <LayoutGrid className="h-4 w-4" />
            </Button>
            <Button
              variant={viewMode === "list" ? "secondary" : "ghost"}
              size="sm"
              onClick={() => setViewMode("list")}
              className="h-8 px-3"
            >
              <List className="h-4 w-4" />
            </Button>
          </div>
        </div>

        {/* Loading State */}
        {loading && (
          <div className="flex flex-col items-center justify-center py-16 gap-3">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <p className="text-sm text-muted-foreground">Loading courses...</p>
          </div>
        )}

        {/* Error State */}
        {error && !loading && (
          <Card className="p-12 bg-linear-to-br from-destructive/5 to-destructive/10 border-destructive/20">
            <div className="flex flex-col items-center justify-center text-center">
              <div className="flex h-20 w-20 items-center justify-center rounded-full bg-destructive/10 mb-4">
                <BookOpen className="h-10 w-10 text-destructive" />
              </div>
              <h3 className="text-lg font-semibold mb-2 text-destructive">
                Failed to load courses
              </h3>
              <p className="text-sm text-muted-foreground max-w-sm mb-4">
                {error}
              </p>
              <Button
                onClick={() =>
                  fetchCourses(appliedFilters, currentPage, pageSize)
                }
                variant="outline"
              >
                Try Again
              </Button>
            </div>
          </Card>
        )}

        {/* Grid View */}
        {!loading &&
          !error &&
          viewMode === "grid" &&
          currentCourses.length > 0 && (
            <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
              {currentCourses.map((course) => (
                <CourseCard
                  key={course.id}
                  course={course}
                  viewMode="grid"
                  onClick={handleCourseClick}
                />
              ))}
            </div>
          )}

        {/* List View */}
        {!loading &&
          !error &&
          viewMode === "list" &&
          currentCourses.length > 0 && (
            <div className="space-y-4">
              {currentCourses.map((course) => (
                <CourseCard
                  key={course.id}
                  course={course}
                  viewMode="list"
                  onClick={handleCourseClick}
                />
              ))}
            </div>
          )}

        {/* Empty State - No filters applied */}
        {!loading &&
          !error &&
          currentCourses.length === 0 &&
          !appliedFilters.majorId && (
            <Card className="p-12 bg-linear-to-br from-card to-muted/20">
              <div className="flex flex-col items-center justify-center text-center">
                <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
                  <BookOpen className="h-10 w-10 text-primary" />
                </div>
                <h3 className="text-lg font-semibold mb-2">
                  Select filters to view courses
                </h3>
                <p className="text-sm text-muted-foreground max-w-sm">
                  Please select at least a university, college, and major to see
                  available courses
                </p>
              </div>
            </Card>
          )}

        {/* Empty State - Filters applied but no results */}
        {!loading &&
          !error &&
          currentCourses.length === 0 &&
          appliedFilters.majorId && (
            <Card className="p-12 bg-linear-to-br from-card to-muted/20">
              <div className="flex flex-col items-center justify-center text-center">
                <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
                  <BookOpen className="h-10 w-10 text-primary" />
                </div>
                <h3 className="text-lg font-semibold mb-2">No courses found</h3>
                <p className="text-sm text-muted-foreground max-w-sm">
                  Try adjusting your filters to find the courses you're looking
                  for
                </p>
              </div>
            </Card>
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

export default CoursesPage;
