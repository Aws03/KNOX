import { useNavigate } from "react-router-dom";
import { BookOpen, FileText, Loader2, Trophy, Link2 } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/ui/table";
import { PageSizeSelector } from "@/shared/components/pagination/PageSizeSelector";
import type { CourseApiResponse } from "@/features/courses/types";

interface CoursesTableProps {
  courses: CourseApiResponse[];
  loading: boolean;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  onPageSizeChange: (size: number) => void;
  hasActiveFilters: boolean;
}

const getRequirementTypeBadgeColor = (type: string) => {
  const colors: Record<string, string> = {
    Specialist: "bg-purple-500/10 text-purple-700 border-purple-300",
    "Core Course": "bg-blue-500/10 text-blue-700 border-blue-300",
    "Free Course": "bg-green-500/10 text-green-700 border-green-300",
    Elective: "bg-orange-500/10 text-orange-700 border-orange-300",
    "Lab Course": "bg-pink-500/10 text-pink-700 border-pink-300",
  };
  return colors[type] || "bg-gray-500/10 text-gray-700 border-gray-300";
};

const getRequirementNatureBadgeColor = (nature: string) => {
  const colors: Record<string, string> = {
    Compulsory: "bg-red-500/10 text-red-700 border-red-300",
    Optional: "bg-green-500/10 text-green-700 border-green-300",
  };
  return colors[nature] || "bg-gray-500/10 text-gray-700 border-gray-300";
};

export const CoursesTable = ({
  courses,
  loading,
  totalCount,
  currentPage,
  pageSize,
  onPageSizeChange,
  hasActiveFilters,
}: CoursesTableProps) => {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader>
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Courses</CardTitle>
            <CardDescription>
              {totalCount > 0 ? (
                <>
                  Showing{" "}
                  <span className="font-semibold text-foreground">
                    {(currentPage - 1) * pageSize + 1}-
                    {Math.min(currentPage * pageSize, totalCount)}
                  </span>{" "}
                  of{" "}
                  <span className="font-semibold text-foreground">
                    {totalCount}
                  </span>{" "}
                  courses
                </>
              ) : (
                "No courses found"
              )}
            </CardDescription>
          </div>
          <div className="flex items-center gap-2">
            <PageSizeSelector pageSize={pageSize} onChange={onPageSizeChange} />
          </div>
        </div>
      </CardHeader>

      <CardContent>
        {loading ? (
          <div className="flex flex-col items-center justify-center py-12">
            <Loader2 className="size-8 animate-spin text-primary" />
            <p className="mt-3 text-sm text-muted-foreground">
              Loading courses...
            </p>
          </div>
        ) : courses.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-12">
            <div className="flex size-16 items-center justify-center rounded-full bg-muted">
              <BookOpen className="size-8 text-muted-foreground" />
            </div>
            <h3 className="mt-4 text-lg font-semibold">No courses found</h3>
            <p className="text-muted-foreground mt-2 text-center text-sm">
              {hasActiveFilters
                ? "Try adjusting your filters"
                : "No courses available for this major"}
            </p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-20">Code</TableHead>
                  <TableHead>Course Name</TableHead>
                  <TableHead className="w-[100px] text-center">
                    Credits
                  </TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Nature</TableHead>
                  <TableHead className="w-[100px] text-center">
                    Materials
                  </TableHead>
                  <TableHead className="w-[100px] text-center">
                    Quizzes
                  </TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {courses.map((course) => (
                  <TableRow key={course.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {course.courseCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        <p className="font-medium">{course.courseName}</p>
                        {course.description && (
                          <p className="text-xs text-muted-foreground line-clamp-1">
                            {course.description}
                          </p>
                        )}
                      </div>
                    </TableCell>
                    <TableCell className="text-center">
                      <Badge variant="secondary">{course.credits}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge
                        variant="outline"
                        className={getRequirementTypeBadgeColor(
                          course.requirementType
                        )}
                      >
                        {course.requirementType}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge
                        variant="outline"
                        className={getRequirementNatureBadgeColor(
                          course.requirementNature
                        )}
                      >
                        {course.requirementNature}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <FileText className="size-4 text-muted-foreground" />
                        <span className="font-medium">
                          {course.numberOfMaterials}
                        </span>
                      </div>
                    </TableCell>
                    <TableCell className="text-center">
                      <div className="flex items-center justify-center gap-1">
                        <Trophy className="size-4 text-muted-foreground" />
                        <span className="font-medium">
                          {course.numberOfQuizzes}
                        </span>
                      </div>
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex justify-end gap-2">
                        <Button
                          variant="outline"
                          className="hover:cursor-pointer"
                          size="sm"
                          onClick={() =>
                            navigate(
                              `/dashboard/courses/${course.id}/materials`
                            )
                          }
                        >
                          <FileText className="mr-2 size-4" />
                          Materials
                        </Button>
                        <Button
                          variant="outline"
                          className="hover:cursor-pointer"
                          size="sm"
                          onClick={() =>
                            navigate(`/dashboard/courses/${course.id}/quizzes`)
                          }
                        >
                          <Trophy className="mr-2 size-4" />
                          Quizzes
                        </Button>
                        <Button
                          variant="outline"
                          className="hover:cursor-pointer"
                          size="sm"
                          onClick={() =>
                            navigate(
                              `/dashboard/courses/${course.id}/resources`
                            )
                          }
                        >
                          <Link2 className="mr-2 size-4" />
                          Resources
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
