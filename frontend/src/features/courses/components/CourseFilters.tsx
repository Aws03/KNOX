import { Filter, XIcon } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import {
  REQUIREMENT_TYPE_MAP,
  REQUIREMENT_NATURE_MAP,
} from "@/features/courses/types";

interface CourseFiltersProps {
  showFilters: boolean;
  onToggleFilters: () => void;
  localRequirementType: string;
  localRequirementNature: string;
  onRequirementTypeChange: (value: string) => void;
  onRequirementNatureChange: (value: string) => void;
  onClearRequirementType: () => void;
  onClearRequirementNature: () => void;
  onApplyFilters: () => void;
  onClearAllFilters: () => void;
  hasActiveFilters: boolean;
  hasUnappliedChanges: boolean;
}

export const CourseFilters = ({
  showFilters,
  onToggleFilters,
  localRequirementType,
  localRequirementNature,
  onRequirementTypeChange,
  onRequirementNatureChange,
  onClearRequirementType,
  onClearRequirementNature,
  onApplyFilters,
  onClearAllFilters,
  hasActiveFilters,
  hasUnappliedChanges,
}: CourseFiltersProps) => {
  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Course Filters</CardTitle>
            <CardDescription>
              Filter courses by requirement type and nature
            </CardDescription>
          </div>
          <Button variant="ghost" size="sm" onClick={onToggleFilters}>
            <Filter className="mr-2 size-4" />
            {showFilters ? "Hide" : "Show"} Filters
          </Button>
        </div>
      </CardHeader>

      {showFilters && (
        <CardContent>
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            {/* Requirement Type Filter */}
            <div className="space-y-2">
              <label className="text-sm font-medium">Requirement Type</label>
              <div className="flex flex-row items-center gap-2">
                <Select
                  key={localRequirementType || "requirement-type-empty"}
                  value={localRequirementType || undefined}
                  onValueChange={onRequirementTypeChange}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All Types" />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.entries(REQUIREMENT_TYPE_MAP).map(
                      ([label, value]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      )
                    )}
                  </SelectContent>
                </Select>
                {localRequirementType && (
                  <XIcon
                    className="size-4 cursor-pointer text-muted-foreground transition-colors hover:text-foreground"
                    onClick={onClearRequirementType}
                  />
                )}
              </div>
            </div>

            {/* Requirement Nature Filter */}
            <div className="space-y-2">
              <label className="text-sm font-medium">Requirement Nature</label>
              <div className="flex flex-row items-center gap-2">
                <Select
                  key={localRequirementNature || "requirement-nature-empty"}
                  value={localRequirementNature || undefined}
                  onValueChange={onRequirementNatureChange}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="All Natures" />
                  </SelectTrigger>
                  <SelectContent>
                    {Object.entries(REQUIREMENT_NATURE_MAP).map(
                      ([label, value]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      )
                    )}
                  </SelectContent>
                </Select>
                {localRequirementNature && (
                  <XIcon
                    className="size-4 cursor-pointer text-muted-foreground transition-colors hover:text-foreground"
                    onClick={onClearRequirementNature}
                  />
                )}
              </div>
            </div>

            {/* Action Buttons */}
            <div className="flex gap-2 sm:mt-6">
              {hasActiveFilters && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onClearAllFilters}
                  className="flex-1 sm:flex-none"
                >
                  <XIcon className="mr-2 size-4" />
                  Clear Filters
                </Button>
              )}
              <Button
                variant="default"
                size="sm"
                onClick={onApplyFilters}
                disabled={
                  !hasUnappliedChanges &&
                  !localRequirementType &&
                  !localRequirementNature
                }
                className="flex-1 sm:flex-none"
              >
                <Filter className="mr-2 size-4" />
                Apply Filters
              </Button>
            </div>
          </div>
        </CardContent>
      )}
    </Card>
  );
};
