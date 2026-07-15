import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import type { QuizQuestion } from "../../types";

type ShortAnswerQuestionProps = {
  question: QuizQuestion;
  answer?: string;
  onAnswerChange: (answer: string) => void;
  disabled?: boolean;
  showCorrectAnswer?: boolean;
  correctAnswer?: string;
};

export function ShortAnswerQuestion({
  question,
  answer = "",
  onAnswerChange,
  disabled = false,
  showCorrectAnswer = false,
  correctAnswer,
}: ShortAnswerQuestionProps) {
  return (
    <div className="space-y-4">
      {question.imageUrl && (
        <div className="rounded-lg overflow-hidden border">
          <img
            src={question.imageUrl}
            alt="Question"
            className="w-full h-auto"
          />
        </div>
      )}

      <div className="space-y-2">
        <Label htmlFor="short-answer">Your Answer</Label>
        <Input
          id="short-answer"
          type="text"
          placeholder="Type your answer here..."
          value={answer}
          onChange={(e) => onAnswerChange(e.target.value)}
          disabled={disabled}
          className={
            showCorrectAnswer && answer.trim()
              ? answer.trim().toLowerCase() === correctAnswer?.toLowerCase()
                ? "border-green-500 bg-green-50 dark:bg-green-950/20"
                : "border-red-500 bg-red-50 dark:bg-red-950/20"
              : ""
          }
        />

        {showCorrectAnswer && correctAnswer && (
          <div className="mt-2 p-3 rounded-md bg-muted">
            <p className="text-sm">
              <span className="font-medium">Correct Answer:</span>{" "}
              <span className="text-green-600 dark:text-green-400">
                {correctAnswer}
              </span>
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
