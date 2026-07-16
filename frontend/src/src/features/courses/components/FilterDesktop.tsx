import {
  Filter,
  X,
  University,
  School,
  Book,
  Album,
  Check,
} from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import { Card } from "@/shared/ui/card";
import { Label } from "@/shared/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { renderSelectContent } from "./FilterSelectContent";
import type {
  PaginatedState,
  University as UniversityType,
  Faculty,
  Major,
} from "../types";
import { COURSE_TYPES, REQUIREMENT_TYPES } from "../types";

type FilterDesktopProps = {
  universitiesState: PaginatedState<UniversityType>;
  facultiesState: PaginatedState<Faculty>;
  majorsState: PaginatedState<Major>;
  selectedUniversity: UniversityType | null;
  selectedCollege: Faculty | null;
  selectedMajor: Major | null;
  selectedCourseType: string;
  selectedRequirement: string;
  activeFiltersCount: number;
  canApplyFilters: boolean;
  hasUnappliedChanges: boolean;
  onUniversityChange: (value: string) => void;
  onCollegeChange: (value: string) => void;
  onMajorChange: (value: string) => void;
  onCourseTypeChange: (value: string) => void;
  onRequirementChange: (value: string) => void;
  onUniversityMenuOpen: (open?: boolean) => void;
  onLoadMoreUniversities: () => void;
  onLoadMoreFaculties: () => void;
  onLoadMoreMajors: () => void;
  onApply: () => void;
  onClear: () => void;
};

export const FilterDesktop = ({
  universitiesState,
  facultiesState,
  majorsState,
  selectedUniversity,
  selectedCollege,
  selectedMajor,
  selectedCourseType,
  selectedRequirement,
  activeFiltersCount,
  canApplyFilters,
  hasUnappliedChanges,
  onUniversityChange,
  onCollegeChange,
  onMajorChange,
  onCourseTypeChange,
  onRequirementChange,
  onUniversityMenuOpen,
  onLoadMoreUniversities,
  onLoadMoreFaculties,
  onLoadMoreMajors,
  onApply,
  onClear,
}: FilterDesktopProps) => {
  return (
    <div className="hidden lg:block">
      <div className="flex items-center justify-between mb-3">
        <div className="flex-1">
          <h3 className="text-base font-bold tracking-tight">Filter Courses</h3>
          <p className="text-xs text-muted-foreground mt-0.5">
            Refine your search to find the perfect courses
          </p>
        </div>
        {activeFiltersCount > 0 && (
          <Badge variant="secondary" className="px-2 py-0.5 text-xs">
            {activeFiltersCount} active
          </Badge>
        )}
      </div>

      <Card className="border-2 shadow-sm bg-card">
        <div className="p-4">
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">University</Label>
              <Select
                value={selectedUniversity?.id.toString() ?? ""}
                onValueChange={onUniversityChange}
                onOpenChange={onUniversityMenuOpen}
              >
                <SelectTrigger className="h-9 border-2 hover:border-primary/50 transition-colors text-sm">
                  <SelectValue placeholder="Select university" />
                </SelectTrigger>
                <SelectContent>
                  {renderSelectContent({
                    state: universitiesState,
                    hasParent: true,
                    onLoadMore: onLoadMoreUniversities,
                    emptyMessage: "No universities found",
                  })}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">College</Label>
              <Select
                value={selectedCollege?.id.toString() ?? ""}
                onValueChange={onCollegeChange}
                disabled={!selectedUniversity}
              >
                <SelectTrigger
                  className={`h-9 border-2 transition-colors text-sm ${
                    !selectedUniversity
                      ? "opacity-50 cursor-not-allowed"
                      : "hover:border-primary/50"
                  }`}
                >
                  <SelectValue
                    placeholder={
                      !selectedUniversity
                        ? "Select university first"
                        : "Select college"
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {renderSelectContent({
                    state: facultiesState,
                    hasParent: !!selectedUniversity,
                    onLoadMore: onLoadMoreFaculties,
                    emptyMessage: "No faculties found",
                  })}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Major</Label>
              <Select
                value={selectedMajor?.id.toString() ?? ""}
                onValueChange={onMajorChange}
                disabled={!selectedCollege}
              >
                <SelectTrigger
                  className={`h-9 border-2 transition-colors text-sm ${
                    !selectedCollege
                      ? "opacity-50 cursor-not-allowed"
                      : "hover:border-primary/50"
                  }`}
                >
                  <SelectValue
                    placeholder={
                      !selectedCollege ? "Select college first" : "Select major"
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {renderSelectContent({
                    state: majorsState,
                    hasParent: !!selectedCollege,
                    onLoadMore: onLoadMoreMajors,
                    emptyMessage: "No majors found",
                  })}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Course Type</Label>
              <div className="flex items-center gap-2">
                <Select
                  value={selectedCourseType}
                  onValueChange={onCourseTypeChange}
                >
                  <SelectTrigger className="h-9 border-2 hover:border-primary/50 transition-colors text-sm">
                    <SelectValue placeholder="All types" />
                  </SelectTrigger>
                  <SelectContent>
                    {COURSE_TYPES.map((type) => (
                      <SelectItem key={type} value={type}>
                        {type}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {selectedCourseType && (
                  <X
                    className="h-4 w-4 cursor-pointer text-muted-foreground transition-colors hover:text-foreground shrink-0"
                    onClick={() => onCourseTypeChange("")}
                  />
                )}
              </div>
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Requirement</Label>
              <div className="flex items-center gap-2">
                <Select
                  value={selectedRequirement}
                  onValueChange={onRequirementChange}
                >
                  <SelectTrigger className="h-9 border-2 hover:border-primary/50 transition-colors text-sm">
                    <SelectValue placeholder="All requirements" />
                  </SelectTrigger>
                  <SelectContent>
                    {REQUIREMENT_TYPES.map((req) => (
                      <SelectItem key={req} value={req}>
                        {req}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {selectedRequirement && (
                  <X
                    className="h-4 w-4 cursor-pointer text-muted-foreground transition-colors hover:text-foreground shrink-0"
                    onClick={() => onRequirementChange("")}
                  />
                )}
              </div>
            </div>
          </div>
        </div>

        <div className="px-4 py-2.5 bg-muted/30 border-t flex items-center justify-between gap-3">
          <div className="text-xs text-muted-foreground flex items-center gap-2">
            {!canApplyFilters ? (
              <span className="flex items-center gap-1.5">
                <span className="w-1.5 h-1.5 rounded-full bg-orange-500 animate-pulse" />
                Select university, college, and major
              </span>
            ) : activeFiltersCount > 0 ? (
              <>
                <span>
                  {activeFiltersCount} filter{activeFiltersCount > 1 ? "s" : ""}{" "}
                  selected
                </span>
                <div className="flex items-center gap-1">
                  {selectedUniversity && (
                    <Badge
                      variant="secondary"
                      className="h-5 px-1.5 text-[10px] bg-sky-50 dark:bg-sky-950/30 text-sky-600 dark:text-sky-400 border-sky-200/50 dark:border-sky-800/30"
                    >
                      <University className="h-2.5 w-2.5" />
                    </Badge>
                  )}
                  {selectedCollege && (
                    <Badge
                      variant="secondary"
                      className="h-5 px-1.5 text-[10px] bg-violet-50 dark:bg-violet-950/30 text-violet-600 dark:text-violet-400 border-violet-200/50 dark:border-violet-800/30"
                    >
                      <School className="h-2.5 w-2.5" />
                    </Badge>
                  )}
                  {selectedMajor && (
                    <Badge
                      variant="secondary"
                      className="h-5 px-1.5 text-[10px] bg-teal-50 dark:bg-teal-950/30 text-teal-600 dark:text-teal-400 border-teal-200/50 dark:border-teal-800/30"
                    >
                      <Book className="h-2.5 w-2.5" />
                    </Badge>
                  )}
                  {selectedCourseType && (
                    <Badge
                      variant="secondary"
                      className="h-5 px-1.5 text-[10px] bg-orange-50 dark:bg-orange-950/30 text-orange-600 dark:text-orange-400 border-orange-200/50 dark:border-orange-800/30"
                    >
                      <Album className="h-2.5 w-2.5" />
                    </Badge>
                  )}
                  {selectedRequirement && (
                    <Badge
                      variant="secondary"
                      className="h-5 px-1.5 text-[10px] bg-pink-50 dark:bg-pink-950/30 text-pink-600 dark:text-pink-400 border-pink-200/50 dark:border-pink-800/30"
                    >
                      <Check className="h-2.5 w-2.5" />
                    </Badge>
                  )}
                </div>
              </>
            ) : (
              "No filters applied"
            )}
          </div>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!activeFiltersCount}
              onClick={onClear}
              className="gap-1.5 h-8 px-3 text-xs hover:bg-destructive/10 hover:text-destructive hover:border-destructive/50"
            >
              <X className="h-3.5 w-3.5" />
              Reset
            </Button>
            <Button
              size="sm"
              className={`gap-1.5 h-8 px-4 text-xs min-w-[120px] transition-all ${
                hasUnappliedChanges
                  ? "animate-[pulse_2s_ease-in-out_infinite] shadow-[0_0_15px_rgba(var(--primary-rgb,59,130,246),0.5)] hover:shadow-[0_0_20px_rgba(var(--primary-rgb,59,130,246),0.7)]"
                  : ""
              }`}
              disabled={!canApplyFilters}
              onClick={onApply}
            >
              <Filter className="h-3.5 w-3.5" />
              Apply Filters
            </Button>
          </div>
        </div>
      </Card>
    </div>
  );
};
