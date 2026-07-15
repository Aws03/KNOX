import {
  University as UniversityIcon,
  School,
  Book,
  CheckCircle2,
  XCircle,
} from "lucide-react";
import { Badge } from "@/shared/ui/badge";
import type { University, Faculty, Major } from "../types";

type FilterBadgesProps = {
  selectedUniversity: University | null;
  selectedFaculty: Faculty | null;
  selectedMajor: Major | null;
  selectedStatus: string;
  selectedVerification: string;
};

export const FilterBadges = ({
  selectedUniversity,
  selectedFaculty,
  selectedMajor,
  selectedStatus,
  selectedVerification,
}: FilterBadgesProps) => {
  return (
    <div className="flex items-center gap-1">
      {selectedUniversity && (
        <Badge
          variant="secondary"
          className="h-5 px-1.5 text-[10px] bg-sky-50 dark:bg-sky-950/30 text-sky-600 dark:text-sky-400 border-sky-200/50 dark:border-sky-800/30"
        >
          <UniversityIcon className="h-2.5 w-2.5" />
        </Badge>
      )}
      {selectedFaculty && (
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
      {selectedStatus && (
        <Badge
          variant="secondary"
          className="h-5 px-1.5 text-[10px] bg-orange-50 dark:bg-orange-950/30 text-orange-600 dark:text-orange-400 border-orange-200/50 dark:border-orange-800/30"
        >
          {selectedStatus === "active" ? (
            <CheckCircle2 className="h-2.5 w-2.5" />
          ) : (
            <XCircle className="h-2.5 w-2.5" />
          )}
        </Badge>
      )}
      {selectedVerification && (
        <Badge
          variant="secondary"
          className="h-5 px-1.5 text-[10px] bg-pink-50 dark:bg-pink-950/30 text-pink-600 dark:text-pink-400 border-pink-200/50 dark:border-pink-800/30"
        >
          <CheckCircle2 className="h-2.5 w-2.5" />
        </Badge>
      )}
    </div>
  );
};
