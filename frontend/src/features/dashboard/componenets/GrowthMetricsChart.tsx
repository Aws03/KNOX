"use client";

import { TrendingUpIcon } from "lucide-react";
import { Area, AreaChart, CartesianGrid, XAxis } from "recharts";

import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import {
  type ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/shared/ui/chart";

type GrowthData = {
  month: string;
  count: number;
};

type Props = {
  quizzesGrowth: GrowthData[];
  materialsGrowth: GrowthData[];
  usersGrowth: GrowthData[];
  className?: string;
};

const chartConfig = {
  quizzes: {
    label: "Quizzes",
    color: "#8b5cf6",
  },
  materials: {
    label: "Materials",
    color: "#3b82f6",
  },
  users: {
    label: "Users",
    color: "#10b981",
  },
} satisfies ChartConfig;

const GrowthMetricsChart = ({
  quizzesGrowth,
  materialsGrowth,
  usersGrowth,
  className,
}: Props) => {
  // Transform data for the chart
  const chartData = quizzesGrowth.map((quiz, index) => {
    const monthDate = new Date(quiz.month);
    const monthName = monthDate.toLocaleDateString("en-US", { month: "short" });

    return {
      month: monthName,
      quizzes: quiz.count,
      materials: materialsGrowth[index]?.count || 0,
      users: usersGrowth[index]?.count || 0,
    };
  });

  // Calculate total growth
  const totalQuizzes = quizzesGrowth.reduce((sum, item) => sum + item.count, 0);
  const totalMaterials = materialsGrowth.reduce(
    (sum, item) => sum + item.count,
    0
  );
  const totalUsers = usersGrowth.reduce((sum, item) => sum + item.count, 0);

  // Calculate growth percentage (comparing last month to previous)
  const quizGrowth =
    quizzesGrowth.length >= 2
      ? (
          ((quizzesGrowth[quizzesGrowth.length - 1].count -
            quizzesGrowth[quizzesGrowth.length - 2].count) /
            quizzesGrowth[quizzesGrowth.length - 2].count) *
          100
        ).toFixed(1)
      : "0";

  return (
    <Card className={className}>
      <CardHeader>
        <CardTitle>Growth Trends</CardTitle>
        <CardDescription>
          Showing growth metrics for the last 6 months
        </CardDescription>
      </CardHeader>
      <CardContent>
        <ChartContainer config={chartConfig}>
          <AreaChart
            accessibilityLayer
            data={chartData}
            margin={{
              left: 12,
              right: 12,
            }}
          >
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey="month"
              tickLine={false}
              axisLine={false}
              tickMargin={8}
            />
            <ChartTooltip cursor={false} content={<ChartTooltipContent />} />
            <defs>
              <linearGradient id="fillQuizzes" x1="0" y1="0" x2="0" y2="1">
                <stop
                  offset="5%"
                  stopColor="var(--color-quizzes)"
                  stopOpacity={0.8}
                />
                <stop
                  offset="95%"
                  stopColor="var(--color-quizzes)"
                  stopOpacity={0.1}
                />
              </linearGradient>
              <linearGradient id="fillMaterials" x1="0" y1="0" x2="0" y2="1">
                <stop
                  offset="5%"
                  stopColor="var(--color-materials)"
                  stopOpacity={0.8}
                />
                <stop
                  offset="95%"
                  stopColor="var(--color-materials)"
                  stopOpacity={0.1}
                />
              </linearGradient>
              <linearGradient id="fillUsers" x1="0" y1="0" x2="0" y2="1">
                <stop
                  offset="5%"
                  stopColor="var(--color-users)"
                  stopOpacity={0.8}
                />
                <stop
                  offset="95%"
                  stopColor="var(--color-users)"
                  stopOpacity={0.1}
                />
              </linearGradient>
            </defs>
            <Area
              dataKey="materials"
              type="natural"
              fill="url(#fillMaterials)"
              fillOpacity={0.4}
              stroke="var(--color-materials)"
              stackId="a"
            />
            <Area
              dataKey="users"
              type="natural"
              fill="url(#fillUsers)"
              fillOpacity={0.4}
              stroke="var(--color-users)"
              stackId="a"
            />
            <Area
              dataKey="quizzes"
              type="natural"
              fill="url(#fillQuizzes)"
              fillOpacity={0.4}
              stroke="var(--color-quizzes)"
              stackId="a"
            />
          </AreaChart>
        </ChartContainer>
      </CardContent>
      <CardFooter>
        <div className="flex w-full items-start gap-2 text-sm">
          <div className="grid gap-2">
            <div className="flex items-center gap-2 font-medium leading-none">
              Trending {parseFloat(quizGrowth) >= 0 ? "up" : "down"} by{" "}
              {Math.abs(parseFloat(quizGrowth))}% this month{" "}
              <TrendingUpIcon className="h-4 w-4" />
            </div>
            <div className="flex items-center gap-2 leading-none text-muted-foreground">
              Total: {totalQuizzes.toLocaleString()} quizzes,{" "}
              {totalMaterials.toLocaleString()} materials,{" "}
              {totalUsers.toLocaleString()} users
            </div>
          </div>
        </div>
      </CardFooter>
    </Card>
  );
};

export default GrowthMetricsChart;
