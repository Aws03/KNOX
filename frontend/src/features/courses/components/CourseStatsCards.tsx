import { BookOpen, FileText, Trophy } from "lucide-react";
import { Card, CardContent } from "@/shared/ui/card";

interface CourseStatsCardsProps {
  totalCount: number;
  totalMaterials: number;
  totalQuizzes: number;
  currentPage: number;
  totalPages: number;
}

export const CourseStatsCards = ({
  totalCount,
  totalMaterials,
  totalQuizzes,
  currentPage,
  totalPages,
}: CourseStatsCardsProps) => {
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center gap-4">
            <div className="flex size-12 items-center justify-center rounded-lg bg-primary/10">
              <BookOpen className="size-6 text-primary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Courses</p>
              <p className="text-2xl font-bold">{totalCount}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center gap-4">
            <div className="flex size-12 items-center justify-center rounded-lg bg-blue-500/10">
              <FileText className="size-6 text-blue-600" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Materials</p>
              <p className="text-2xl font-bold">{totalMaterials}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center gap-4">
            <div className="flex size-12 items-center justify-center rounded-lg bg-green-500/10">
              <Trophy className="size-6 text-green-600" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Quizzes</p>
              <p className="text-2xl font-bold">{totalQuizzes}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center gap-4">
            <div className="flex size-12 items-center justify-center rounded-lg bg-orange-500/10">
              <BookOpen className="size-6 text-orange-600" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Showing Page</p>
              <p className="text-2xl font-bold">
                {currentPage} / {totalPages}
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
