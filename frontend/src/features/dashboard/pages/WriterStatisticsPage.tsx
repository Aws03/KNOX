import {
  BarChart3,
  FileText,
  Clock,
  TrendingUp,
  Award,
  BookOpen,
} from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";

const WriterStatisticsPage = () => {
  // Mock statistics data
  const stats = {
    totalMaterials: 156,
    totalQuizzes: 42,
    thisMonth: 23,
    avgRating: 4.7,
    totalViews: 12543,
    activeHours: 127,
  };

  const recentActivity = [
    {
      id: 1,
      type: "Material",
      title: "Data Structures - Introduction to Trees",
      date: "2 hours ago",
      views: 145,
    },
    {
      id: 2,
      type: "Quiz",
      title: "Object-Oriented Programming Quiz #5",
      date: "5 hours ago",
      views: 89,
    },
    {
      id: 3,
      type: "Material",
      title: "Database Design Best Practices",
      date: "1 day ago",
      views: 234,
    },
    {
      id: 4,
      type: "Material",
      title: "Advanced JavaScript Concepts",
      date: "2 days ago",
      views: 312,
    },
  ];

  const topContent = [
    { title: "React Hooks Deep Dive", views: 1847, type: "Material" },
    { title: "Python Final Exam Practice", views: 1523, type: "Quiz" },
    { title: "Web Security Fundamentals", views: 1401, type: "Material" },
    { title: "Algorithms Mid-term Quiz", views: 1276, type: "Quiz" },
    { title: "Cloud Computing Introduction", views: 1198, type: "Material" },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="space-y-1">
          <div className="flex items-center gap-3">
            <div className="flex size-12 items-center justify-center rounded-lg bg-primary/10">
              <BarChart3 className="size-6 text-primary" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight">
                Writer Statistics
              </h1>
              <p className="text-muted-foreground text-sm">
                Track your content performance and activity
              </p>
            </div>
          </div>
        </div>
        <Badge variant="secondary" className="w-fit px-3 py-1.5">
          Last updated: Just now
        </Badge>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Total Materials
            </CardTitle>
            <FileText className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalMaterials}</div>
            <p className="text-xs text-muted-foreground mt-1">
              +12 from last month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Quizzes</CardTitle>
            <BookOpen className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalQuizzes}</div>
            <p className="text-xs text-muted-foreground mt-1">
              +5 from last month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">This Month</CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.thisMonth}</div>
            <p className="text-xs text-muted-foreground mt-1">
              Content published
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Average Rating
            </CardTitle>
            <Award className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.avgRating}/5.0</div>
            <p className="text-xs text-muted-foreground mt-1">
              Based on 1,234 ratings
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Views</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {stats.totalViews.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              +2,350 from last month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Hours</CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.activeHours}h</div>
            <p className="text-xs text-muted-foreground mt-1">This month</p>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        {/* Recent Activity */}
        <Card>
          <CardHeader>
            <CardTitle>Recent Activity</CardTitle>
            <CardDescription>Your latest published content</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {recentActivity.map((item) => (
                <div
                  key={item.id}
                  className="flex items-start justify-between gap-4 border-b pb-4 last:border-0 last:pb-0"
                >
                  <div className="space-y-1 flex-1 min-w-0">
                    <div className="flex items-center gap-2">
                      <Badge
                        variant={
                          item.type === "Material" ? "default" : "secondary"
                        }
                        className="text-xs"
                      >
                        {item.type}
                      </Badge>
                    </div>
                    <p className="font-medium text-sm truncate">{item.title}</p>
                    <p className="text-xs text-muted-foreground">{item.date}</p>
                  </div>
                  <div className="text-right shrink-0">
                    <p className="text-sm font-semibold">{item.views}</p>
                    <p className="text-xs text-muted-foreground">views</p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Top Content */}
        <Card>
          <CardHeader>
            <CardTitle>Top Performing Content</CardTitle>
            <CardDescription>Most viewed this month</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {topContent.map((item, index) => (
                <div
                  key={index}
                  className="flex items-center gap-4 border-b pb-4 last:border-0 last:pb-0"
                >
                  <div className="flex items-center justify-center size-8 rounded-full bg-primary/10 font-bold text-primary shrink-0">
                    {index + 1}
                  </div>
                  <div className="space-y-1 flex-1 min-w-0">
                    <p className="font-medium text-sm truncate">{item.title}</p>
                    <div className="flex items-center gap-2">
                      <Badge
                        variant={
                          item.type === "Material" ? "default" : "secondary"
                        }
                        className="text-xs"
                      >
                        {item.type}
                      </Badge>
                      <p className="text-xs text-muted-foreground">
                        {item.views.toLocaleString()} views
                      </p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Performance Chart Placeholder */}
      <Card>
        <CardHeader>
          <CardTitle>Performance Overview</CardTitle>
          <CardDescription>Views and engagement over time</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-64 bg-muted/30 rounded-lg border-2 border-dashed">
            <div className="text-center space-y-2">
              <BarChart3 className="size-12 mx-auto text-muted-foreground" />
              <p className="text-sm text-muted-foreground font-medium">
                Chart visualization coming soon
              </p>
              <p className="text-xs text-muted-foreground">
                Performance metrics will be displayed here
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default WriterStatisticsPage;
