import { Filter, X } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
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
import { Label } from "@/shared/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { FilterSelect } from "./FilterSelect";
import type { PaginatedState } from "../hooks/usePaginatedSelect";
import type { University, Faculty, Major } from "../types";

type MobileFiltersProps = {
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

export const MobileFilters = ({
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
}: MobileFiltersProps) => {
  return (
    <div className="lg:hidden">
      <div className="flex items-center justify-between gap-3">
        <div>
          <h3 className="font-bold text-base">Users</h3>
          <p className="text-xs text-muted-foreground">
            {activeFiltersCount > 0
              ? `${activeFiltersCount} filter${
                  activeFiltersCount > 1 ? "s" : ""
                } active`
              : "All users"}
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
                Filter Users
              </DrawerTitle>
              <DrawerDescription className="text-sm">
                Filter users by academic info and status
              </DrawerDescription>
            </DrawerHeader>
            <div className="px-4 pb-4 space-y-5 max-h-[60vh] overflow-y-auto pt-4">
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">University</Label>
                <FilterSelect
                  value={selectedUniversity?.id.toString() ?? ""}
                  onChange={onUniversityChange}
                  onOpenChange={onUniversityMenuOpen}
                  state={universitiesState}
                  onLoadMore={onLoadMoreUniversities}
                  placeholder="All universities"
                  className="h-11 border-2"
                />
              </div>
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Faculty</Label>
                <FilterSelect
                  value={selectedFaculty?.id.toString() ?? ""}
                  onChange={onFacultyChange}
                  state={facultiesState}
                  onLoadMore={onLoadMoreFaculties}
                  placeholder="All faculties"
                  disabled={!selectedUniversity}
                  disabledPlaceholder="Select university first"
                  className={`h-11 border-2 ${
                    !selectedUniversity ? "opacity-50 cursor-not-allowed" : ""
                  }`}
                />
              </div>
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Major</Label>
                <FilterSelect
                  value={selectedMajor?.id.toString() ?? ""}
                  onChange={onMajorChange}
                  state={majorsState}
                  onLoadMore={onLoadMoreMajors}
                  placeholder="All majors"
                  disabled={!selectedFaculty}
                  disabledPlaceholder="Select faculty first"
                  className={`h-11 border-2 ${
                    !selectedFaculty ? "opacity-50 cursor-not-allowed" : ""
                  }`}
                />
              </div>
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Status</Label>
                <Select value={selectedStatus} onValueChange={onStatusChange}>
                  <SelectTrigger className="h-11 border-2">
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="active">Active</SelectItem>
                    <SelectItem value="blocked">Blocked</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2.5">
                <Label className="text-sm font-semibold">Verification</Label>
                <Select
                  value={selectedVerification}
                  onValueChange={onVerificationChange}
                >
                  <SelectTrigger className="h-11 border-2">
                    <SelectValue placeholder="All users" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="verified">Verified</SelectItem>
                    <SelectItem value="unverified">Unverified</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <DrawerFooter className="border-t pt-4 pb-6 px-4">
              <div className="flex flex-col gap-2 w-full">
                <Button className="w-full gap-2 h-11" onClick={onApplyFilters}>
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
                      onClick={onClearFilters}
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
