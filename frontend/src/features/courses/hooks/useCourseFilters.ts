import { useState } from "react";

export const useCourseFilters = () => {
  const [showFilters, setShowFilters] = useState(false);
  const [localRequirementType, setLocalRequirementType] = useState<string>("");
  const [localRequirementNature, setLocalRequirementNature] =
    useState<string>("");
  const [requirementType, setRequirementType] = useState<string>("");
  const [requirementNature, setRequirementNature] = useState<string>("");

  const hasActiveFilters = Boolean(requirementType || requirementNature);
  const hasUnappliedChanges =
    localRequirementType !== requirementType ||
    localRequirementNature !== requirementNature;

  const handleApplyFilters = () => {
    setRequirementType(localRequirementType);
    setRequirementNature(localRequirementNature);
  };

  const handleClearFilters = () => {
    setLocalRequirementType("");
    setLocalRequirementNature("");
    setRequirementType("");
    setRequirementNature("");
  };

  const handleClearRequirementTypeFilter = () => {
    setLocalRequirementType("");
    setRequirementType("");
  };

  const handleClearRequirementNatureFilter = () => {
    setLocalRequirementNature("");
    setRequirementNature("");
  };

  return {
    showFilters,
    setShowFilters,
    localRequirementType,
    setLocalRequirementType,
    localRequirementNature,
    setLocalRequirementNature,
    requirementType,
    requirementNature,
    hasActiveFilters,
    hasUnappliedChanges,
    handleApplyFilters,
    handleClearFilters,
    handleClearRequirementTypeFilter,
    handleClearRequirementNatureFilter,
  };
};
