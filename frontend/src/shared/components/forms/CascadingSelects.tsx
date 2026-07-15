import { useCallback } from "react";
import { PaginatedSelect, type PaginatedItem } from "./PaginatedSelect";

type University = PaginatedItem;
type Faculty = PaginatedItem & { universityId: number };
type Major = PaginatedItem & { facultyId: number };

type CascadingSelectsProps = {
  selectedUniversity: University | null;
  selectedFaculty: Faculty | null;
  selectedMajor: Major | null;
  onUniversityChange: (university: University | null) => void;
  onFacultyChange: (faculty: Faculty | null) => void;
  onMajorChange: (major: Major | null) => void;
  fetchUniversities: (pageNumber: number) => Promise<{
    items: University[];
    hasNextPage: boolean;
  }>;
  fetchFaculties: (
    universityId: number,
    pageNumber: number
  ) => Promise<{
    items: Faculty[];
    hasNextPage: boolean;
  }>;
  fetchMajors: (
    facultyId: number,
    pageNumber: number
  ) => Promise<{
    items: Major[];
    hasNextPage: boolean;
  }>;
  required?: boolean;
  errors?: {
    university?: string;
    faculty?: string;
    major?: string;
  };
};

export function CascadingSelects({
  selectedUniversity,
  selectedFaculty,
  selectedMajor,
  onUniversityChange,
  onFacultyChange,
  onMajorChange,
  fetchUniversities,
  fetchFaculties,
  fetchMajors,
  required = false,
  errors = {},
}: CascadingSelectsProps) {
  const handleUniversityChange = useCallback(
    (university: University | null) => {
      onUniversityChange(university);
      // Reset dependent selections
      onFacultyChange(null);
      onMajorChange(null);
    },
    [onUniversityChange, onFacultyChange, onMajorChange]
  );

  const handleFacultyChange = useCallback(
    (faculty: Faculty | null) => {
      onFacultyChange(faculty);
      // Reset dependent selection
      onMajorChange(null);
    },
    [onFacultyChange, onMajorChange]
  );

  const fetchFacultiesForUniversity = useCallback(
    async (pageNumber: number) => {
      if (!selectedUniversity) {
        return { items: [], hasNextPage: false };
      }
      return fetchFaculties(selectedUniversity.id, pageNumber);
    },
    [selectedUniversity, fetchFaculties]
  );

  const fetchMajorsForFaculty = useCallback(
    async (pageNumber: number) => {
      if (!selectedFaculty) {
        return { items: [], hasNextPage: false };
      }
      return fetchMajors(selectedFaculty.id, pageNumber);
    },
    [selectedFaculty, fetchMajors]
  );

  return (
    <div className="space-y-4">
      <PaginatedSelect
        label="University"
        placeholder="Select your university"
        value={selectedUniversity}
        onChange={handleUniversityChange}
        fetchPage={fetchUniversities}
        required={required}
        error={errors.university}
      />

      <PaginatedSelect
        label="Faculty"
        placeholder="Select your faculty"
        value={selectedFaculty}
        onChange={handleFacultyChange}
        fetchPage={fetchFacultiesForUniversity}
        disabled={!selectedUniversity}
        parentId={selectedUniversity?.id}
        required={required}
        error={errors.faculty}
      />

      <PaginatedSelect
        label="Major"
        placeholder="Select your major"
        value={selectedMajor}
        onChange={onMajorChange}
        fetchPage={fetchMajorsForFaculty}
        disabled={!selectedFaculty}
        parentId={selectedFaculty?.id}
        required={required}
        error={errors.major}
      />
    </div>
  );
}

export type { University, Faculty, Major };
