import { Checkbox } from "@/shared/ui/checkbox";
import { Label } from "@/shared/ui/label";
import type { QuizQuestion, QuestionChoice } from "../../types";
import { useState } from "react";
import { QuizImageViewer } from "../QuizImageViewer";
import { ImageIcon } from "lucide-react";

type MultipleChoiceQuestionProps = {
  question: QuizQuestion;
  selectedChoiceIds?: number[];
  onToggle: (choiceId: number, checked: boolean) => void;
  disabled?: boolean;
  showCorrectAnswer?: boolean;
};

export function MultipleChoiceQuestion({
  question,
  selectedChoiceIds = [],
  onToggle,
  disabled = false,
  showCorrectAnswer = false,
}: MultipleChoiceQuestionProps) {
  const [viewingImage, setViewingImage] = useState<string | null>(null);

  const getChoiceClassName = (choice: QuestionChoice) => {
    const isSelected = selectedChoiceIds.includes(choice.id);

    if (!showCorrectAnswer) {
      return isSelected
        ? "border-primary bg-primary/5 ring-2 ring-primary/20"
        : "hover:border-primary/50 hover:bg-muted/50";
    }

    if (choice.isCorrect) {
      return "border-green-500 bg-green-50 dark:bg-green-950/20";
    }

    if (isSelected && !choice.isCorrect) {
      return "border-red-500 bg-red-50 dark:bg-red-950/20";
    }

    return "";
  };

  return (
    <div className="space-y-3">
      <p className="text-xs text-muted-foreground mb-2">
        Select all that apply
      </p>
      <div className="space-y-2">
        {question.choices.map((choice, index) => {
          const isSelected = selectedChoiceIds.includes(choice.id);
          const letter = String.fromCharCode(65 + index); // A, B, C, D...

          return (
            <div
              key={choice.id}
              className={`group flex items-start gap-3 rounded-xl border-2 p-4 transition-all duration-200 cursor-pointer ${getChoiceClassName(
                choice
              )}`}
              onClick={() => !disabled && onToggle(choice.id, !isSelected)}
            >
              {/* Letter indicator */}
              <div
                className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg text-sm font-semibold transition-colors ${
                  isSelected
                    ? "bg-primary text-primary-foreground"
                    : "bg-muted text-muted-foreground group-hover:bg-primary/10 group-hover:text-primary"
                }`}
              >
                {letter}
              </div>

              <Checkbox
                id={`choice-${choice.id}`}
                checked={isSelected}
                onCheckedChange={(checked) =>
                  onToggle(choice.id, checked as boolean)
                }
                disabled={disabled}
                className="mt-1 sr-only"
              />
              <Label
                htmlFor={`choice-${choice.id}`}
                className="flex-1 cursor-pointer"
              >
                <div className="space-y-2">
                  <p
                    className={`text-sm font-medium leading-relaxed ${
                      isSelected
                        ? "text-foreground"
                        : "text-muted-foreground group-hover:text-foreground"
                    }`}
                  >
                    {choice.text}
                  </p>
                  {choice.imageUrl && (
                    <button
                      type="button"
                      onClick={(e) => {
                        e.stopPropagation();
                        setViewingImage(choice.imageUrl!);
                      }}
                      className="relative group/img rounded-lg overflow-hidden border-2 border-muted hover:border-primary/50 transition-colors inline-block"
                    >
                      <img
                        src={choice.imageUrl}
                        alt={`Choice ${choice.id}`}
                        className="h-42 w-auto max-w-max object-cover"
                      />
                      <div className="absolute inset-0 bg-black/0 group-hover/img:bg-black/30 transition-colors flex items-center justify-center">
                        <ImageIcon className="h-5 w-5 text-white opacity-0 group-hover/img:opacity-100 transition-opacity" />
                      </div>
                    </button>
                  )}
                </div>
              </Label>

              {/* Checkbox indicator */}
              <div
                className={`flex h-6 w-6 shrink-0 items-center justify-center rounded border-2 transition-colors ${
                  isSelected
                    ? "bg-primary border-primary text-primary-foreground"
                    : "border-muted-foreground/30"
                }`}
              >
                {isSelected && (
                  <svg
                    className="h-4 w-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={3}
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                )}
              </div>
            </div>
          );
        })}
      </div>

      {/* Image Viewer */}
      {viewingImage && (
        <QuizImageViewer
          open={!!viewingImage}
          onOpenChange={() => setViewingImage(null)}
          imageUrl={viewingImage}
          alt="Choice image"
        />
      )}
    </div>
  );
}
