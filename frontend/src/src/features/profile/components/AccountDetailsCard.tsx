import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import {
  Calendar,
  Shield,
  CheckCircle2,
  ShieldCheck,
  Clock,
} from "lucide-react";
import type { User } from "@/app/providers/AuthContext";
import { formatDate, getRoleBadgeVariant } from "../utils/profileUtils";

interface AccountDetailsCardProps {
  user: User;
}

export const AccountDetailsCard = ({ user }: AccountDetailsCardProps) => {
  return (
    <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg hover:shadow-xl transition-shadow">
      {/* Gradient top border */}
      <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-violet-500 to-purple-500" />

      <CardHeader className="pb-3">
        <CardTitle className="flex items-center gap-3 text-lg font-semibold">
          <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-violet-500/10 to-purple-500/10 dark:from-violet-500/20 dark:to-purple-500/20">
            <Clock className="h-5 w-5 text-violet-600 dark:text-violet-400" />
          </div>
          Account Details
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {user.dateJoined && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <Calendar className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-0.5">
                Member Since
              </p>
              <p className="text-sm font-medium text-slate-900 dark:text-slate-100">
                {formatDate(user.dateJoined)}
              </p>
            </div>
          </div>
        )}
        {user.role && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <Shield className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-1">
                Role
              </p>
              <Badge
                variant="outline"
                className={`${getRoleBadgeVariant(user.role)} text-xs`}
              >
                {user.role === "User" ? "Student" : user.role}
              </Badge>
            </div>
          </div>
        )}
        {user.isActive !== undefined && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <CheckCircle2 className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-1">
                Account Status
              </p>
              <Badge
                variant="outline"
                className={`text-xs ${
                  user.isActive
                    ? "bg-green-500/10 text-green-700 dark:text-green-400 border-green-200 dark:border-green-800"
                    : "bg-gray-500/10 text-gray-700 dark:text-gray-400 border-gray-200 dark:border-gray-800"
                }`}
              >
                {user.isActive ? "Active" : "Inactive"}
              </Badge>
            </div>
          </div>
        )}
        {user.isVerified !== undefined && (
          <div className="group flex items-start gap-4 p-3 rounded-xl bg-slate-50 dark:bg-slate-800/50 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
            <div className="flex items-center justify-center h-9 w-9 rounded-lg bg-white dark:bg-slate-700 shadow-sm">
              <ShieldCheck className="h-4 w-4 text-slate-600 dark:text-slate-400" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-1">
                Verification Status
              </p>
              <Badge
                variant="outline"
                className={`text-xs ${
                  user.isVerified
                    ? "bg-blue-500/10 text-blue-700 dark:text-blue-400 border-blue-200 dark:border-blue-800"
                    : "bg-gray-500/10 text-gray-700 dark:text-gray-400 border-gray-200 dark:border-gray-800"
                }`}
              >
                {user.isVerified ? "Verified" : "Not Verified"}
              </Badge>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
