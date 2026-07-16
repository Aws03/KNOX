import { ArrowLeft, Building2, PlusIcon } from "lucide-react";
import { Button } from "@/shared/ui/button";

interface UniversityHeaderProps {
  universityName?: string;
  onBack: () => void;
  onAddFaculty: () => void;
}

export const UniversityHeader = ({
  universityName,
  onBack,
  onAddFaculty,
}: UniversityHeaderProps) => {
  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="space-y-1">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={onBack}>
            <ArrowLeft className="size-5" />
          </Button>
          <div className="flex size-12 items-center justify-center rounded-lg bg-primary/10">
            <Building2 className="size-6 text-primary" />
          </div>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">
              {universityName}
            </h1>
            <p className="text-muted-foreground text-sm">
              Manage faculties in this university
            </p>
          </div>
        </div>
      </div>
      <Button onClick={onAddFaculty} size="lg">
        <PlusIcon className="mr-2 size-4" />
        Add Faculty
      </Button>
    </div>
  );
};
