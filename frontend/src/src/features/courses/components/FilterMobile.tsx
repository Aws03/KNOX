import { Filter, X } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import { Label } from "@/shared/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerDescription,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
} from "@/shared/ui/drawer";
import { renderSelectContent } from "./FilterSelectContent";
import type { PaginatedState, University, Faculty, Major } from "../types";
import { COURSE_TYPES, REQUIREMENT_TYPES } from "../types";

type FilterMobileProps = {
  universitiesState: PaginatedState<University>;
  facultiesState: PaginatedState<Faculty>;
  majorsState: PaginatedState<Major>;
  selectedUniversity: University | null;
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

export const FilterMobile = ({
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
}: FilterMobileProps) => {
  return (
    <div className="lg:hidden">
      <div className="flex items-center justify-between gap-3">
        <div>
          <h3 className="font-bold text-base">Courses</h3>
          <p className="text-xs text-muted-foreground">
            {activeFiltersCount > 0
              ? `${activeFiltersCount} filter${
                  activeFiltersCount > 1 ? "s" : ""
                } active`
              : "All courses"}
          </p>
        </div>
        <Drawer>
          <DrawerTrigger asChild>
            <Button
              variant="outline"
              className="relative gap-2 h-9 px-3 border-2 text-sm"
            >
              <Filter className="h-3.5 w-3.5" />
              Filter
              {activeFiltersCount > 0 && (
                <Badge className="ml-1 h-4 min-w-4 rounded-full px-1 text-[10px] font-semibold">
                  {activeFiltersCount}
                </Badge>
              )}
            </Button>
          </DrawerTrigger>
          <DrawerContent>
            <DrawerHeader className="border-b pb-4">
              <DrawerTitle className="text-xl font-bold">
                Filter Courses
              </DrawerTitle>
              <DrawerDescription className="text-sm">
                Choose your preferences to find the right courses
              </DrawerDescription>
            </DrawerHeader>
            <div className="px-4 pb-4 space-y-5 max-h-[60vh] overflow-y-auto pt-4">
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">University</Label>
                <Select
                  value={selectedUniversity?.id.toString() ?? ""}
                  onValueChange={onUniversityChange}
                  onOpenChange={onUniversityMenuOpen}
                >
                  <SelectTrigger className="h-11 border-2">
                    <SelectValue placeholder="Choose university" />
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
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">College</Label>
                <Select
                  value={selectedCollege?.id.toString() ?? ""}
                  onValueChange={onCollegeChange}
                  disabled={!selectedUniversity}
                >
                  <SelectTrigger
                    className={`h-11 border-2 ${
                      !selectedUniversity ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                  >
                    <SelectValue
                      placeholder={
                        !selectedUniversity
                          ? "Select university first"
                          : "Choose college"
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
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Major</Label>
                <Select
                  value={selectedMajor?.id.toString() ?? ""}
                  onValueChange={onMajorChange}
                  disabled={!selectedCollege}
                >
                  <SelectTrigger
                    className={`h-11 border-2 ${
                      !selectedCollege ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                  >
                    <SelectValue
                      placeholder={
                        !selectedCollege
                          ? "Select college first"
                          : "Choose major"
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
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Course Type</Label>
                <div className="flex items-center gap-2">
                  <Select
                    value={selectedCourseType}
                    onValueChange={onCourseTypeChange}
                  >
                    <SelectTrigger className="h-11 border-2">
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
                      className="h-5 w-5 cursor-pointer text-muted-foreground transition-colors hover:text-foreground shrink-0"
                      onClick={() => onCourseTypeChange("")}
                    />
                  )}
                </div>
              </div>
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Requirement</Label>
                <div className="flex items-center gap-2">
                  <Select
                    value={selectedRequirement}
                    onValueChange={onRequirementChange}
                  >
                    <SelectTrigger className="h-11 border-2">
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
                      className="h-5 w-5 cursor-pointer text-muted-foreground transition-colors hover:text-foreground shrink-0"
                      onClick={() => onRequirementChange("")}
                    />
                  )}
                </div>
              </div>
            </div>
            <DrawerFooter className="border-t pt-4 pb-6 px-4">
              {!canApplyFilters && (
                <div className="mb-2 text-sm text-muted-foreground text-center flex items-center justify-center gap-2">
                  <span className="w-2 h-2 rounded-full bg-orange-500 animate-pulse" />
                  Select university, college, and major
                </div>
              )}
              <div className="flex flex-col gap-2 w-full">
                <Button
                  className={`w-full gap-2 h-11 transition-all ${
                    hasUnappliedChanges
                      ? "animate-[pulse_2s_ease-in-out_infinite] shadow-[0_0_15px_rgba(var(--primary-rgb,59,130,246),0.5)] hover:shadow-[0_0_20px_rgba(var(--primary-rgb,59,130,246),0.7)]"
                      : ""
                  }`}
                  disabled={!canApplyFilters}
                  onClick={onApply}
                >
                  <Filter className="h-4 w-4" />
                  Apply Filters
                </Button>
                <div className="flex w-full gap-2">
                  <DrawerClose asChild>
                    <Button variant="outline" className="flex-1 h-11">
                      Cancel
                    </Button>
                  </DrawerClose>
                  {activeFiltersCount > 0 && (
                    <Button
                      variant="outline"
                      onClick={onClear}
                      className="flex-1 h-11 hover:bg-destructive/10 hover:text-destructive hover:border-destructive/50"
                    >
                      <X className="h-4 w-4 mr-2" />
                      Clear All
                    </Button>
                  )}
                </div>
              </div>
            </DrawerFooter>
          </DrawerContent>
        </Drawer>
      </div>
    </div>
  );
};
