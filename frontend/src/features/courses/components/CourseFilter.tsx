import { useEffect, useState } from "react";
import { useAuth } from "@/app/providers/useAuth";
import { useFilterState } from "../hooks/useFilterState";
import { useCascadingData } from "../hooks/useCascadingData";
import { FilterMobile } from "./FilterMobile";
import { FilterDesktop } from "./FilterDesktop";
import type { CourseFilterValues, University, Faculty, Major } from "../types";

type CourseFilterProps = {
  isLoggedIn?: boolean;
  onApply?: (filters: CourseFilterValues) => void;
  onReset?: (filters: CourseFilterValues) => void;
};

export type { CourseFilterValues };

const CourseFilter = ({
  isLoggedIn = false,
  onApply,
  onReset,
}: CourseFilterProps) => {
  const { user } = useAuth();
  const [isInitialized, setIsInitialized] = useState(false);
  const [appliedFilters, setAppliedFilters] =
    useState<CourseFilterValues | null>(null);

  const filterState = useFilterState();
  const cascadingData = useCascadingData();

  // Check if current filters differ from applied filters
  const hasUnappliedChanges = Boolean(
    filterState.hasAcademicSelection &&
      appliedFilters &&
      (filterState.selectedUniversity?.id !== appliedFilters.universityId ||
        filterState.selectedCollege?.id !== appliedFilters.collegeId ||
        filterState.selectedMajor?.id !== appliedFilters.majorId ||
        filterState.selectedCourseType !== (appliedFilters.courseType || "") ||
        filterState.selectedRequirement !== (appliedFilters.requirement || ""))
  );

  // Restore stored filters and ensure data is loaded
  useEffect(() => {
    if (filterState.isRestoredFromStorage && !isInitialized) {
      const restoreFilters = async () => {
        try {
          if (filterState.selectedUniversity) {
            cascadingData.ensureUniversityExists(
              filterState.selectedUniversity
            );

            if (filterState.selectedCollege) {
              await cascadingData.fetchFacultiesData(
                filterState.selectedUniversity.id
              );
              cascadingData.ensureFacultyExists(filterState.selectedCollege);

              if (filterState.selectedMajor) {
                await cascadingData.fetchMajorsData(
                  filterState.selectedCollege.id
                );
                cascadingData.ensureMajorExists(filterState.selectedMajor);
              }
            }
          }

          // Auto-apply restored filters
          if (filterState.hasAcademicSelection) {
            setTimeout(() => {
              setAppliedFilters(filterState.currentFilterValues);
              onApply?.(filterState.currentFilterValues);
            }, 100);
          }

          setIsInitialized(true);
          filterState.setIsRestoredFromStorage(false);
        } catch (error) {
          console.error("Failed to restore filters:", error);
          setIsInitialized(true);
        }
      };
      restoreFilters();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filterState.isRestoredFromStorage]);

  // Initialize with user data if logged in
  useEffect(() => {
    if (!isLoggedIn || !user || isInitialized) return;

    const initializeWithUserData = async () => {
      try {
        // Check if we should use stored state or user data
        const shouldUseUserData =
          !filterState.isRestoredFromStorage ||
          (filterState.isRestoredFromStorage &&
            filterState.selectedUniversity?.id !== user.universityId);

        if (shouldUseUserData && user.universityId && user.universityName) {
          const userUniversity: University = {
            id: user.universityId,
            name: user.universityName,
          };
          filterState.setSelectedUniversity(userUniversity);
          cascadingData.ensureUniversityExists(userUniversity);

          if (user.facultyId && user.facultyName) {
            await cascadingData.fetchFacultiesData(user.universityId);
            const userFaculty: Faculty = {
              id: user.facultyId,
              name: user.facultyName,
              universityId: user.universityId,
            };
            filterState.setSelectedCollege(userFaculty);
            cascadingData.ensureFacultyExists(userFaculty);

            if (user.majorId && user.majorName) {
              await cascadingData.fetchMajorsData(user.facultyId);
              const userMajor: Major = {
                id: user.majorId,
                name: user.majorName,
                facultyId: user.facultyId,
              };
              filterState.setSelectedMajor(userMajor);
              cascadingData.ensureMajorExists(userMajor);

              setTimeout(() => {
                onApply?.({
                  universityId: user.universityId,
                  collegeId: user.facultyId,
                  majorId: user.majorId,
                  majorName: user.majorName,
                });
              }, 100);
            }
          }
        }

        if (filterState.isRestoredFromStorage) {
          filterState.setIsRestoredFromStorage(false);
        }
        setIsInitialized(true);
      } catch (error) {
        console.error("Failed to initialize filter with user data:", error);
        setIsInitialized(true);
      }
    };

    initializeWithUserData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isLoggedIn, user, isInitialized]);

  const handleClearFilters = () => {
    setAppliedFilters(null);
    if (isLoggedIn && user) {
      filterState.clearFilters(true);
      onReset?.({
        universityId: user.universityId,
        collegeId: user.facultyId,
        majorId: user.majorId,
        majorName: user.majorName,
      });
    } else {
      filterState.clearFilters(false);
      cascadingData.resetFaculties();
      cascadingData.resetMajors();
      onReset?.({});
    }
  };

  const handleApplyFilters = () => {
    if (!filterState.hasAcademicSelection) return;
    setAppliedFilters(filterState.currentFilterValues);
    onApply?.(filterState.currentFilterValues);
  };

  // Handle cascading selections
  const handleUniversityChange = (value: string) => {
    const nextUniversity =
      cascadingData.universitiesState.items.find(
        (uni) => uni.id.toString() === value
      ) ?? null;
    filterState.setSelectedUniversity(nextUniversity);
    filterState.setSelectedCollege(null);
    filterState.setSelectedMajor(null);
    cascadingData.resetFaculties();
    cascadingData.resetMajors();
    if (nextUniversity) cascadingData.fetchFacultiesData(nextUniversity.id);
  };

  const handleCollegeChange = (value: string) => {
    const nextCollege =
      cascadingData.facultiesState.items.find(
        (faculty) => faculty.id.toString() === value
      ) ?? null;
    filterState.setSelectedCollege(nextCollege);
    filterState.setSelectedMajor(null);
    cascadingData.resetMajors();
    if (nextCollege) cascadingData.fetchMajorsData(nextCollege.id);
  };

  const handleMajorChange = (value: string) => {
    const nextMajor =
      cascadingData.majorsState.items.find(
        (major) => major.id.toString() === value
      ) ?? null;
    filterState.setSelectedMajor(nextMajor);
  };

  const handleUniversityMenuOpen = (open?: boolean) => {
    if (
      open &&
      !cascadingData.universitiesState.items.length &&
      !cascadingData.universitiesState.loading
    ) {
      cascadingData.fetchUniversitiesData();
    }
  };

  const handleLoadMoreUniversities = () => {
    if (
      !cascadingData.universitiesState.loading &&
      cascadingData.universitiesState.hasNextPage
    ) {
      cascadingData.fetchUniversitiesData(
        cascadingData.universitiesState.pageNumber + 1,
        true
      );
    }
  };

  const handleLoadMoreFaculties = () => {
    if (
      filterState.selectedUniversity &&
      !cascadingData.facultiesState.loading &&
      cascadingData.facultiesState.hasNextPage
    ) {
      cascadingData.fetchFacultiesData(
        filterState.selectedUniversity.id,
        cascadingData.facultiesState.pageNumber + 1,
        true
      );
    }
  };

  const handleLoadMoreMajors = () => {
    if (
      filterState.selectedCollege &&
      !cascadingData.majorsState.loading &&
      cascadingData.majorsState.hasNextPage
    ) {
      cascadingData.fetchMajorsData(
        filterState.selectedCollege.id,
        cascadingData.majorsState.pageNumber + 1,
        true
      );
    }
  };

  const sharedProps = {
    universitiesState: cascadingData.universitiesState,
    facultiesState: cascadingData.facultiesState,
    majorsState: cascadingData.majorsState,
    selectedUniversity: filterState.selectedUniversity,
    selectedCollege: filterState.selectedCollege,
    selectedMajor: filterState.selectedMajor,
    selectedCourseType: filterState.selectedCourseType,
    selectedRequirement: filterState.selectedRequirement,
    activeFiltersCount: filterState.activeFiltersCount,
    canApplyFilters: filterState.hasAcademicSelection,
    hasUnappliedChanges,
    onUniversityChange: handleUniversityChange,
    onCollegeChange: handleCollegeChange,
    onMajorChange: handleMajorChange,
    onCourseTypeChange: filterState.setSelectedCourseType,
    onRequirementChange: filterState.setSelectedRequirement,
    onUniversityMenuOpen: handleUniversityMenuOpen,
    onLoadMoreUniversities: handleLoadMoreUniversities,
    onLoadMoreFaculties: handleLoadMoreFaculties,
    onLoadMoreMajors: handleLoadMoreMajors,
    onApply: handleApplyFilters,
    onClear: handleClearFilters,
  };

  return (
    <div className="w-full border-b bg-background">
      <div className="container mx-auto px-4 py-3 sm:px-6 lg:px-8">
        <FilterMobile {...sharedProps} />
        <FilterDesktop {...sharedProps} />
      </div>
    </div>
  );
};

export default CourseFilter;
