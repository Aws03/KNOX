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
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import type { QuizListItem } from "../types";
import { formatDate, getTimeAgo } from "../types";

interface QuizCardGridProps {
  quiz: QuizListItem;
  courseId: string;
  isManagementMode: boolean;
  onQuizClick: (quizId: number) => void;
  onEdit?: (quizId: number) => void;
  onDelete?: (quizId: number) => void;
}

export const QuizCardGrid = ({
  quiz,
  courseId,
  isManagementMode,
  onQuizClick,
  onEdit,
  onDelete,
}: QuizCardGridProps) => {
  return (
    <Card
      className="group relative overflow-hidden transition-all duration-300 hover:shadow-2xl hover:scale-[1.02] cursor-pointer border-2 hover:border-primary/50 bg-linear-to-br from-card via-card to-card/80"
      onClick={() => onQuizClick(quiz.id)}
    >
      {/* Decorative gradient */}
      <div className="absolute top-0 right-0 w-32 h-32 bg-linear-to-br from-primary/10 via-primary/5 to-transparent rounded-bl-full transition-all duration-300 group-hover:from-primary/20 group-hover:scale-110" />

      <CardHeader className="pb-4 relative">
        <div className="mb-3 flex items-start justify-between gap-2">
          <Badge
            variant="secondary"
            className="shrink-0 transition-colors group-hover:bg-primary/20"
          >
            <Trophy className="h-3 w-3 mr-1" />
            Quiz #{quiz.id}
          </Badge>
          <div className="flex items-center gap-1">
            {!isManagementMode && (
              <ShareButton
                url={`/courses/${courseId}/quizzes/${quiz.id}`}
                title={`Check out this quiz: ${quiz.title}`}
                variant="ghost"
                size="icon"
              />
            )}
            {isManagementMode && (
              <>
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
              </>
            )}
            <Badge
              variant="outline"
              className="shrink-0 gap-1 border-primary/30"
            >
              <ThumbsUp className="h-3 w-3" />
              {quiz.likes}
            </Badge>
          </div>
        </div>
        <CardTitle className="line-clamp-2 text-base leading-snug transition-colors duration-300 group-hover:text-primary">
          {quiz.title}
        </CardTitle>
      </CardHeader>

      <CardContent className="pt-0 space-y-3">
        {/* Author */}
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <User className="h-4 w-4" />
          <span className="truncate">{quiz.writerName}</span>
        </div>

        {/* Date */}
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <Calendar className="h-4 w-4" />
          <span>{formatDate(quiz.createdAt)}</span>
          <Badge variant="outline" className="text-xs ml-auto">
            <Clock className="h-3 w-3 mr-1" />
            {getTimeAgo(quiz.createdAt)}
          </Badge>
        </div>

        {/* Button */}
        {!isManagementMode ? (
          <Button
            className="w-full mt-3 transition-all duration-300 hover:shadow-lg hover:scale-105"
            variant="outline"
            size="sm"
            onClick={(e) => {
              e.stopPropagation();
              onQuizClick(quiz.id);
            }}
          >
            Start Quiz
          </Button>
        ) : (
          <div className="flex gap-2 mt-3">
            <Button
              className="flex-1 transition-all duration-300"
              variant="outline"
              size="sm"
              onClick={(e) => {
                e.stopPropagation();
                onQuizClick(quiz.id);
              }}
            >
              View Quiz
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
