import type { UserFilters } from "../types";
import { useUserFilters } from "../hooks/useUserFilters";
import { MobileFilters } from "./MobileFilters";
import { DesktopFilters } from "./DesktopFilters";

// Re-export for backward compatibility
export type UserFilterValues = UserFilters;

type UserFilterProps = {
  onApply?: (filters: UserFilterValues) => void;
  onReset?: (filters: UserFilterValues) => void;
};

const UserFilter = ({ onApply, onReset }: UserFilterProps) => {
  const {
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    selectedStatus,
    selectedVerification,
    setSelectedStatus,
    setSelectedVerification,
    universitiesState,
    facultiesState,
    majorsState,
    currentFilterValues,
    activeFiltersCount,
    handleUniversityChange,
    handleFacultyChange,
    handleMajorChange,
    handleClearFilters,
    handleUniversityMenuOpen,
    handleLoadMoreUniversities,
    handleLoadMoreFaculties,
    handleLoadMoreMajors,
  } = useUserFilters();

  const handleApplyFilters = () => {
    onApply?.(currentFilterValues);
  };

  const handleResetFilters = () => {
    handleClearFilters();
    onReset?.({});
  };

  const commonProps = {
    activeFiltersCount,
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    selectedStatus,
    selectedVerification,
    universitiesState,
    facultiesState,
    majorsState,
    onUniversityChange: handleUniversityChange,
    onFacultyChange: handleFacultyChange,
    onMajorChange: handleMajorChange,
    onStatusChange: setSelectedStatus,
    onVerificationChange: setSelectedVerification,
    onUniversityMenuOpen: handleUniversityMenuOpen,
    onLoadMoreUniversities: handleLoadMoreUniversities,
    onLoadMoreFaculties: handleLoadMoreFaculties,
    onLoadMoreMajors: handleLoadMoreMajors,
    onApplyFilters: handleApplyFilters,
    onClearFilters: handleResetFilters,
  };

  return (
    <div className="w-full">
      <MobileFilters {...commonProps} />
      <DesktopFilters {...commonProps} />
    </div>
  );
};

export default UserFilter;
export { UserFilter };
