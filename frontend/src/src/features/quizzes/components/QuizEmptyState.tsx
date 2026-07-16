import { Trophy } from "lucide-react";
import { Card } from "@/shared/ui/card";

export const QuizEmptyState = () => {
  return (
    <Card className="p-12 bg-linear-to-br from-card to-muted/20">
      <div className="flex flex-col items-center justify-center text-center">
        <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
          <Trophy className="h-10 w-10 text-primary" />
        </div>
        <h3 className="text-lg font-semibold mb-2">No quizzes found</h3>
        <p className="text-sm text-muted-foreground max-w-sm">
          This course doesn't have any quizzes yet. Check back later!
        </p>
      </div>
    </Card>
  );
};
