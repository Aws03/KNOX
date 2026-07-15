import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Hash, UserIcon, IdCard } from "lucide-react";
import type { User } from "@/app/providers/AuthContext";

interface UserIdentifiersCardProps {
  user: User;
}

export const UserIdentifiersCard = ({ user }: UserIdentifiersCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-base font-semibold">
          <IdCard className="h-4 w-4" />
          User Identifiers
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {user.id && (
          <div className="space-y-1">
            <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
              <Hash className="h-3 w-3" />
              User ID
            </div>
            <p className="text-sm font-mono text-slate-900 dark:text-slate-100 break-all pl-5">
              {user.id}
            </p>
          </div>
        )}
        {user.domainUserId && (
          <div className="space-y-1">
            <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
              <Hash className="h-3 w-3" />
              Domain User ID
            </div>
            <p className="text-sm font-mono text-slate-900 dark:text-slate-100 pl-5">
              {user.domainUserId}
            </p>
          </div>
        )}
        {user.name && (
          <div className="space-y-1">
            <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
              <UserIcon className="h-3 w-3" />
              Username
            </div>
            <p className="text-sm text-slate-900 dark:text-slate-100 pl-5">
              {user.name}
            </p>
          </div>
        )}
        {user.fullName && (
          <div className="space-y-1">
            <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
              <UserIcon className="h-3 w-3" />
              Full Name
            </div>
            <p className="text-sm text-slate-900 dark:text-slate-100 pl-5">
              {user.fullName}
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
