import { ArrowLeft } from "lucide-react";
import { Button } from "@/shared/ui/button";
import type { University, Faculty, Major } from "@/features/courses/types";

interface MajorCoursesHeaderProps {
  university: University | null;
  faculty: Faculty | null;
  major: Major | null;
  onBackClick: () => void;
}

export const MajorCoursesHeader = ({
  faculty,
  major,
  onBackClick,
}: MajorCoursesHeaderProps) => {
  return (
    <div className="space-y-4">
      <Button variant="ghost" className="gap-2" onClick={onBackClick}>
        <ArrowLeft className="size-4" />
        Back to {faculty?.name || "Faculty"}
      </Button>

      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-3xl font-bold">{major?.name} - Courses</h1>
          <p className="text-muted-foreground mt-1">
            Manage courses for this major
          </p>
        </div>
      </div>
    </div>
  );
};
