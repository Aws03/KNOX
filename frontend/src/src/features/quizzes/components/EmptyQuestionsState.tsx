import { Plus } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card } from "@/shared/ui/card";

interface EmptyQuestionsStateProps {
  onAddQuestion: () => void;
}

export const EmptyQuestionsState = ({
  onAddQuestion,
}: EmptyQuestionsStateProps) => {
  return (
    <Card className="p-12 bg-linear-to-br from-card to-muted/20">
      <div className="flex flex-col items-center justify-center text-center">
        <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
          <Plus className="h-10 w-10 text-primary" />
        </div>
        <h3 className="text-lg font-semibold mb-2">No questions yet</h3>
        <p className="text-sm text-muted-foreground max-w-sm mb-4">
          Start building your quiz by adding your first question
        </p>
        <Button onClick={onAddQuestion}>
          <Plus className="h-4 w-4 mr-2" />
          Add First Question
        </Button>
      </div>
    </Card>
  );
};
