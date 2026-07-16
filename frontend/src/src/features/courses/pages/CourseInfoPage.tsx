import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  FileText,
  Trophy,
  Loader2,
  AlertCircle,
  ExternalLink,
  Play,
  BookOpen,
  ArrowLeft,
  Gauge,
  Link as LinkIcon,
  Youtube,
  GraduationCap,
  FileEdit,
  Layers,
  Sparkles,
} from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { Separator } from "@/shared/ui/separator";
import ShareButton from "@/shared/ui/ShareButton";
import { fetchCourseInfo, fetchCourseById } from "@/features/courses/api";
import type {
  CourseInfo,
  CourseResource,
  ResourceType,
  CourseApiResponse,
} from "@/features/courses/types";

// Helper function to extract YouTube video ID
const getYouTubeVideoId = (url: string): string | null => {
  const patterns = [
    /(?:https?:\/\/)?(?:www\.)?youtube\.com\/watch\?v=([^&]+)/,
    /(?:https?:\/\/)?(?:www\.)?youtu\.be\/([^?]+)/,
    /(?:https?:\/\/)?(?:www\.)?youtube\.com\/embed\/([^?]+)/,
  ];

  for (const pattern of patterns) {
    const match = url.match(pattern);
    if (match) return match[1];
  }
  return null;
};

// YouTube Embed Component
const YouTubeEmbed = ({
  url,
  title,
  compact = false,
}: {
  url: string;
  title?: string;
  compact?: boolean;
}) => {
  const videoId = getYouTubeVideoId(url);

  if (!videoId) {
    return (
      <Button className="w-full" onClick={() => window.open(url, "_blank")}>
        <Play className="h-4 w-4 mr-2" />
        Watch Video
      </Button>
    );
  }

  return (
    <div
      className={`relative w-full rounded-lg overflow-hidden shadow-lg border-2 border-primary/20 hover:border-primary/40 transition-colors ${
        compact ? "aspect-video max-h-[180px]" : "aspect-video"
      }`}
    >
      <iframe
        src={`https://www.youtube.com/embed/${videoId}`}
        title={title || "Video"}
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowFullScreen
        className="absolute inset-0 w-full h-full"
      />
    </div>
  );
};

// Resource type display configuration
const RESOURCE_TYPE_CONFIG: Record<
  ResourceType,
  { label: string; icon: typeof Youtube; color: string }
> = {
  ECampusCourse: {
    label: "ECampus Course",
    icon: GraduationCap,
    color: "bg-blue-500",
  },
  YouTubeVideo: { label: "YouTube Video", icon: Youtube, color: "bg-red-500" },
  YouTubePlaylist: {
    label: "YouTube Playlist",
    icon: Youtube,
    color: "bg-red-600",
  },
  Article: { label: "Article", icon: FileEdit, color: "bg-green-500" },
  BlogPost: { label: "Blog Post", icon: FileEdit, color: "bg-green-600" },
  UdemyCourse: {
    label: "Udemy Course",
    icon: GraduationCap,
    color: "bg-purple-500",
  },
  CourseraCourse: {
    label: "Coursera Course",
    icon: GraduationCap,
    color: "bg-blue-600",
  },
  EdXCourse: {
    label: "edX Course",
    icon: GraduationCap,
    color: "bg-indigo-500",
  },
  LinkedInLearning: {
    label: "LinkedIn Learning",
    icon: GraduationCap,
    color: "bg-blue-700",
  },
  PluralSight: {
    label: "Pluralsight",
    icon: GraduationCap,
    color: "bg-pink-500",
  },
  OtherPlatformCourse: {
    label: "Online Course",
    icon: Layers,
    color: "bg-gray-500",
  },
  Other: { label: "Resource", icon: LinkIcon, color: "bg-gray-600" },
};

// Difficulty level configuration
const DIFFICULTY_CONFIG = {
  Easy: {
    label: "Easy",
    color: "bg-green-500 text-white",
    textColor: "text-green-600",
  },
  Moderate: {
    label: "Moderate",
    color: "bg-yellow-500 text-white",
    textColor: "text-yellow-600",
  },
  Hard: {
    label: "Hard",
    color: "bg-red-500 text-white",
    textColor: "text-red-600",
  },
};

// Resource Card Component
const ResourceCard = ({ resource }: { resource: CourseResource }) => {
  const config =
    RESOURCE_TYPE_CONFIG[resource.type] || RESOURCE_TYPE_CONFIG.Other;
  const IconComponent = config.icon;
  const isECampus = resource.type === "ECampusCourse";

  return (
    <Card
      className={`group overflow-hidden transition-all duration-300 hover:shadow-xl ${
        isECampus
          ? "border-2 border-blue-500/50 bg-linear-to-br from-blue-50 via-background to-background dark:from-blue-950/20 dark:via-background dark:to-background"
          : "hover:border-primary/50 border"
      }`}
    >
      <CardContent className="p-0">
        {/* Header section */}
        <div
          className={`p-4 ${
            isECampus
              ? "bg-linear-to-r from-blue-500/10 via-blue-500/5 to-transparent"
              : "bg-linear-to-r from-muted/50 to-transparent"
          }`}
        >
          <div className="flex items-start gap-3">
            {/* Icon */}
            <div
              className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-lg ${
                config.color
              } shadow-md transition-transform duration-300 group-hover:scale-105 ${
                isECampus ? "ring-2 ring-blue-400/50 ring-offset-2" : ""
              }`}
            >
              <IconComponent className="h-5 w-5 text-white" />
            </div>

            {/* Title & Badge */}
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1">
                <h4 className="font-semibold text-sm line-clamp-1 group-hover:text-primary transition-colors">
                  {resource.title}
                </h4>
                {isECampus && (
                  <Sparkles className="h-4 w-4 text-blue-500 shrink-0 animate-pulse" />
                )}
              </div>
              <Badge
                variant="secondary"
                className={`text-xs ${
                  isECampus ? "bg-blue-500 text-white hover:bg-blue-600" : ""
                }`}
              >
                {config.label}
              </Badge>
            </div>
          </div>
        </div>

        {/* Content section */}
        <div className="px-4 py-3 space-y-3">
          {resource.description && (
            <p className="text-sm text-muted-foreground line-clamp-2">
              {resource.description}
            </p>
          )}

          {/* Demo Video Frame - Compact */}
          {resource.hasDemonstrationVideo && resource.demonstrationVideoUrl && (
            <div className="space-y-1.5">
              <p className="text-xs font-medium text-muted-foreground flex items-center gap-1">
                <Play className="h-3 w-3" />
                Demo Preview
              </p>
              <YouTubeEmbed
                url={resource.demonstrationVideoUrl}
                title={`${resource.title} - Demo`}
                compact
              />
            </div>
          )}
        </div>

        {/* Action button - Full width at bottom */}
        <div className={`p-4 pt-0`}>
          <Button
            variant={isECampus ? "default" : "outline"}
            className={`w-full justify-center gap-2 font-medium transition-all ${
              isECampus
                ? "bg-blue-500 hover:bg-blue-600 text-white shadow-lg hover:shadow-xl"
                : "border-2 hover:border-primary hover:bg-primary hover:text-primary-foreground"
            }`}
            onClick={() => window.open(resource.url, "_blank")}
          >
            <ExternalLink className="h-4 w-4" />
            {isECampus ? "Open in ECampus" : "Visit Resource"}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
};

const CourseInfoPage = () => {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [courseInfo, setCourseInfo] = useState<CourseInfo | null>(null);
  const [course, setCourse] = useState<CourseApiResponse | null>(null);

  useEffect(() => {
    // Scroll to top when component mounts
    window.scrollTo(0, 0);

    const loadData = async () => {
      if (!courseId) {
        setError("Course ID is required");
        setLoading(false);
        return;
      }

      setLoading(true);
      setError(null);

      try {
        const [infoResponse, courseResponse] = await Promise.all([
          fetchCourseInfo(parseInt(courseId)),
          fetchCourseById(parseInt(courseId)),
        ]);
        setCourseInfo(infoResponse);
        setCourse(courseResponse);
      } catch (err) {
        console.error("Failed to fetch course info:", err);
        setError(
          err instanceof Error
            ? err.message
            : "Failed to load course information"
        );
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [courseId]);

  const difficultyConfig = courseInfo?.difficultyLevel
    ? DIFFICULTY_CONFIG[courseInfo.difficultyLevel]
    : null;

  return (
    <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
      <div className="container mx-auto px-4 py-6 sm:px-6 lg:px-8">
        {/* Back Button & Share */}
        <div className="flex items-center justify-between mb-4">
          <Button
            variant="ghost"
            className="-ml-2"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back
          </Button>
          {course && (
            <ShareButton
              url={window.location.href}
              title={`${course.courseName} - Course Info`}
              variant="outline"
              size="sm"
            />
          )}
        </div>

        {/* Loading State */}
        {loading && (
          <div className="flex flex-col items-center justify-center py-16 gap-3">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <p className="text-sm text-muted-foreground">
              Loading course information...
            </p>
          </div>
        )}

        {/* Error State */}
        {error && !loading && (
          <Card className="p-12 bg-linear-to-br from-destructive/5 to-destructive/10 border-destructive/20">
            <div className="flex flex-col items-center justify-center text-center">
              <div className="flex h-20 w-20 items-center justify-center rounded-full bg-destructive/10 mb-4">
                <AlertCircle className="h-10 w-10 text-destructive" />
              </div>
              <h3 className="text-lg font-semibold mb-2 text-destructive">
                Failed to load course information
              </h3>
              <p className="text-sm text-muted-foreground max-w-sm mb-4">
                {error}
              </p>
              <Button
                onClick={() => window.location.reload()}
                variant="outline"
              >
                Try Again
              </Button>
            </div>
          </Card>
        )}

        {/* Content */}
        {!loading && !error && courseInfo && course && (
          <div className="space-y-6">
            {/* Header Card */}
            <Card className="overflow-hidden border-2">
              <div className="bg-linear-to-r from-primary/10 via-primary/5 to-transparent p-6 sm:p-8">
                <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
                  {/* Course Title & Info */}
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-3 flex-wrap">
                      <Badge variant="secondary" className="font-mono text-sm">
                        {course.courseCode}
                      </Badge>
                      {difficultyConfig && (
                        <Badge className={difficultyConfig.color}>
                          <Gauge className="h-3 w-3 mr-1" />
                          {difficultyConfig.label}
                        </Badge>
                      )}
                      <Badge variant="outline">{course.credits} Credits</Badge>
                    </div>
                    <h1 className="text-2xl sm:text-3xl font-bold mb-2">
                      {course.courseName}
                    </h1>
                    {courseInfo.description && (
                      <p className="text-muted-foreground max-w-2xl">
                        {courseInfo.description}
                      </p>
                    )}
                  </div>

                  {/* Action Buttons */}
                  <div className="flex gap-3 shrink-0">
                    <Button
                      variant="outline"
                      className="transition-all hover:shadow-lg"
                      onClick={() => navigate(`/courses/${courseId}/materials`)}
                    >
                      <FileText className="h-4 w-4 mr-2" />
                      Materials
                      <Badge variant="secondary" className="ml-2">
                        {course.numberOfMaterials}
                      </Badge>
                    </Button>
                    <Button
                      variant="outline"
                      className="transition-all hover:shadow-lg"
                      onClick={() => navigate(`/courses/${courseId}/quizzes`)}
                    >
                      <Trophy className="h-4 w-4 mr-2" />
                      Quizzes
                      <Badge variant="secondary" className="ml-2">
                        {course.numberOfQuizzes}
                      </Badge>
                    </Button>
                  </div>
                </div>
              </div>
            </Card>

            {/* Demonstration Video */}
            {courseInfo.demonstrationVideoUrl && (
              <Card className="overflow-hidden border-2 border-primary/20">
                <CardHeader className="bg-linear-to-r from-primary/10 via-primary/5 to-transparent py-4">
                  <CardTitle className="flex items-center gap-2 text-lg">
                    <Play className="h-5 w-5 text-primary" />
                    Course Overview Video
                  </CardTitle>
                  {courseInfo.demonstrationVideoTitle && (
                    <p className="text-sm text-muted-foreground font-normal mt-1">
                      {courseInfo.demonstrationVideoTitle}
                    </p>
                  )}
                </CardHeader>
                <CardContent className="pt-4 pb-5">
                  <div className="max-w-2xl mx-auto">
                    <YouTubeEmbed
                      url={courseInfo.demonstrationVideoUrl}
                      title={
                        courseInfo.demonstrationVideoTitle || "Course Overview"
                      }
                    />
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Resources Section */}
            {courseInfo.resources && courseInfo.resources.length > 0 && (
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <h2 className="text-xl font-semibold flex items-center gap-2">
                    <BookOpen className="h-5 w-5 text-primary" />
                    Learning Resources
                    <Badge variant="secondary" className="ml-1">
                      {courseInfo.resourceCount}
                    </Badge>
                  </h2>
                </div>

                <Separator />

                <div className="grid gap-4 sm:grid-cols-2">
                  {courseInfo.resources.map((resource) => (
                    <ResourceCard key={resource.id} resource={resource} />
                  ))}
                </div>
              </div>
            )}

            {/* No Resources State */}
            {(!courseInfo.resources || courseInfo.resources.length === 0) && (
              <Card className="p-12 bg-linear-to-br from-card to-muted/20">
                <div className="flex flex-col items-center justify-center text-center">
                  <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
                    <BookOpen className="h-10 w-10 text-primary" />
                  </div>
                  <h3 className="text-lg font-semibold mb-2">
                    No additional resources yet
                  </h3>
                  <p className="text-sm text-muted-foreground max-w-sm">
                    Check back later for learning resources and helpful links
                    for this course.
                  </p>
                </div>
              </Card>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default CourseInfoPage;
