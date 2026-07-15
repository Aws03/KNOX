import {
  CheckCircle2,
  XCircle,
  Trophy,
  ThumbsUp,
  ThumbsDown,
} from "lucide-react";
import { Card } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { Button } from "@/shared/ui/button";
import { Progress } from "@/shared/ui/progress";

type QuizResultsCardProps = {
  score: number;
  totalQuestions: number;
  percentage: number;
  quizTitle: string;
  likes?: number;
  dislikes?: number;
  isLoggedIn?: boolean;
  onLike?: () => void;
  onDislike?: () => void;
  onTryAgain: () => void;
  onBackToQuizzes: () => void;
};

export function QuizResultsCard({
  score,
  totalQuestions,
  percentage,
  quizTitle,
  likes = 0,
  dislikes = 0,
  isLoggedIn = false,
  onLike,
  onDislike,
  onTryAgain,
  onBackToQuizzes,
}: QuizResultsCardProps) {
  const isPassed = percentage >= 60;

  const getPerformanceBadge = () => {
    if (percentage >= 90) {
      return (
        <Badge variant="default" className="text-base px-4 py-2">
          <Trophy className="mr-2 size-4" />
          Excellent!
        </Badge>
      );
    }
    if (percentage >= 70) {
      return (
        <Badge variant="secondary" className="text-base px-4 py-2">
          <CheckCircle2 className="mr-2 size-4" />
          Well Done!
        </Badge>
      );
    }
    if (percentage >= 60) {
      return (
        <Badge variant="outline" className="text-base px-4 py-2">
          Passed
        </Badge>
      );
    }
    return (
      <Badge variant="outline" className="text-base px-4 py-2">
        Try Again
      </Badge>
    );
  };

  return (
    <Card className="border-2 bg-linear-to-br from-card to-muted/20 p-8">
      <div className="space-y-6 text-center">
        {/* Result Icon */}
        <div
          className={`mx-auto flex size-24 items-center justify-center rounded-full ${
            isPassed
              ? "bg-green-500/10 text-green-500"
              : "bg-orange-500/10 text-orange-500"
          }`}
        >
          {isPassed ? (
            <CheckCircle2 className="size-12" />
          ) : (
            <XCircle className="size-12" />
          )}
        </div>

        {/* Title */}
        <div>
          <h2 className="mb-2 text-3xl font-bold">
            {isPassed ? "Great Job!" : "Keep Practicing!"}
          </h2>
          <p className="text-muted-foreground">
            You've completed "{quizTitle}"
          </p>
        </div>

        {/* Score */}
        <div className="space-y-4">
          <div>
            <div className="mb-2 text-6xl font-bold text-primary">
              {percentage}%
            </div>
            <p className="text-muted-foreground text-lg">
              {score} out of {totalQuestions} correct
            </p>
          </div>
          <Progress value={percentage} className="h-3" />
        </div>

        {/* Performance Badge */}
        <div className="pt-4">{getPerformanceBadge()}</div>

        {/* Like/Dislike Buttons */}
        {isLoggedIn && (onLike || onDislike) && (
          <div className="flex items-center justify-center gap-4 border-t pt-6">
            <p className="text-muted-foreground text-sm">Rate this quiz:</p>
            <div className="flex gap-2">
              {onLike && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onLike}
                  className="gap-2 transition-all hover:border-green-500/50 hover:bg-green-500/10 hover:text-green-500"
                >
                  <ThumbsUp className="size-4" />
                  Like ({likes})
                </Button>
              )}
              {onDislike && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onDislike}
                  className="gap-2 transition-all hover:border-red-500/50 hover:bg-red-500/10 hover:text-red-500"
                >
                  <ThumbsDown className="size-4" />
                  Dislike ({dislikes})
                </Button>
              )}
            </div>
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex flex-col gap-3 pt-6 sm:flex-row">
          <Button variant="outline" className="flex-1" onClick={onTryAgain}>
            Try Again
          </Button>
          <Button className="flex-1" onClick={onBackToQuizzes}>
            Back to Quizzes
          </Button>
        </div>
      </div>
    </Card>
  );
}
