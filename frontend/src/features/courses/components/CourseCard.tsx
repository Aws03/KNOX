import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { FileText, Trophy, Info, UserPlus, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/shared/ui/button";
import ShareButton from "@/shared/ui/ShareButton";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import { apiClient } from "@/lib/api/apiClient";
import { useAuth } from "@/app/providers/useAuth";
import type { Course } from "../types";

type CourseCardProps = {
  course: Course;
  viewMode: "grid" | "list";
  onClick?: (courseId: number) => void;
};

export type { Course };

export default function CourseCard({
  course,
  viewMode,
  onClick,
}: CourseCardProps) {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [isEnrollDialogOpen, setIsEnrollDialogOpen] = useState(false);
  const [isEnrolling, setIsEnrolling] = useState(false);

  const handleClick = () => {
    if (onClick) {
      onClick(course.id);
    }
  };

  const handleEnroll = async () => {
    setIsEnrolling(true);
    try {
      await apiClient.post(`/courses/${course.id}/enroll`, { notes: null });
      setIsEnrollDialogOpen(false);
      toast.success("Successfully enrolled in the course!", {
        description: "You can now access this course from your profile.",
      });
    } catch (error) {
      console.error("Enrollment failed:", error);
      toast.error("Failed to enroll", {
        description: "Maybe you are already enrolled.",
      });
    } finally {
      setIsEnrolling(false);
    }
  };

  if (viewMode === "grid") {
    return (
      <Card
        className="group relative overflow-hidden transition-all duration-300 hover:shadow-2xl cursor-pointer border hover:border-primary/50 bg-linear-to-br from-card via-card to-card/80 flex flex-col h-full"
        onClick={handleClick}
      >
        {/* Decorative gradient */}
        <div className="absolute top-0 right-0 w-24 h-24 bg-linear-to-br from-primary/10 via-primary/5 to-transparent rounded-bl-full transition-all duration-300 group-hover:from-primary/20 group-hover:scale-110" />

        {/* Header */}
        <CardHeader className="pb-3 relative space-y-2">
          <div className="flex items-center justify-between gap-2">
            <Badge
              variant="secondary"
              className="shrink-0 font-mono text-xs transition-colors group-hover:bg-primary/20"
            >
              {course.code}
            </Badge>
            <div className="flex items-center gap-1">
              {isAuthenticated && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8 cursor-pointer"
                  onClick={(e) => {
                    e.stopPropagation();
                    setIsEnrollDialogOpen(true);
                  }}
                  title="Enroll in course"
                >
                  <UserPlus className="h-4 w-4" />
                </Button>
              )}
              <ShareButton
                url={`/courses/${course.id}`}
                title={`Check out ${course.name}`}
                variant="ghost"
                size="icon"
                className="h-8 w-8"
              />
            </div>
          </div>
          <CardTitle className="line-clamp-2 text-base leading-snug transition-colors duration-300 group-hover:text-primary">
            {course.name}
          </CardTitle>
        </CardHeader>

        {/* Content - grows to fill space */}
        <CardContent className="pt-0 flex-1 flex flex-col">
          {/* Major & Badges */}
          <div className="space-y-2 mb-3">
            <p className="text-xs text-muted-foreground font-medium">
              {course.major}
            </p>
            <div className="flex flex-wrap gap-1">
              {course.badges.map((badge, index) => (
                <Badge
                  key={index}
                  variant={badge === "Obligatory" ? "default" : "outline"}
                  className="text-xs py-0 h-5"
                >
                  {badge}
                </Badge>
              ))}
            </div>
          </div>

          {/* Stats - Compact inline */}
          <div className="flex items-center gap-4 py-3 px-3 rounded-lg bg-muted/50 mb-3">
            <div className="flex items-center gap-2">
              <FileText className="h-4 w-4 text-primary" />
              <span className="text-sm font-semibold">{course.materials}</span>
              <span className="text-xs text-muted-foreground">Materials</span>
            </div>
            <div className="h-4 w-px bg-border" />
            <div className="flex items-center gap-2">
              <Trophy className="h-4 w-4 text-primary" />
              <span className="text-sm font-semibold">{course.quizzes}</span>
              <span className="text-xs text-muted-foreground">Quizzes</span>
            </div>
          </div>

          {/* Spacer to push buttons to bottom */}
          <div className="flex-1" />

          {/* Action Buttons - Always same structure */}
          <div className="grid grid-cols-2 gap-2 mt-auto">
            <Button
              className="transition-all duration-200 hover:shadow-md text-xs h-9 cursor-pointer"
              variant="outline"
              size="sm"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/courses/${course.id}/materials`);
              }}
            >
              <FileText className="h-3.5 w-3.5 mr-1" />
              Materials
            </Button>
            <Button
              className="transition-all duration-200 hover:shadow-md text-xs h-9 cursor-pointer"
              variant="outline"
              size="sm"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/courses/${course.id}/quizzes`);
              }}
            >
              <Trophy className="h-3.5 w-3.5 mr-1" />
              Quizzes
            </Button>
            {course.hasCourseInfo && (
              <Button
                className="col-span-2 transition-all duration-200 hover:shadow-md text-xs h-9 bg-primary text-primary-foreground hover:bg-primary/90 cursor-pointer"
                size="sm"
                onClick={(e) => {
                  e.stopPropagation();
                  navigate(`/courses/${course.id}/info`);
                }}
              >
                <Info className="h-3.5 w-3.5 mr-1" />
                Course Info
              </Button>
            )}
          </div>
        </CardContent>

        {/* Enrollment Dialog */}
        <Dialog open={isEnrollDialogOpen} onOpenChange={setIsEnrollDialogOpen}>
          <DialogContent onClick={(e) => e.stopPropagation()}>
            <DialogHeader>
              <DialogTitle>Enroll in {course.name}?</DialogTitle>
              <DialogDescription>
                By enrolling in this course, you'll be able to:
                <ul className="list-disc list-inside mt-2 space-y-1">
                  <li>Access the course from your profile easily</li>
                  <li>View it in your enrolled courses section</li>
                  <li>Track your progress and materials</li>
                </ul>
              </DialogDescription>
            </DialogHeader>
            <DialogFooter>
              <Button
                variant="outline"
                onClick={(e) => {
                  e.stopPropagation();
                  setIsEnrollDialogOpen(false);
                }}
                disabled={isEnrolling}
              >
                Cancel
              </Button>
              <Button
                onClick={(e) => {
                  e.stopPropagation();
                  handleEnroll();
                }}
                disabled={isEnrolling}
              >
                {isEnrolling ? (
                  <>
                    <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    Enrolling...
                  </>
                ) : (
                  <>
                    <UserPlus className="h-4 w-4 mr-2" />
                    Enroll
                  </>
                )}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </Card>
    );
  }

  // List View
  return (
    <Card
      className="group overflow-hidden transition-all duration-300 hover:shadow-xl cursor-pointer border hover:border-primary/50 bg-linear-to-r from-card via-card to-card/80"
      onClick={handleClick}
    >
      <div className="flex flex-col lg:flex-row lg:items-center p-5 gap-4">
        {/* Left: Course Info */}
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-3 mb-2 flex-wrap">
            <Badge
              variant="secondary"
              className="shrink-0 font-mono text-xs transition-colors group-hover:bg-primary/20"
            >
              {course.code}
            </Badge>
            <h3 className="text-lg font-semibold transition-colors duration-300 group-hover:text-primary line-clamp-1">
              {course.name}
            </h3>
            <div className="flex items-center gap-1">
              {isAuthenticated && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8 cursor-pointer"
                  onClick={(e) => {
                    e.stopPropagation();
                    setIsEnrollDialogOpen(true);
                  }}
                  title="Enroll in course"
                >
                  <UserPlus className="h-4 w-4" />
                </Button>
              )}
              <ShareButton
                url={`/courses/${course.id}`}
                title={`Check out ${course.name}`}
                variant="ghost"
                size="icon"
                className="h-8 w-8"
              />
            </div>
          </div>

          <p className="text-xs text-muted-foreground mb-2 font-medium">
            {course.major}
          </p>

          <div className="flex flex-wrap gap-1.5">
            {course.badges.map((badge, index) => (
              <Badge
                key={index}
                variant={badge === "Obligatory" ? "default" : "outline"}
                className="text-xs py-0 h-5"
              >
                {badge}
              </Badge>
            ))}
          </div>
        </div>

        {/* Middle: Stats */}
        <div className="flex gap-6 lg:px-6 lg:border-l lg:border-r border-border/50">
          <div className="flex items-center gap-2 text-sm">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 transition-all duration-300 group-hover:bg-primary/20">
              <FileText className="h-4 w-4 text-primary" />
            </div>
            <div>
              <div className="font-semibold">{course.materials}</div>
              <div className="text-xs text-muted-foreground">Materials</div>
            </div>
          </div>
          <div className="flex items-center gap-2 text-sm">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 transition-all duration-300 group-hover:bg-primary/20">
              <Trophy className="h-4 w-4 text-primary" />
            </div>
            <div>
              <div className="font-semibold">{course.quizzes}</div>
              <div className="text-xs text-muted-foreground">Quizzes</div>
            </div>
          </div>
        </div>

        {/* Right: Buttons */}
        <div className="flex gap-2 shrink-0 flex-wrap lg:flex-nowrap">
          <Button
            className="transition-all duration-200 hover:shadow-md cursor-pointer"
            variant="outline"
            size="sm"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/courses/${course.id}/materials`);
            }}
          >
            <FileText className="mr-1.5 h-4 w-4" />
            Materials
          </Button>

          <Button
            variant="outline"
            size="sm"
            className="transition-all duration-200 hover:shadow-md cursor-pointer"
            onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
              e.stopPropagation();
              navigate(`/courses/${course.id}/quizzes`);
            }}
          >
            <Trophy className="mr-1.5 h-4 w-4" />
            Quizzes
          </Button>

          {course.hasCourseInfo && (
            <Button
              size="sm"
              className="transition-all duration-200 hover:shadow-md bg-primary text-primary-foreground hover:bg-primary/90 cursor-pointer"
              onClick={(e: React.MouseEvent<HTMLButtonElement>) => {
                e.stopPropagation();
                navigate(`/courses/${course.id}/info`);
              }}
            >
              <Info className="mr-1.5 h-4 w-4" />
              Course Info
            </Button>
          )}
        </div>
      </div>

      {/* Enrollment Dialog */}
      <Dialog open={isEnrollDialogOpen} onOpenChange={setIsEnrollDialogOpen}>
        <DialogContent onClick={(e) => e.stopPropagation()}>
          <DialogHeader>
            <DialogTitle>Enroll in {course.name}?</DialogTitle>
            <DialogDescription>
              By enrolling in this course, you'll be able to:
              <ul className="list-disc list-inside mt-2 space-y-1">
                <li>Access the course from your profile easily</li>
                <li>View it in your enrolled courses section</li>
                <li>Track your progress and materials</li>
              </ul>
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={(e) => {
                e.stopPropagation();
                setIsEnrollDialogOpen(false);
              }}
              disabled={isEnrolling}
            >
              Cancel
            </Button>
            <Button
              onClick={(e) => {
                e.stopPropagation();
                handleEnroll();
              }}
              disabled={isEnrolling}
            >
              {isEnrolling ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Enrolling...
                </>
              ) : (
                <>
                  <UserPlus className="h-4 w-4 mr-2" />
                  Enroll
                </>
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </Card>
  );
}
