import { Trophy } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card } from "@/shared/ui/card";

interface QuizErrorStateProps {
  error: string;
  onRetry: () => void;
}

export const QuizErrorState = ({ error, onRetry }: QuizErrorStateProps) => {
  return (
    <Card className="p-12 bg-linear-to-br from-destructive/5 to-destructive/10 border-destructive/20">
      <div className="flex flex-col items-center justify-center text-center">
        <div className="flex h-20 w-20 items-center justify-center rounded-full bg-destructive/10 mb-4">
          <Trophy className="h-10 w-10 text-destructive" />
        </div>
        <h3 className="text-lg font-semibold mb-2 text-destructive">
          Failed to load quizzes
        </h3>
        <p className="text-sm text-muted-foreground max-w-sm mb-4">{error}</p>
        <Button onClick={onRetry} variant="outline">
          Try Again
        </Button>
      </div>
    </Card>
  );
};
