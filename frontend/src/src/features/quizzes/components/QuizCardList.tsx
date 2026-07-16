import {
  Trophy,
  ThumbsUp,
  User,
  Calendar,
  Clock,
  Edit,
  Trash2,
} from "lucide-react";
import { Button } from "@/shared/ui/button";
import ShareButton from "@/shared/ui/ShareButton";
import { Card } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import type { QuizListItem } from "../types";
import { formatDate, getTimeAgo } from "../types";

interface QuizCardListProps {
  quiz: QuizListItem;
  courseId: string;
  isManagementMode: boolean;
  onQuizClick: (quizId: number) => void;
  onEdit?: (quizId: number) => void;
  onDelete?: (quizId: number) => void;
}

export const QuizCardList = ({
  quiz,
  courseId,
  isManagementMode,
  onQuizClick,
  onEdit,
  onDelete,
}: QuizCardListProps) => {
  return (
    <Card
      className="group overflow-hidden transition-all duration-300 hover:shadow-xl cursor-pointer border-2 hover:border-primary/50 bg-linear-to-r from-card via-card to-card/80 hover:translate-x-1"
      onClick={() => onQuizClick(quiz.id)}
    >
      <div className="flex flex-col lg:flex-row lg:items-center p-6 gap-6">
        {/* Left: Quiz Info */}
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-3 mb-3 flex-wrap">
            <Badge
              variant="secondary"
              className="shrink-0 transition-colors group-hover:bg-primary/20"
            >
              <Trophy className="h-3 w-3 mr-1" />
              Quiz #{quiz.id}
            </Badge>
            <h3 className="text-xl font-semibold transition-colors duration-300 group-hover:text-primary">
              {quiz.title}
            </h3>
            {!isManagementMode && (
              <ShareButton
                url={`/courses/${courseId}/quizzes/${quiz.id}`}
                title={`Check out this quiz: ${quiz.title}`}
                variant="ghost"
                size="icon"
              />
            )}
          </div>

          <div className="flex flex-wrap gap-4 text-sm text-muted-foreground">
            <div className="flex items-center gap-2">
              <User className="h-4 w-4" />
              <span>{quiz.writerName}</span>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              <span>{formatDate(quiz.createdAt)}</span>
            </div>
            <div className="flex items-center gap-2">
              <Clock className="h-4 w-4" />
              <span>{getTimeAgo(quiz.createdAt)}</span>
            </div>
          </div>
        </div>

        {/* Right: Actions */}
        <div className="flex items-center gap-4 lg:border-l lg:pl-6 border-border/50">
          <div className="flex items-center gap-2 text-sm">
            <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10 transition-all duration-300 group-hover:bg-primary/20">
              <ThumbsUp className="h-5 w-5 text-primary" />
            </div>
            <div>
              <div className="font-semibold text-lg">{quiz.likes}</div>
              <div className="text-xs text-muted-foreground">Likes</div>
            </div>
          </div>

          {!isManagementMode ? (
            <Button
              className="lg:w-44 transition-all duration-300 hover:shadow-lg hover:scale-105"
              variant="outline"
              size="default"
              onClick={(e) => {
                e.stopPropagation();
                onQuizClick(quiz.id);
              }}
            >
              Start Quiz
            </Button>
          ) : (
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="default"
                onClick={(e) => {
                  e.stopPropagation();
                  onQuizClick(quiz.id);
                }}
              >
                View Quiz
              </Button>
              <Button
                variant="ghost"
                size="icon"
                onClick={(e) => {
                  e.stopPropagation();
                  onEdit?.(quiz.id);
                }}
              >
                <Edit className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                onClick={(e) => {
                  e.stopPropagation();
                  onDelete?.(quiz.id);
                }}
              >
                <Trash2 className="h-4 w-4 text-destructive" />
              </Button>
            </div>
          )}
        </div>
      </div>
    </Card>
  );
};
