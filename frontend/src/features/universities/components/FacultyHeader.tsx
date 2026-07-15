import { ArrowLeft, School, PlusIcon } from "lucide-react";
import { Button } from "@/shared/ui/button";

interface FacultyHeaderProps {
  facultyName?: string;
  onBack: () => void;
  onAddMajor: () => void;
  showAddButton?: boolean;
  showBackButton?: boolean;
}

export const FacultyHeader = ({
  facultyName,
  onBack,
  onAddMajor,
  showAddButton = true,
  showBackButton = true,
}: FacultyHeaderProps) => {
  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="space-y-1">
        <div className="flex items-center gap-3">
          {showBackButton && (
            <Button variant="ghost" size="icon" onClick={onBack}>
              <ArrowLeft className="size-5" />
            </Button>
          )}
          <div className="flex size-12 items-center justify-center rounded-lg bg-primary/10">
            <School className="size-6 text-primary" />
          </div>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">
              {facultyName || "Faculty Details"}
            </h1>
            <p className="text-muted-foreground text-sm">
              Manage majors in this faculty
            </p>
          </div>
        </div>
      </div>
      {showAddButton && (
        <Button onClick={onAddMajor} size="lg">
          <PlusIcon className="mr-2 size-4" />
          Add Major
        </Button>
      )}
    </div>
  );
};
