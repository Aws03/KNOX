import { useCallback, useMemo, useState } from "react";
import type { UserFilters, University, Faculty, Major } from "../types";
import { usePaginatedSelect } from "./usePaginatedSelect";

export const useUserFilters = () => {
  const [selectedUniversity, setSelectedUniversity] =
    useState<University | null>(null);
  const [selectedFaculty, setSelectedFaculty] = useState<Faculty | null>(null);
  const [selectedMajor, setSelectedMajor] = useState<Major | null>(null);
  const [selectedStatus, setSelectedStatus] = useState<string>("");
  const [selectedVerification, setSelectedVerification] = useState<string>("");

  const universities = usePaginatedSelect<University>();
  const faculties = usePaginatedSelect<Faculty>();
  const majors = usePaginatedSelect<Major>();

  const currentFilterValues = useMemo<UserFilters>(() => {
    const filters: UserFilters = {};

    if (selectedUniversity) filters.universityId = selectedUniversity.id;
    if (selectedFaculty) filters.facultyId = selectedFaculty.id;
    if (selectedMajor) filters.majorId = selectedMajor.id;
    if (selectedStatus === "active") filters.isActive = true;
    if (selectedStatus === "blocked") filters.isActive = false;
    if (selectedVerification === "verified") filters.isVerified = true;
    if (selectedVerification === "unverified") filters.isVerified = false;

    return filters;
  }, [
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    selectedStatus,
    selectedVerification,
  ]);

  const activeFiltersCount = useMemo(
    () =>
      [
        selectedUniversity,
        selectedFaculty,
        selectedMajor,
        selectedStatus,
        selectedVerification,
      ].filter(Boolean).length,
    [
      selectedUniversity,
      selectedFaculty,
      selectedMajor,
      selectedStatus,
      selectedVerification,
    ]
  );

  const handleUniversityChange = useCallback(
    (value: string) => {
      const nextUniversity =
        universities.state.items.find((uni) => uni.id.toString() === value) ??
        null;
      setSelectedUniversity(nextUniversity);
      setSelectedFaculty(null);
      setSelectedMajor(null);
      faculties.reset();
      majors.reset();

      if (nextUniversity) {
        faculties.fetchData(
          `/faculties/by-university/${nextUniversity.id}`,
          1,
          false,
          nextUniversity.id
        );
      }
    },
    [universities.state.items, faculties, majors]
  );

  const handleFacultyChange = useCallback(
    (value: string) => {
      const nextFaculty =
        faculties.state.items.find(
          (faculty) => faculty.id.toString() === value
        ) ?? null;
      setSelectedFaculty(nextFaculty);
      setSelectedMajor(null);
      majors.reset();

      if (nextFaculty) {
        majors.fetchData(
          `/majors/by-faculty/${nextFaculty.id}`,
          1,
          false,
          nextFaculty.id
        );
      }
    },
    [faculties.state.items, majors]
  );

  const handleMajorChange = useCallback(
    (value: string) => {
      const nextMajor =
        majors.state.items.find((major) => major.id.toString() === value) ??
        null;
      setSelectedMajor(nextMajor);
    },
    [majors.state.items]
  );

  const handleClearFilters = useCallback(() => {
    setSelectedUniversity(null);
    setSelectedFaculty(null);
    setSelectedMajor(null);
    setSelectedStatus("");
    setSelectedVerification("");
    faculties.reset();
    majors.reset();
  }, [faculties, majors]);

  const handleUniversityMenuOpen = useCallback(
    (open?: boolean) => {
      if (!open) return;
      if (!universities.state.items.length && !universities.state.loading) {
        universities.fetchData("/universities");
      }
    },
    [universities]
  );

  const handleLoadMoreUniversities = useCallback(() => {
    universities.loadMore("/universities");
  }, [universities]);

  const handleLoadMoreFaculties = useCallback(() => {
    if (!selectedUniversity) return;
    faculties.loadMore(
      `/faculties/by-university/${selectedUniversity.id}`,
      selectedUniversity.id
    );
  }, [selectedUniversity, faculties]);

  const handleLoadMoreMajors = useCallback(() => {
    if (!selectedFaculty) return;
    majors.loadMore(
      `/majors/by-faculty/${selectedFaculty.id}`,
      selectedFaculty.id
    );
  }, [selectedFaculty, majors]);

  return {
    // States
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    selectedStatus,
    selectedVerification,
    setSelectedStatus,
    setSelectedVerification,

    // Paginated data
    universitiesState: universities.state,
    facultiesState: faculties.state,
    majorsState: majors.state,

    // Computed values
    currentFilterValues,
    activeFiltersCount,

    // Handlers
    handleUniversityChange,
    handleFacultyChange,
    handleMajorChange,
    handleClearFilters,
    handleUniversityMenuOpen,
    handleLoadMoreUniversities,
    handleLoadMoreFaculties,
    handleLoadMoreMajors,
  };
};
