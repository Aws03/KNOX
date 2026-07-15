import {
  User,
  Calendar,
  FileQuestion,
  Trophy,
  ThumbsUp,
  ThumbsDown,
} from "lucide-react";
import { Badge } from "@/shared/ui/badge";
import { Card } from "@/shared/ui/card";
import { Progress } from "@/shared/ui/progress";
import ShareButton from "@/shared/ui/ShareButton";
import type { QuizDetails } from "../types";
import { formatDate } from "../types";

interface QuizHeaderProps {
  quiz: QuizDetails;
  answeredCount: number;
  totalQuestions: number;
  courseId: string;
  quizId: string;
}

export const QuizHeader = ({
  quiz,
  answeredCount,
  totalQuestions,
  courseId,
  quizId,
}: QuizHeaderProps) => {
  const progressPercentage = (answeredCount / totalQuestions) * 100;

  return (
    <Card className="p-6 bg-linear-to-br from-card to-muted/10 border-2">
      <div className="space-y-4">
        {/* Title and Share Section */}
        <div className="flex items-start justify-between gap-4">
          <div className="flex-1">
            <div className="flex items-center gap-2 mb-2">
              <Badge variant="secondary">
                <Trophy className="h-3 w-3 mr-1" />
                Quiz #{quiz.id}
              </Badge>
              <Badge variant="outline">
                <FileQuestion className="h-3 w-3 mr-1" />
                {totalQuestions} Questions
              </Badge>
            </div>
            <h1 className="text-2xl font-bold mb-2">{quiz.title}</h1>
            {quiz.description && (
              <p className="text-muted-foreground">{quiz.description}</p>
            )}
          </div>
          <ShareButton
            url={`/courses/${courseId}/quizzes/${quizId}`}
            title={`Check out this quiz: ${quiz.title}`}
            showText={true}
          />
        </div>

        {/* Metadata Section */}
        <div className="flex flex-wrap gap-4 text-sm text-muted-foreground border-t pt-4">
          <div className="flex items-center gap-2">
            <User className="h-4 w-4" />
            <span>Writer: {quiz.writerName}</span>
          </div>
          <div className="flex items-center gap-2">
            <Calendar className="h-4 w-4" />
            <span>{formatDate(quiz.createdAt)}</span>
          </div>
          <div className="flex items-center gap-2">
            <ThumbsUp className="h-4 w-4" />
            <span>{quiz.likes} likes</span>
          </div>
          <div className="flex items-center gap-2">
            <ThumbsDown className="h-4 w-4" />
            <span>{quiz.dislikes} dislikes</span>
          </div>
        </div>

        {/* Progress Section */}
        <div className="space-y-2 border-t pt-4">
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">Progress</span>
            <span className="font-semibold">
              {answeredCount} / {totalQuestions} answered
            </span>
          </div>
          <Progress value={progressPercentage} className="h-2" />
        </div>
      </div>
    </Card>
  );
};
