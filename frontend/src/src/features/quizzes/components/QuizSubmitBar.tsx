import { Loader2, Trophy, AlertCircle, ChevronUp } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card } from "@/shared/ui/card";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/shared/ui/dialog";

interface QuizSubmitBarProps {
  answeredCount: number;
  totalQuestions: number;
  canSubmit: boolean;
  isSubmitting: boolean;
  showConfirmDialog: boolean;
  onSubmitClick: () => void;
  onConfirmSubmit: () => void;
  onCancelSubmit: () => void;
  unansweredQuestionIds?: number[];
  onScrollToQuestion?: (questionId: number) => void;
}

export const QuizSubmitBar = ({
  answeredCount,
  totalQuestions,
  canSubmit,
  isSubmitting,
  showConfirmDialog,
  onSubmitClick,
  onConfirmSubmit,
  onCancelSubmit,
  unansweredQuestionIds = [],
  onScrollToQuestion,
}: QuizSubmitBarProps) => {
  const remainingQuestions = totalQuestions - answeredCount;
  const progressPercentage = (answeredCount / totalQuestions) * 100;

  const handleGoToUnanswered = () => {
    if (unansweredQuestionIds.length > 0 && onScrollToQuestion) {
      onScrollToQuestion(unansweredQuestionIds[0]);
    }
  };

  return (
    <>
      <Card className="mt-8 sticky bottom-4 shadow-2xl border-2 bg-background/95 backdrop-blur-sm overflow-hidden">
        {/* Progress bar at top */}
        <div className="h-1.5 bg-muted">
          <div
            className={`h-full transition-all duration-500 ${
              canSubmit
                ? "bg-linear-to-r from-green-500 to-emerald-500"
                : "bg-linear-to-r from-primary to-primary/70"
            }`}
            style={{ width: `${progressPercentage}%` }}
          />
        </div>

        <div className="p-6">
          <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
            <div className="text-center sm:text-left flex-1">
              <div className="flex items-center gap-3 mb-1">
                <p className="font-bold text-lg">
                  {answeredCount}{" "}
                  <span className="text-muted-foreground font-normal">of</span>{" "}
                  {totalQuestions}
                </p>
                {canSubmit ? (
                  <span className="inline-flex items-center gap-1.5 text-sm font-medium text-green-600 dark:text-green-400 bg-green-100 dark:bg-green-900/30 px-2.5 py-0.5 rounded-full">
                    <Trophy className="h-3.5 w-3.5" />
                    Ready!
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-1.5 text-sm font-medium text-orange-600 dark:text-orange-400 bg-orange-100 dark:bg-orange-900/30 px-2.5 py-0.5 rounded-full">
                    <AlertCircle className="h-3.5 w-3.5" />
                    {remainingQuestions} left
                  </span>
                )}
              </div>
              <p className="text-sm text-muted-foreground">
                {canSubmit
                  ? "All questions answered. You're ready to submit!"
                  : `Answer ${remainingQuestions} more question${
                      remainingQuestions > 1 ? "s" : ""
                    } to submit`}
              </p>
            </div>

            <div className="flex items-center gap-3 w-full sm:w-auto">
              {!canSubmit && unansweredQuestionIds.length > 0 && (
                <Button
                  variant="outline"
                  onClick={handleGoToUnanswered}
                  className="flex-1 sm:flex-none gap-2"
                >
                  <ChevronUp className="h-4 w-4" />
                  Go to Question
                </Button>
              )}
              <Button
                size="lg"
                onClick={canSubmit ? onSubmitClick : handleGoToUnanswered}
                disabled={isSubmitting}
                className={`flex-1 sm:flex-none min-w-[180px] transition-all ${
                  canSubmit
                    ? "bg-linear-to-r from-green-500 to-emerald-500 hover:from-green-600 hover:to-emerald-600 text-white shadow-lg shadow-green-500/25"
                    : ""
                }`}
              >
                {isSubmitting ? (
                  <>
                    <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    Submitting...
                  </>
                ) : canSubmit ? (
                  <>
                    <Trophy className="h-4 w-4 mr-2" />
                    Submit Quiz
                  </>
                ) : (
                  <>
                    <AlertCircle className="h-4 w-4 mr-2" />
                    Answer All Questions
                  </>
                )}
              </Button>
            </div>
          </div>
        </div>
      </Card>

      <Dialog open={showConfirmDialog} onOpenChange={onCancelSubmit}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-linear-to-br from-primary/20 to-primary/5">
              <Trophy className="h-8 w-8 text-primary" />
            </div>
            <DialogTitle className="text-center text-xl">
              Ready to Submit?
            </DialogTitle>
            <DialogDescription className="text-center">
              You've answered all {totalQuestions} questions. Once you submit,
              you won't be able to change your answers.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="flex-col sm:flex-row gap-2 sm:gap-0">
            <Button
              variant="outline"
              onClick={onCancelSubmit}
              className="w-full sm:w-auto"
            >
              Review Answers
            </Button>
            <Button
              onClick={onConfirmSubmit}
              className="w-full sm:w-auto bg-linear-to-r from-green-500 to-emerald-500 hover:from-green-600 hover:to-emerald-600"
            >
              <Trophy className="h-4 w-4 mr-2" />
              Submit Quiz
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
};
