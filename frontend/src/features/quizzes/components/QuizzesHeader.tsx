import { ArrowLeft, Plus, Trophy } from "lucide-react";
import { Button } from "@/shared/ui/button";

interface QuizzesHeaderProps {
  isManagementMode: boolean;
  totalCount: number;
  onBack: () => void;
  onAddQuiz?: () => void;
}

export const QuizzesHeader = ({
  isManagementMode,
  totalCount,
  onBack,
  onAddQuiz,
}: QuizzesHeaderProps) => {
  return (
    <div className="mb-6">
      <Button variant="ghost" className="mb-4 gap-2" onClick={onBack}>
        <ArrowLeft className="h-4 w-4" />
        Back to Courses
      </Button>
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Course Quizzes</h1>
          <p className="text-muted-foreground mt-2">
            {isManagementMode
              ? "Manage quizzes for this course"
              : "Practice and test your knowledge"}
          </p>
        </div>
        {isManagementMode && onAddQuiz && (
          <Button onClick={onAddQuiz} size="default">
            <Plus className="mr-2 h-4 w-4" />
            Add Quiz
          </Button>
        )}
        <div className="flex items-center gap-2 rounded-xl border border-border/70 bg-background/60 px-4 py-3 shadow-sm">
          <Trophy className="h-5 w-5 text-primary" />
          <div>
            <p className="text-xs text-muted-foreground">Total Quizzes</p>
            <p className="text-xl font-semibold">{totalCount}</p>
          </div>
        </div>
      </div>
    </div>
  );
};
