import { useState } from "react";
import { Badge } from "@/shared/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { RadioGroup, RadioGroupItem } from "@/shared/ui/radio-group";
import { Label } from "@/shared/ui/label";
import { ImageIcon, CheckCircle2 } from "lucide-react";
import type { QuizQuestion, UserAnswer } from "../types";
import {
  SingleChoiceQuestion,
  MultipleChoiceQuestion,
  ShortAnswerQuestion,
} from "./question-renderers";
import { QuizImageViewer } from "./QuizImageViewer";

interface QuizQuestionCardProps {
  question: QuizQuestion;
  index: number;
  isAnswered: boolean;
  userAnswer?: UserAnswer;
  onSingleChoice: (questionId: number, choiceId: number) => void;
  onMultipleChoice: (
    questionId: number,
    choiceId: number,
    checked: boolean
  ) => void;
  onShortAnswer: (questionId: number, answer: string) => void;
  isHighlighted?: boolean;
}

const questionTypeLabels: Record<string, { label: string; color: string }> = {
  SingleChoice: {
    label: "Single Choice",
    color: "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400",
  },
  MultipleChoice: {
    label: "Multiple Choice",
    color:
      "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400",
  },
  TrueFalse: {
    label: "True / False",
    color:
      "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400",
  },
  ShortAnswer: {
    label: "Short Answer",
    color:
      "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400",
  },
};

export const QuizQuestionCard = ({
  question,
  index,
  isAnswered,
  userAnswer,
  onSingleChoice,
  onMultipleChoice,
  onShortAnswer,
  isHighlighted = false,
}: QuizQuestionCardProps) => {
  const [isImageViewerOpen, setIsImageViewerOpen] = useState(false);

  const typeInfo = questionTypeLabels[question.type] || {
    label: question.type,
    color: "bg-muted",
  };

  const renderQuestionContent = () => {
    switch (question.type) {
      case "SingleChoice":
        return (
          <SingleChoiceQuestion
            question={question}
            selectedChoiceId={userAnswer?.selectedChoiceIds?.[0]}
            onSelect={(choiceId) => onSingleChoice(question.id, choiceId)}
          />
        );

      case "MultipleChoice":
        return (
          <MultipleChoiceQuestion
            question={question}
            selectedChoiceIds={userAnswer?.selectedChoiceIds || []}
            onToggle={(choiceId, checked) =>
              onMultipleChoice(question.id, choiceId, checked)
            }
          />
        );

      case "TrueFalse":
        return (
          <RadioGroup
            value={userAnswer?.selectedChoiceIds?.[0]?.toString() || ""}
            onValueChange={(value) =>
              onSingleChoice(question.id, parseInt(value))
            }
            className="space-y-2"
          >
            {question.choices.map((choice, idx) => {
              const isSelected =
                userAnswer?.selectedChoiceIds?.[0] === choice.id;
              return (
                <div
                  key={choice.id}
                  className={`group flex items-center gap-3 rounded-xl border-2 p-4 transition-all duration-200 cursor-pointer ${
                    isSelected
                      ? "border-primary bg-primary/5 ring-2 ring-primary/20"
                      : "hover:border-primary/50 hover:bg-muted/50"
                  }`}
                  onClick={() => onSingleChoice(question.id, choice.id)}
                >
                  <div
                    className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-full text-sm font-bold transition-colors ${
                      isSelected
                        ? "bg-primary text-primary-foreground"
                        : "bg-muted text-muted-foreground group-hover:bg-primary/10 group-hover:text-primary"
                    }`}
                  >
                    {idx === 0 ? "T" : "F"}
                  </div>
                  <RadioGroupItem
                    value={choice.id.toString()}
                    id={`choice-${choice.id}`}
                    className="sr-only"
                  />
                  <Label
                    htmlFor={`choice-${choice.id}`}
                    className={`flex-1 cursor-pointer font-semibold text-base ${
                      isSelected
                        ? "text-foreground"
                        : "text-muted-foreground group-hover:text-foreground"
                    }`}
                  >
                    {choice.text}
                  </Label>
                </div>
              );
            })}
          </RadioGroup>
        );

      case "ShortAnswer":
        return (
          <ShortAnswerQuestion
            question={question}
            answer={userAnswer?.shortAnswer || ""}
            onAnswerChange={(answer) => onShortAnswer(question.id, answer)}
          />
        );

      default:
        return null;
    }
  };

  return (
    <>
      <Card
        id={`question-${question.id}`}
        className={`relative overflow-hidden transition-all duration-300 ${
          isHighlighted
            ? "border-2 border-orange-500 ring-4 ring-orange-500/20 animate-pulse"
            : isAnswered
            ? "border-2 border-green-500/40 bg-linear-to-br from-green-50/50 to-transparent dark:from-green-950/20"
            : "border-2 border-muted hover:border-muted-foreground/20"
        }`}
      >
        {/* Answered indicator bar */}
        {isAnswered && (
          <div className="absolute top-0 left-0 right-0 h-1 bg-linear-to-r from-green-500 to-emerald-500" />
        )}

        <CardHeader className="pb-4">
          <div className="flex items-start gap-4">
            {/* Question number */}
            <div
              className={`flex h-12 w-12 shrink-0 items-center justify-center rounded-xl font-bold text-lg shadow-sm ${
                isAnswered
                  ? "bg-linear-to-br from-green-500 to-emerald-500 text-white"
                  : "bg-linear-to-br from-primary/10 to-primary/5 text-primary border border-primary/20"
              }`}
            >
              {isAnswered ? <CheckCircle2 className="h-6 w-6" /> : index + 1}
            </div>

            <div className="flex-1 min-w-0">
              {/* Badges */}
              <div className="flex flex-wrap items-center gap-2 mb-3">
                <span className="text-sm font-medium text-muted-foreground">
                  Question {index + 1}
                </span>
                <Badge
                  className={`text-xs font-medium ${typeInfo.color} border-0`}
                >
                  {typeInfo.label}
                </Badge>
                {isAnswered && (
                  <Badge className="text-xs font-medium bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 border-0">
                    Answered
                  </Badge>
                )}
              </div>

              {/* Question text */}
              <CardTitle className="text-lg font-semibold leading-relaxed text-foreground">
                {question.text}
              </CardTitle>

              {/* Question image */}
              {question.imageUrl && (
                <button
                  type="button"
                  onClick={() => setIsImageViewerOpen(true)}
                  className="mt-4 relative group rounded-xl overflow-hidden border-2 border-muted hover:border-primary/50 transition-all inline-block"
                >
                  <img
                    src={question.imageUrl}
                    alt="Question"
                    className="w-auto max-h-60 object-contain"
                  />
                  <div className="absolute inset-0 bg-black/0 group-hover:bg-black/30 transition-colors flex items-center justify-center">
                    <div className="flex items-center gap-2 text-white opacity-0 group-hover:opacity-100 transition-opacity bg-black/50 px-3 py-1.5 rounded-full text-sm font-medium">
                      <ImageIcon className="h-4 w-4" />
                      Click to view
                    </div>
                  </div>
                </button>
              )}
            </div>
          </div>
        </CardHeader>

        <CardContent className="pt-0 pl-16 sm:pl-20">
          {renderQuestionContent()}
        </CardContent>
      </Card>

      {/* Image Viewer Dialog */}
      {question.imageUrl && (
        <QuizImageViewer
          open={isImageViewerOpen}
          onOpenChange={setIsImageViewerOpen}
          imageUrl={question.imageUrl}
          alt={`Question ${index + 1} image`}
        />
      )}
    </>
  );
};
