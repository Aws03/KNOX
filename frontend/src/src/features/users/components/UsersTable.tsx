import {
  UserCheck,
  UserX,
  Shield,
  CheckCircle2,
  XCircle,
  Mail,
} from "lucide-react";
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
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/shared/ui/tooltip";
import type { User } from "../types";

type UsersTableProps = {
  users: User[];
  loading: boolean;
  onAssignRole: (user: User) => void;
  onToggleStatus: (user: User) => void;
};

export const UsersTable = ({
  users,
  loading,
  onAssignRole,
  onToggleStatus,
}: UsersTableProps) => {
  if (loading) {
    return (
      <div className="grid gap-4">
        {[...Array(3)].map((_, i) => (
          <Card key={i} className="overflow-hidden">
            <div className="h-24 animate-pulse bg-muted" />
          </Card>
        ))}
      </div>
    );
  }

  if (users.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-16">
          <div className="flex size-20 items-center justify-center rounded-full bg-muted">
            <UserCheck className="size-10 text-muted-foreground" />
          </div>
          <h3 className="mt-4 text-lg font-semibold">No users found</h3>
          <p className="text-muted-foreground mt-2 text-center text-sm">
            Try adjusting your search or filters
          </p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>All Users ({users.length})</CardTitle>
        <CardDescription>Manage user accounts and permissions</CardDescription>
      </CardHeader>
      <CardContent className="p-0">
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-20 py-4 pl-6">ID</TableHead>
                <TableHead className="min-w-[150px] py-4">Name</TableHead>
                <TableHead className="min-w-[200px] py-4">Email</TableHead>
                <TableHead className="min-w-[180px] py-4">Academic</TableHead>
                <TableHead className="min-w-[120px] py-4">Status</TableHead>
                <TableHead className="min-w-[180px] py-4 pr-6 text-right">
                  Actions
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.id}>
                  <TableCell className="py-4 pl-6 font-medium">
                    <Badge variant="outline">{user.id}</Badge>
                  </TableCell>
                  <TableCell className="py-4 max-w-[150px]">
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <div className="font-medium truncate cursor-default">
                            {user.name.value}
                          </div>
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>{user.name.value}</p>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  </TableCell>
                  <TableCell className="py-4 max-w-[200px]">
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <div className="flex items-center gap-2 text-sm text-muted-foreground cursor-default">
                            <Mail className="size-3.5 shrink-0" />
                            <span className="truncate">
                              {user.email.address}
                            </span>
                          </div>
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>{user.email.address}</p>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  </TableCell>
                  <TableCell className="py-4 max-w-[180px]">
                    <div className="space-y-0.5 text-xs">
                      <div className="font-medium truncate">
                        {user.majorName}
                      </div>
                      <div className="text-muted-foreground truncate text-[11px]">
                        {user.facultyName}
                      </div>
                      <div className="text-muted-foreground truncate text-[11px]">
                        {user.universityName}
                      </div>
                    </div>
                  </TableCell>
                  <TableCell className="py-4">
                    <div className="flex flex-col gap-1.5">
                      <Badge
                        variant={user.isActive ? "default" : "secondary"}
                        className="w-fit"
                      >
                        {user.isActive ? (
                          <>
                            <CheckCircle2 className="mr-1 size-3" />
                            Active
                          </>
                        ) : (
                          <>
                            <XCircle className="mr-1 size-3" />
                            Blocked
                          </>
                        )}
                      </Badge>
                      {user.isVerified && (
                        <Badge variant="outline" className="w-fit">
                          <CheckCircle2 className="mr-1 size-3" />
                          Verified
                        </Badge>
                      )}
                    </div>
                  </TableCell>
                  <TableCell className="py-4 pr-6 text-right">
                    <div className="flex justify-end gap-2 whitespace-nowrap">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onAssignRole(user)}
                      >
                        <Shield className="mr-2 size-4" />
                        Role
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        className={
                          user.isActive
                            ? "text-destructive hover:bg-destructive/10"
                            : "text-green-600 hover:bg-green-50 dark:hover:bg-green-950"
                        }
                        onClick={() => onToggleStatus(user)}
                      >
                        {user.isActive ? (
                          <>
                            <UserX className="mr-2 size-4" />
                            Block
                          </>
                        ) : (
                          <>
                            <UserCheck className="mr-2 size-4" />
                            Activate
                          </>
                        )}
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
};
