import { Filter, X, CheckCircle2, XCircle } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Label } from "@/shared/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { FilterSelect } from "./FilterSelect";
import { FilterBadges } from "./FilterBadges";
import type { PaginatedState } from "../hooks/usePaginatedSelect";
import type { University, Faculty, Major } from "../types";

type DesktopFiltersProps = {
  activeFiltersCount: number;
  selectedUniversity: University | null;
  selectedFaculty: Faculty | null;
  selectedMajor: Major | null;
  selectedStatus: string;
  selectedVerification: string;
  universitiesState: PaginatedState<University>;
  facultiesState: PaginatedState<Faculty>;
  majorsState: PaginatedState<Major>;
  onUniversityChange: (value: string) => void;
  onFacultyChange: (value: string) => void;
  onMajorChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onVerificationChange: (value: string) => void;
  onUniversityMenuOpen: (open?: boolean) => void;
  onLoadMoreUniversities: () => void;
  onLoadMoreFaculties: () => void;
  onLoadMoreMajors: () => void;
  onApplyFilters: () => void;
  onClearFilters: () => void;
};

export const DesktopFilters = ({
  activeFiltersCount,
  selectedUniversity,
  selectedFaculty,
  selectedMajor,
  selectedStatus,
  selectedVerification,
  universitiesState,
  facultiesState,
  majorsState,
  onUniversityChange,
  onFacultyChange,
  onMajorChange,
  onStatusChange,
  onVerificationChange,
  onUniversityMenuOpen,
  onLoadMoreUniversities,
  onLoadMoreFaculties,
  onLoadMoreMajors,
  onApplyFilters,
  onClearFilters,
}: DesktopFiltersProps) => {
  return (
    <div className="hidden lg:block">
      <div className="border-2 rounded-lg bg-card">
        <div className="p-4">
          <div className="grid gap-3 md:grid-cols-3 lg:grid-cols-5">
            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">University</Label>
              <FilterSelect
                value={selectedUniversity?.id.toString() ?? ""}
                onChange={onUniversityChange}
                onOpenChange={onUniversityMenuOpen}
                state={universitiesState}
                onLoadMore={onLoadMoreUniversities}
                placeholder="All universities"
                className="h-9 border-2 hover:border-primary/50 transition-colors text-sm"
              />
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Faculty</Label>
              <FilterSelect
                value={selectedFaculty?.id.toString() ?? ""}
                onChange={onFacultyChange}
                state={facultiesState}
                onLoadMore={onLoadMoreFaculties}
                placeholder="All faculties"
                disabled={!selectedUniversity}
                disabledPlaceholder="Select university first"
                className={`h-9 border-2 transition-colors text-sm ${
                  !selectedUniversity
                    ? "opacity-50 cursor-not-allowed"
                    : "hover:border-primary/50"
                }`}
              />
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Major</Label>
              <FilterSelect
                value={selectedMajor?.id.toString() ?? ""}
                onChange={onMajorChange}
                state={majorsState}
                onLoadMore={onLoadMoreMajors}
                placeholder="All majors"
                disabled={!selectedFaculty}
                disabledPlaceholder="Select faculty first"
                className={`h-9 border-2 transition-colors text-sm ${
                  !selectedFaculty
                    ? "opacity-50 cursor-not-allowed"
                    : "hover:border-primary/50"
                }`}
              />
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Status</Label>
              <Select value={selectedStatus} onValueChange={onStatusChange}>
                <SelectTrigger className="h-9 border-2 hover:border-primary/50 transition-colors text-sm">
                  <SelectValue placeholder="All statuses" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="active">
                    <div className="flex items-center gap-2">
                      <CheckCircle2 className="h-3.5 w-3.5 text-green-600" />
                      Active
                    </div>
                  </SelectItem>
                  <SelectItem value="blocked">
                    <div className="flex items-center gap-2">
                      <XCircle className="h-3.5 w-3.5 text-destructive" />
                      Blocked
                    </div>
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-1.5">
              <Label className="text-xs font-semibold">Verification</Label>
              <Select
                value={selectedVerification}
                onValueChange={onVerificationChange}
              >
                <SelectTrigger className="h-9 border-2 hover:border-primary/50 transition-colors text-sm">
                  <SelectValue placeholder="All users" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="verified">
                    <div className="flex items-center gap-2">
                      <CheckCircle2 className="h-3.5 w-3.5 text-green-600" />
                      Verified
                    </div>
                  </SelectItem>
                  <SelectItem value="unverified">
                    <div className="flex items-center gap-2">
                      <XCircle className="h-3.5 w-3.5 text-muted-foreground" />
                      Unverified
                    </div>
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        <div className="px-4 py-2.5 bg-muted/30 border-t flex items-center justify-between gap-3">
          <div className="text-xs text-muted-foreground flex items-center gap-2">
            {activeFiltersCount > 0 ? (
              <>
                <span>
                  {activeFiltersCount} filter
                  {activeFiltersCount > 1 ? "s" : ""} selected
                </span>
                <FilterBadges
                  selectedUniversity={selectedUniversity}
                  selectedFaculty={selectedFaculty}
                  selectedMajor={selectedMajor}
                  selectedStatus={selectedStatus}
                  selectedVerification={selectedVerification}
                />
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
              onClick={onClearFilters}
              className="gap-1.5 h-8 px-3 text-xs hover:bg-destructive/10 hover:text-destructive hover:border-destructive/50"
            >
              <X className="h-3.5 w-3.5" />
              Reset
            </Button>
            <Button
              size="sm"
              className="gap-1.5 h-8 px-4 text-xs min-w-[120px]"
              onClick={onApplyFilters}
            >
              <Filter className="h-3.5 w-3.5" />
              Apply Filters
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
