import { useState } from "react";
import {
  CheckCircle2,
  XCircle,
  ImageIcon,
  Award,
  AlertTriangle,
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import type { QuizQuestion, UserAnswer } from "../types";
import { QuizImageViewer } from "./QuizImageViewer";

type QuizReviewSectionProps = {
  questions: QuizQuestion[];
  userAnswers: Map<number, UserAnswer>;
  isQuestionCorrect: (question: QuizQuestion) => boolean;
};

export function QuizReviewSection({
  questions,
  userAnswers,
  isQuestionCorrect,
}: QuizReviewSectionProps) {
  const [viewingImage, setViewingImage] = useState<string | null>(null);

  const correctCount = questions.filter((q) => isQuestionCorrect(q)).length;
  const incorrectCount = questions.length - correctCount;

  return (
    <div className="mt-8 space-y-6">
      {/* Review Header */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-6">
        <div>
          <h2 className="text-2xl font-bold">Review Your Answers</h2>
          <p className="text-muted-foreground text-sm mt-1">
            Check which questions you got right and learn from mistakes
          </p>
        </div>
        <div className="flex items-center gap-3">
          <Badge className="bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 border-0 px-3 py-1.5">
            <CheckCircle2 className="h-4 w-4 mr-1.5" />
            {correctCount} Correct
          </Badge>
          <Badge className="bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400 border-0 px-3 py-1.5">
            <XCircle className="h-4 w-4 mr-1.5" />
            {incorrectCount} Incorrect
          </Badge>
        </div>
      </div>

      {questions.map((question, index) => {
        const isCorrect = isQuestionCorrect(question);
        const userAnswer = userAnswers.get(question.id);
        const correctChoices = question.choices.filter((c) => c.isCorrect);

        return (
          <Card
            key={question.id}
            className={`overflow-hidden transition-all ${
              isCorrect
                ? "border-2 border-green-500/40 bg-linear-to-br from-green-50/50 to-transparent dark:from-green-950/20"
                : "border-2 border-red-500/40 bg-linear-to-br from-red-50/50 to-transparent dark:from-red-950/20"
            }`}
          >
            {/* Status bar */}
            <div
              className={`h-1 ${
                isCorrect
                  ? "bg-linear-to-r from-green-500 to-emerald-500"
                  : "bg-linear-to-r from-red-500 to-rose-500"
              }`}
            />

            <CardHeader className="pb-4">
              <div className="flex items-start gap-4">
                {/* Result icon */}
                <div
                  className={`flex size-12 shrink-0 items-center justify-center rounded-xl shadow-sm ${
                    isCorrect
                      ? "bg-linear-to-br from-green-500 to-emerald-500 text-white"
                      : "bg-linear-to-br from-red-500 to-rose-500 text-white"
                  }`}
                >
                  {isCorrect ? (
                    <Award className="size-6" />
                  ) : (
                    <AlertTriangle className="size-6" />
                  )}
                </div>

                <div className="flex-1 min-w-0">
                  {/* Badges */}
                  <div className="mb-3 flex flex-wrap items-center gap-2">
                    <span className="text-sm font-medium text-muted-foreground">
                      Question {index + 1}
                    </span>
                    <Badge variant="outline" className="text-xs">
                      {question.type}
                    </Badge>
                    <Badge
                      className={`text-xs font-semibold border-0 ${
                        isCorrect
                          ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400"
                          : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400"
                      }`}
                    >
                      {isCorrect ? "✓ Correct" : "✗ Incorrect"}
                    </Badge>
                  </div>

                  {/* Question text */}
                  <CardTitle className="text-lg leading-relaxed">
                    {question.text}
                  </CardTitle>

                  {/* Question image */}
                  {question.imageUrl && (
                    <button
                      type="button"
                      onClick={() => setViewingImage(question.imageUrl!)}
                      className="mt-3 relative group rounded-xl overflow-hidden border-2 border-muted hover:border-primary/50 transition-all inline-block"
                    >
                      <img
                        src={question.imageUrl}
                        alt="Question"
                        className="max-w-xs max-h-32 object-contain"
                      />
                      <div className="absolute inset-0 bg-black/0 group-hover:bg-black/30 transition-colors flex items-center justify-center">
                        <ImageIcon className="h-5 w-5 text-white opacity-0 group-hover:opacity-100 transition-opacity" />
                      </div>
                    </button>
                  )}
                </div>
              </div>
            </CardHeader>

            <CardContent className="space-y-4 pl-16 sm:pl-20">
              {/* Short Answer Type */}
              {question.type === "ShortAnswer" && (
                <div className="space-y-3">
                  <div
                    className={`rounded-xl border-2 p-4 ${
                      isCorrect
                        ? "border-green-500/30 bg-green-500/5"
                        : "border-red-500/30 bg-red-500/5"
                    }`}
                  >
                    <p className="text-muted-foreground mb-2 text-xs font-semibold uppercase tracking-wide">
                      Your Answer
                    </p>
                    <div className="flex items-start gap-2">
                      {isCorrect ? (
                        <CheckCircle2 className="mt-0.5 size-5 shrink-0 text-green-600" />
                      ) : (
                        <XCircle className="mt-0.5 size-5 shrink-0 text-red-600" />
                      )}
                      <p className="text-sm font-medium">
                        {userAnswer?.shortAnswer || "(No answer provided)"}
                      </p>
                    </div>
                  </div>
                  {!isCorrect && correctChoices.length > 0 && (
                    <div className="rounded-xl border-2 border-green-500/30 bg-green-500/5 p-4">
                      <p className="text-muted-foreground mb-2 text-xs font-semibold uppercase tracking-wide">
                        Correct Answer
                      </p>
                      <div className="flex items-start gap-2">
                        <CheckCircle2 className="mt-0.5 size-5 shrink-0 text-green-600" />
                        <p className="text-sm font-medium text-green-700 dark:text-green-400">
                          {correctChoices[0].text}
                        </p>
                      </div>
                    </div>
                  )}
                </div>
              )}

              {/* Choice-based Questions */}
              {question.type !== "ShortAnswer" && (
                <div className="space-y-2">
                  {question.choices.map((choice, choiceIndex) => {
                    const isSelected = userAnswer?.selectedChoiceIds?.includes(
                      choice.id
                    );
                    const isCorrectChoice = choice.isCorrect;
                    const letter = String.fromCharCode(65 + choiceIndex);

                    let containerClass = "border-muted bg-muted/20";
                    let letterClass = "bg-muted text-muted-foreground";
                    let textClass = "text-muted-foreground";
                    let indicator = null;

                    if (isSelected && isCorrectChoice) {
                      containerClass = "border-green-500/50 bg-green-500/10";
                      letterClass = "bg-green-500 text-white";
                      textClass = "text-green-700 dark:text-green-400";
                      indicator = (
                        <div className="flex items-center gap-1.5 text-xs font-semibold text-green-600 dark:text-green-400">
                          <CheckCircle2 className="size-4" />
                          Your answer (correct!)
                        </div>
                      );
                    } else if (isSelected && !isCorrectChoice) {
                      containerClass = "border-red-500/50 bg-red-500/10";
                      letterClass = "bg-red-500 text-white";
                      textClass = "text-red-700 dark:text-red-400";
                      indicator = (
                        <div className="flex items-center gap-1.5 text-xs font-semibold text-red-600 dark:text-red-400">
                          <XCircle className="size-4" />
                          Your answer (incorrect)
                        </div>
                      );
                    } else if (!isSelected && isCorrectChoice) {
                      containerClass =
                        "border-green-500/30 border-dashed bg-green-500/5";
                      letterClass = "bg-green-500/20 text-green-600";
                      textClass = "text-green-600 dark:text-green-400";
                      indicator = (
                        <div className="flex items-center gap-1.5 text-xs font-semibold text-green-600 dark:text-green-400">
                          <CheckCircle2 className="size-4" />
                          Correct answer
                        </div>
                      );
                    }

                    return (
                      <div
                        key={choice.id}
                        className={`flex items-start gap-3 rounded-xl border-2 p-4 transition-all ${containerClass}`}
                      >
                        {/* Letter indicator */}
                        <div
                          className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg text-sm font-semibold ${letterClass}`}
                        >
                          {letter}
                        </div>

                        <div className="min-w-0 flex-1 space-y-2">
                          <p className={`text-sm font-medium ${textClass}`}>
                            {choice.text}
                          </p>

                          {choice.imageUrl && (
                            <button
                              type="button"
                              onClick={() => setViewingImage(choice.imageUrl!)}
                              className="relative group rounded-lg overflow-hidden border-2 border-muted hover:border-primary/50 transition-colors inline-block"
                            >
                              <img
                                src={choice.imageUrl}
                                alt={choice.text}
                                className="h-16 w-auto max-w-[100px] object-cover"
                              />
                              <div className="absolute inset-0 bg-black/0 group-hover:bg-black/30 transition-colors flex items-center justify-center">
                                <ImageIcon className="h-4 w-4 text-white opacity-0 group-hover:opacity-100 transition-opacity" />
                              </div>
                            </button>
                          )}

                          {indicator}
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </CardContent>
          </Card>
        );
      })}

      {/* Image Viewer */}
      {viewingImage && (
        <QuizImageViewer
          open={!!viewingImage}
          onOpenChange={() => setViewingImage(null)}
          imageUrl={viewingImage}
          alt="Quiz image"
        />
      )}
    </div>
  );
}
