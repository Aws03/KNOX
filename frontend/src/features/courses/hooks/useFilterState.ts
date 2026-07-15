import { useState, useMemo, useEffect } from "react";
import type { CourseFilterValues, University, Faculty, Major } from "../types";

const FILTER_STORAGE_KEY = "course_filter_state";

interface StoredFilterState {
  university: University | null;
  college: Faculty | null;
  major: Major | null;
  courseType: string;
  requirement: string;
  timestamp: number;
}

const loadFilterState = (): StoredFilterState | null => {
  try {
    const stored = localStorage.getItem(FILTER_STORAGE_KEY);
    if (!stored) return null;
    const parsed = JSON.parse(stored) as StoredFilterState;
    // Check if stored data is less than 1 hour old
    const oneHour = 60 * 60 * 1000;
    if (Date.now() - parsed.timestamp > oneHour) {
      localStorage.removeItem(FILTER_STORAGE_KEY);
      return null;
    }
    return parsed;
  } catch {
    return null;
  }
};

const saveFilterState = (state: Omit<StoredFilterState, "timestamp">) => {
  try {
    const toStore: StoredFilterState = {
      ...state,
      timestamp: Date.now(),
    };
    localStorage.setItem(FILTER_STORAGE_KEY, JSON.stringify(toStore));
  } catch (error) {
    console.error("Failed to save filter state:", error);
  }
};

export const useFilterState = () => {
  const storedState = loadFilterState();

  const [selectedUniversity, setSelectedUniversity] =
    useState<University | null>(storedState?.university ?? null);
  const [selectedCollege, setSelectedCollege] = useState<Faculty | null>(
    storedState?.college ?? null
  );
  const [selectedMajor, setSelectedMajor] = useState<Major | null>(
    storedState?.major ?? null
  );
  const [selectedCourseType, setSelectedCourseType] = useState(
    storedState?.courseType ?? ""
  );
  const [selectedRequirement, setSelectedRequirement] = useState(
    storedState?.requirement ?? ""
  );
  const [isRestoredFromStorage, setIsRestoredFromStorage] = useState(
    !!storedState
  );

  // Save filter state to localStorage whenever it changes
  useEffect(() => {
    // Only save if all academic selections are complete
    if (selectedUniversity && selectedCollege && selectedMajor) {
      saveFilterState({
        university: selectedUniversity,
        college: selectedCollege,
        major: selectedMajor,
        courseType: selectedCourseType,
        requirement: selectedRequirement,
      });
    } else {
      // Clear storage if any academic selection is null
      localStorage.removeItem(FILTER_STORAGE_KEY);
    }
  }, [
    selectedUniversity,
    selectedCollege,
    selectedMajor,
    selectedCourseType,
    selectedRequirement,
  ]);

  const currentFilterValues = useMemo<CourseFilterValues>(
    () => ({
      universityId: selectedUniversity?.id,
      collegeId: selectedCollege?.id,
      majorId: selectedMajor?.id,
      majorName: selectedMajor?.name,
      courseType: selectedCourseType || undefined,
      requirement: selectedRequirement || undefined,
    }),
    [
      selectedUniversity,
      selectedCollege,
      selectedMajor,
      selectedCourseType,
      selectedRequirement,
    ]
  );

  const hasAcademicSelection = Boolean(
    selectedUniversity && selectedCollege && selectedMajor
  );

  const activeFiltersCount = [
    selectedUniversity,
    selectedCollege,
    selectedMajor,
    selectedCourseType,
    selectedRequirement,
  ].filter(Boolean).length;

  const clearFilters = (keepAcademic = false) => {
    if (keepAcademic) {
      setSelectedCourseType("");
      setSelectedRequirement("");
    } else {
      setSelectedUniversity(null);
      setSelectedCollege(null);
      setSelectedMajor(null);
      setSelectedCourseType("");
      setSelectedRequirement("");
      localStorage.removeItem(FILTER_STORAGE_KEY);
    }
  };

  return {
    selectedUniversity,
    setSelectedUniversity,
    selectedCollege,
    setSelectedCollege,
    selectedMajor,
    setSelectedMajor,
    selectedCourseType,
    setSelectedCourseType,
    selectedRequirement,
    setSelectedRequirement,
    currentFilterValues,
    hasAcademicSelection,
    activeFiltersCount,
    clearFilters,
    isRestoredFromStorage,
    setIsRestoredFromStorage,
  };
};
