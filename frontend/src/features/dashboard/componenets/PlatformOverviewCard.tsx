import {
  BookOpenIcon,
  ClipboardListIcon,
  FileTextIcon,
  GraduationCapIcon,
} from "lucide-react";

import { Card, CardContent, CardHeader } from "@/shared/ui/card";
import { cn } from "@/lib/utils";

type Props = {
  totalMajors: number;
  totalCourses: number;
  totalQuizzes: number;
  totalMaterials: number;
  className?: string;
};

const PlatformOverviewCard = ({
  totalMajors,
  totalCourses,
  totalQuizzes,
  totalMaterials,
  className,
}: Props) => {
  const stats = [
    {
      icon: <GraduationCapIcon className="size-4" />,
      label: "Majors",
      value: totalMajors.toLocaleString(),
      color: "text-blue-500",
      bgColor: "bg-blue-500/10",
    },
    {
      icon: <BookOpenIcon className="size-4" />,
      label: "Courses",
      value: totalCourses.toLocaleString(),
      color: "text-green-500",
      bgColor: "bg-green-500/10",
    },
    {
      icon: <ClipboardListIcon className="size-4" />,
      label: "Quizzes",
      value: totalQuizzes.toLocaleString(),
      color: "text-purple-500",
      bgColor: "bg-purple-500/10",
    },
    {
      icon: <FileTextIcon className="size-4" />,
      label: "Materials",
      value: totalMaterials.toLocaleString(),
      color: "text-orange-500",
      bgColor: "bg-orange-500/10",
    },
  ];

  return (
    <Card className={cn("gap-4", className)}>
      <CardHeader className="flex justify-between">
        <div className="flex flex-col gap-1">
          <span className="text-lg font-semibold">Platform Overview</span>
          <span className="text-muted-foreground text-sm">
            Key metrics across the platform
          </span>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          {stats.map((stat, index) => (
            <div
              key={index}
              className="flex items-center gap-3 rounded-md border p-4"
            >
              <div
                className={cn(
                  "flex size-10 items-center justify-center rounded-md",
                  stat.bgColor,
                  stat.color
                )}
              >
                {stat.icon}
              </div>
              <div className="flex flex-col gap-0.5">
                <span className="text-muted-foreground text-xs font-medium">
                  {stat.label}
                </span>
                <span className="text-xl font-bold">{stat.value}</span>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};

export default PlatformOverviewCard;
