import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { GraduationCap, Building2, Award, BookOpen } from "lucide-react";
import type { User } from "@/app/providers/AuthContext";

interface AcademicInfoCardProps {
  user: User;
}

export const AcademicInfoCard = ({ user }: AcademicInfoCardProps) => {
  if (!user.universityName && !user.facultyName && !user.majorName) {
    return (
      <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg hover:shadow-xl transition-shadow">
        <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-amber-500 to-orange-500" />
        <CardHeader className="pb-3">
          <CardTitle className="flex items-center gap-3 text-lg font-semibold">
            <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-amber-500/10 to-orange-500/10 dark:from-amber-500/20 dark:to-orange-500/20">
              <GraduationCap className="h-5 w-5 text-amber-600 dark:text-amber-400" />
            </div>
            Academic Information
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col items-center justify-center py-8 text-center">
            <GraduationCap className="h-12 w-12 text-slate-300 dark:text-slate-700 mb-4" />
            <p className="text-sm text-slate-500 dark:text-slate-400">
              No academic information added yet
            </p>
            <p className="text-xs text-slate-400 dark:text-slate-500 mt-1">
              Add your university, faculty, and major in profile settings
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg hover:shadow-xl transition-shadow">
      {/* Gradient top border */}
      <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-amber-500 to-orange-500" />

      <CardHeader className="pb-3">
        <CardTitle className="flex items-center gap-3 text-lg font-semibold">
          <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-amber-500/10 to-orange-500/10 dark:from-amber-500/20 dark:to-orange-500/20">
            <GraduationCap className="h-5 w-5 text-amber-600 dark:text-amber-400" />
          </div>
          Academic Information
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {user.universityName && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <Building2 className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-0.5">
                University
              </p>
              <p className="text-sm font-medium text-slate-900 dark:text-slate-100 truncate">
                {user.universityName}
              </p>
            </div>
          </div>
        )}
        {user.facultyName && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <Award className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-0.5">
                Faculty
              </p>
              <p className="text-sm font-medium text-slate-900 dark:text-slate-100 truncate">
                {user.facultyName}
              </p>
            </div>
          </div>
        )}
        {user.majorName && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <BookOpen className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-0.5">
                Major
              </p>
              <p className="text-sm font-medium text-slate-900 dark:text-slate-100 truncate">
                {user.majorName}
              </p>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
