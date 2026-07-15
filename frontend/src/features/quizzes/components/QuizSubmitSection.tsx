import { Loader2, Save } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card, CardContent } from "@/shared/ui/card";

interface QuizSubmitSectionProps {
  questionsCount: number;
  isSubmitting: boolean;
  onCancel: () => void;
  onSubmit: () => void;
}

export const QuizSubmitSection = ({
  questionsCount,
  isSubmitting,
  onCancel,
  onSubmit,
}: QuizSubmitSectionProps) => {
  return (
    <Card className="sticky bottom-4 shadow-2xl border-2">
      <CardContent className="pt-6">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
          <div>
            <p className="font-semibold">Ready to create your quiz?</p>
            <p className="text-sm text-muted-foreground">
              {questionsCount} question{questionsCount !== 1 ? "s" : ""} added
            </p>
          </div>
          <div className="flex gap-2 w-full sm:w-auto">
            <Button
              variant="outline"
              onClick={onCancel}
              disabled={isSubmitting}
              className="flex-1 sm:flex-none"
            >
              Cancel
            </Button>
            <Button
              onClick={onSubmit}
              disabled={isSubmitting || questionsCount === 0}
              className="flex-1 sm:flex-none min-w-[150px]"
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Creating...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4 mr-2" />
                  Create Quiz
                </>
              )}
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};
