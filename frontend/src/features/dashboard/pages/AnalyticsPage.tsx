import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  ActivityIcon,
  ArrowDownIcon,
  ArrowUpIcon,
  BookOpenIcon,
  BuildingIcon,
  ClipboardListIcon,
  FileTextIcon,
  GraduationCapIcon,
  TargetIcon,
  TrendingUpIcon,
  UsersIcon,
  ZapIcon,
} from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { Progress } from "@/shared/ui/progress";
import { Tabs, TabsList, TabsTrigger } from "@/shared/ui/tabs";
import { cn } from "@/lib/utils";

// Custom analytics components
import GrowthMetricsChart from "@/features/dashboard/componenets/GrowthMetricsChart";

// Analytics data structure
import type { AnalyticsData } from "../types";
import { fetchAnalyticsData } from "../api";

// Calculate growth percentages
const calculateGrowth = (data: { month: string; count: number }[]): number => {
  if (data.length < 2) return 0;
  const current = data[data.length - 1].count;
  const previous = data[data.length - 2].count;
  return parseFloat((((current - previous) / previous) * 100).toFixed(1));
};

// Hero Metric Card Component
interface HeroMetricProps {
  title: string;
  value: string | number;
  subtitle: string;
  icon: React.ReactNode;
  trend?: number;
  gradient: string;
  iconBg: string;
}

const HeroMetricCard = ({
  title,
  value,
  subtitle,
  icon,
  trend,
  gradient,
  iconBg,
}: HeroMetricProps) => (
  <Card className="group relative overflow-hidden border-0 shadow-lg transition-all duration-300 hover:shadow-xl hover:-translate-y-1">
    <div className={cn("absolute inset-0 opacity-5", gradient)} />
    <div className="absolute top-0 right-0 w-32 h-32 -mr-8 -mt-8 rounded-full opacity-10 bg-linear-to-br from-white to-transparent" />
    <CardContent className="relative p-6">
      <div className="flex items-start justify-between">
        <div className="space-y-3">
          <p className="text-sm font-medium text-muted-foreground uppercase tracking-wider">
            {title}
          </p>
          <div className="flex items-baseline gap-2">
            <h3 className="text-4xl font-bold tracking-tight">
              {typeof value === "number" ? value.toLocaleString() : value}
            </h3>
            {trend !== undefined && (
              <Badge
                variant={trend >= 0 ? "default" : "destructive"}
                className={cn(
                  "gap-1 font-semibold",
                  trend >= 0
                    ? "bg-emerald-500/15 text-emerald-600 hover:bg-emerald-500/20 border-emerald-500/20"
                    : "bg-red-500/15 text-red-600 hover:bg-red-500/20 border-red-500/20"
                )}
              >
                {trend >= 0 ? (
                  <ArrowUpIcon className="size-3" />
                ) : (
                  <ArrowDownIcon className="size-3" />
                )}
                {Math.abs(trend)}%
              </Badge>
            )}
          </div>
          <p className="text-sm text-muted-foreground">{subtitle}</p>
        </div>
        <div
          className={cn(
            "flex size-14 items-center justify-center rounded-2xl transition-transform duration-300 group-hover:scale-110",
            iconBg
          )}
        >
          {icon}
        </div>
      </div>
    </CardContent>
  </Card>
);

// Stats Card Component
interface StatsCardProps {
  icon: React.ReactNode;
  label: string;
  value: string | number;
  description?: string;
  color: string;
  bgColor: string;
}

const StatsCard = ({
  icon,
  label,
  value,
  description,
  color,
  bgColor,
}: StatsCardProps) => (
  <div className="group flex items-center gap-4 rounded-xl border bg-card/50 p-4 transition-all duration-200 hover:bg-card hover:shadow-md">
    <div
      className={cn(
        "flex size-12 shrink-0 items-center justify-center rounded-xl transition-transform duration-200 group-hover:scale-105",
        bgColor,
        color
      )}
    >
      {icon}
    </div>
    <div className="flex-1 min-w-0">
      <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
        {label}
      </p>
      <p className="text-2xl font-bold truncate">
        {typeof value === "number" ? value.toLocaleString() : value}
      </p>
      {description && (
        <p className="text-xs text-muted-foreground truncate">{description}</p>
      )}
    </div>
  </div>
);

// Engagement Ring Component
interface EngagementRingProps {
  value: number;
  max: number;
  label: string;
  color: string;
}

const EngagementRing = ({ value, max, label, color }: EngagementRingProps) => {
  const percentage = Math.min((value / max) * 100, 100);
  return (
    <div className="flex flex-col items-center gap-3">
      <div className="relative size-24">
        <svg className="size-24 -rotate-90" viewBox="0 0 36 36">
          <circle
            cx="18"
            cy="18"
            r="15.5"
            fill="none"
            className="stroke-muted"
            strokeWidth="2.5"
          />
          <circle
            cx="18"
            cy="18"
            r="15.5"
            fill="none"
            className={color}
            strokeWidth="2.5"
            strokeDasharray={`${percentage} 100`}
            strokeLinecap="round"
          />
        </svg>
        <div className="absolute inset-0 flex items-center justify-center">
          <span className="text-lg font-bold">{Math.round(percentage)}%</span>
        </div>
      </div>
      <span className="text-sm font-medium text-muted-foreground text-center">
        {label}
      </span>
    </div>
  );
};

// Quick Insight Card
interface QuickInsightProps {
  icon: React.ReactNode;
  title: string;
  value: string;
  subtext: string;
  accentColor: string;
}

const QuickInsightCard = ({
  icon,
  title,
  value,
  subtext,
  accentColor,
}: QuickInsightProps) => (
  <Card className="relative overflow-hidden border-0 bg-linear-to-br from-card to-muted/30 shadow-md">
    <div className={cn("absolute left-0 top-0 bottom-0 w-1", accentColor)} />
    <CardContent className="p-5">
      <div className="flex items-start gap-4">
        <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
          {icon}
        </div>
        <div className="flex-1 space-y-1">
          <p className="text-sm font-medium text-muted-foreground">{title}</p>
          <p className="text-2xl font-bold">{value}</p>
          <p className="text-xs text-muted-foreground">{subtext}</p>
        </div>
      </div>
    </CardContent>
  </Card>
);

const AnalyticsPage = () => {
  // Fetched on mount (i.e. once this route actually renders, which
  // ProtectedRoute already gates to SuperAdmin), not at module load time.
  const {
    data: analyticsData,
    isLoading,
    isError,
  } = useQuery<AnalyticsData>({
    queryKey: ["dashboard-statistics"],
    queryFn: fetchAnalyticsData,
  });

  // Calculate growth rates
  const usersGrowth = useMemo(
    () => (analyticsData ? calculateGrowth(analyticsData.usersGrowth) : 0),
    [analyticsData]
  );
  const quizzesGrowth = useMemo(
    () => (analyticsData ? calculateGrowth(analyticsData.quizzesGrowth) : 0),
    [analyticsData]
  );
  const materialsGrowth = useMemo(
    () => (analyticsData ? calculateGrowth(analyticsData.materialsGrowth) : 0),
    [analyticsData]
  );

  if (isLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <div className="text-lg text-muted-foreground">
          Loading analytics...
        </div>
      </div>
    );
  }

  if (isError || !analyticsData) {
    return (
      <div className="flex h-96 items-center justify-center">
        <div className="text-lg text-destructive">
          Failed to load analytics data.
        </div>
      </div>
    );
  }

  // Hero metrics data
  const heroMetrics: HeroMetricProps[] = [
    {
      title: "Total Users",
      value: analyticsData.totalUsers,
      subtitle: "Active learners on the platform",
      icon: <UsersIcon className="size-7 text-violet-600" />,
      trend: usersGrowth,
      gradient: "bg-gradient-to-br from-violet-500 to-purple-600",
      iconBg: "bg-violet-100 dark:bg-violet-500/20",
    },
    {
      title: "Total Quizzes",
      value: analyticsData.totalQuizzes,
      subtitle: "Assessments created",
      icon: <ClipboardListIcon className="size-7 text-blue-600" />,
      trend: quizzesGrowth,
      gradient: "bg-gradient-to-br from-blue-500 to-cyan-600",
      iconBg: "bg-blue-100 dark:bg-blue-500/20",
    },
    {
      title: "Total Materials",
      value: analyticsData.totalMaterials,
      subtitle: "Learning resources available",
      icon: <FileTextIcon className="size-7 text-emerald-600" />,
      trend: materialsGrowth,
      gradient: "bg-gradient-to-br from-emerald-500 to-teal-600",
      iconBg: "bg-emerald-100 dark:bg-emerald-500/20",
    },
    {
      title: "Quiz Attempts",
      value: analyticsData.activeQuizAttemptsLast30Days,
      subtitle: "In the last 30 days",
      icon: <ActivityIcon className="size-7 text-amber-600" />,
      gradient: "bg-gradient-to-br from-amber-500 to-orange-600",
      iconBg: "bg-amber-100 dark:bg-amber-500/20",
    },
  ];

  // Platform overview stats
  const platformStats: StatsCardProps[] = [
    {
      icon: <BuildingIcon className="size-5" />,
      label: "Universities",
      value: analyticsData.totalUniversities,
      description: "Partner institutions",
      color: "text-indigo-600",
      bgColor: "bg-indigo-100 dark:bg-indigo-500/20",
    },
    {
      icon: <GraduationCapIcon className="size-5" />,
      label: "Faculties",
      value: analyticsData.totalFaculties,
      description: "Academic departments",
      color: "text-pink-600",
      bgColor: "bg-pink-100 dark:bg-pink-500/20",
    },
    {
      icon: <BookOpenIcon className="size-5" />,
      label: "Majors",
      value: analyticsData.totalMajors,
      description: "Study programs",
      color: "text-cyan-600",
      bgColor: "bg-cyan-100 dark:bg-cyan-500/20",
    },
    {
      icon: <FileTextIcon className="size-5" />,
      label: "Courses",
      value: analyticsData.totalCourses,
      description: "Available courses",
      color: "text-orange-600",
      bgColor: "bg-orange-100 dark:bg-orange-500/20",
    },
  ];

  return (
    <div className="space-y-8">
      {/* Header Section */}
      <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div className="space-y-2">
          <div className="flex items-center gap-3">
            <h1 className="text-3xl font-bold tracking-tight md:text-4xl">
              Platform Analytics
            </h1>
            <div className="flex items-center mt-2">
              <Badge variant="outline" className="gap-1.5 py-1.5 px-3">
                <div className="size-2 rounded-full bg-emerald-500 animate-pulse" />
                Live Data
              </Badge>
            </div>
          </div>
          {/* <p className="text-muted-foreground max-w-2xl">
            Comprehensive insights into Uni-Hub's growth, engagement, and
            platform performance metrics.
          </p> */}
        </div>
      </div>

      {/* Hero Metrics Grid */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {heroMetrics.map((metric, index) => (
          <HeroMetricCard key={index} {...metric} />
        ))}
      </div>

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        {/* Platform Overview - Spans 2 columns */}
        <Card className="lg:col-span-2 shadow-lg border-0">
          <CardHeader className="border-b bg-muted/30">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="flex size-9 items-center justify-center rounded-lg bg-primary/10">
                  <BuildingIcon className="size-4 text-primary" />
                </div>
                <div>
                  <CardTitle className="text-lg">Platform Overview</CardTitle>
                  <p className="text-sm text-muted-foreground">
                    Institution & academic structure
                  </p>
                </div>
              </div>
            </div>
          </CardHeader>
          <CardContent className="p-6">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              {platformStats.map((stat, index) => (
                <StatsCard key={index} {...stat} />
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Engagement Metrics */}
        <Card className="shadow-lg border-0">
          <CardHeader className="border-b bg-muted/30">
            <div className="flex items-center gap-3">
              <div className="flex size-9 items-center justify-center rounded-lg bg-amber-500/10">
                <ZapIcon className="size-4 text-amber-600" />
              </div>
              <div>
                <CardTitle className="text-lg">Engagement</CardTitle>
                <p className="text-sm text-muted-foreground">
                  Platform activity metrics
                </p>
              </div>
            </div>
          </CardHeader>
          <CardContent className="p-6">
            <div className="flex items-center justify-around gap-4">
              <EngagementRing
                value={analyticsData.activeQuizAttemptsLast30Days}
                max={5000}
                label="Quiz Completion"
                color="stroke-violet-500"
              />
              <EngagementRing
                value={analyticsData.averageMaterialsPerCourse * 100}
                max={500}
                label="Resource Usage"
                color="stroke-emerald-500"
              />
            </div>
            <div className="mt-6 space-y-4">
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">
                  Avg. Quizzes per Course
                </span>
                <span className="font-semibold">
                  {analyticsData.averageQuizzesPerCourse.toFixed(2)}
                </span>
              </div>
              <Progress
                value={(analyticsData.averageQuizzesPerCourse / 3) * 100}
                className="h-2"
              />
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">
                  Avg. Materials per Course
                </span>
                <span className="font-semibold">
                  {analyticsData.averageMaterialsPerCourse.toFixed(2)}
                </span>
              </div>
              <Progress
                value={(analyticsData.averageMaterialsPerCourse / 5) * 100}
                className="h-2"
              />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Growth Chart Section */}
      <Card className="shadow-lg border-0 overflow-hidden">
        <CardHeader className="border-b bg-muted/30">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="flex size-9 items-center justify-center rounded-lg bg-blue-500/10">
                <TrendingUpIcon className="size-4 text-blue-600" />
              </div>
              <div>
                <CardTitle className="text-lg">Growth Analytics</CardTitle>
                <p className="text-sm text-muted-foreground">
                  6-month performance trends
                </p>
              </div>
            </div>
            <Tabs defaultValue="all" className="w-auto">
              <TabsList className="h-8">
                <TabsTrigger value="all" className="text-xs px-3">
                  All
                </TabsTrigger>
                <TabsTrigger value="quizzes" className="text-xs px-3">
                  Quizzes
                </TabsTrigger>
                <TabsTrigger value="materials" className="text-xs px-3">
                  Materials
                </TabsTrigger>
              </TabsList>
            </Tabs>
          </div>
        </CardHeader>
        <CardContent className="p-0">
          <GrowthMetricsChart
            quizzesGrowth={analyticsData.quizzesGrowth}
            materialsGrowth={analyticsData.materialsGrowth}
            usersGrowth={analyticsData.usersGrowth}
            className="border-0 shadow-none"
          />
        </CardContent>
      </Card>

      {/* Quick Insights Grid */}
      <div className="grid grid-cols-1 gap-5 md:grid-cols-3">
        <QuickInsightCard
          icon={<BookOpenIcon className="size-5 text-cyan-600" />}
          title="Total Majors"
          value={analyticsData.totalMajors.toString()}
          subtext="Across all partner universities"
          accentColor="bg-cyan-500"
        />
        <QuickInsightCard
          icon={<TargetIcon className="size-5 text-violet-600" />}
          title="Quiz to Course Ratio"
          value={analyticsData.averageQuizzesPerCourse.toFixed(2)}
          subtext="Average quizzes per course"
          accentColor="bg-violet-500"
        />
        <QuickInsightCard
          icon={<ActivityIcon className="size-5 text-emerald-600" />}
          title="Monthly Active Attempts"
          value={analyticsData.activeQuizAttemptsLast30Days.toLocaleString()}
          subtext="Quiz submissions this month"
          accentColor="bg-emerald-500"
        />
      </div>

      {/* Footer Stats Banner */}
      <Card className="border-0 bg-linear-to-r from-primary/5 via-primary/10 to-primary/5 shadow-lg">
        <CardContent className="py-6">
          <div className="flex flex-col items-center justify-center gap-6 md:flex-row md:justify-around">
            <div className="text-center">
              <p className="text-3xl font-bold text-primary">
                {(
                  analyticsData.totalQuizzes + analyticsData.totalMaterials
                ).toLocaleString()}
              </p>
              <p className="text-sm text-muted-foreground">
                Total Learning Resources
              </p>
            </div>
            <div className="hidden md:block h-12 w-px bg-border" />
            <div className="text-center">
              <p className="text-3xl font-bold text-primary">
                {analyticsData.totalUniversities}
              </p>
              <p className="text-sm text-muted-foreground">
                Partner Universities
              </p>
            </div>
            <div className="hidden md:block h-12 w-px bg-border" />
            <div className="text-center">
              <p className="text-3xl font-bold text-primary">
                {(
                  (analyticsData.totalQuizzes / analyticsData.totalCourses) *
                  100
                ).toFixed(0)}
                %
              </p>
              <p className="text-sm text-muted-foreground">
                Course Quiz Coverage
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default AnalyticsPage;
