import {
  GraduationCap,
  Pencil,
  Trash2,
  PlusIcon,
  ArrowRight,
  FolderTree,
} from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/ui/table";
import type { Major } from "../types";

interface MajorTableProps {
  majors: Major[];
  loading: boolean;
  searchTerm: string;
  facultyName?: string;
  onMajorClick: (majorId: number) => void;
  onEditClick: (major: Major) => void;
  onDeleteClick?: (major: Major) => void;
  onAddClick: () => void;
  showActions?: boolean;
}

export const MajorTable = ({
  majors,
  loading,
  searchTerm,
  facultyName,
  onMajorClick,
  onEditClick,
  onDeleteClick,
  onAddClick,
  showActions = true,
}: MajorTableProps) => {
  if (loading) {
    return (
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {[...Array(6)].map((_, i) => (
          <Card key={i} className="overflow-hidden">
            <div className="h-32 animate-pulse bg-muted" />
          </Card>
        ))}
      </div>
    );
  }

  if (majors.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-16">
          <div className="flex size-20 items-center justify-center rounded-full bg-muted">
            <GraduationCap className="size-10 text-muted-foreground" />
          </div>
          <h3 className="mt-4 text-lg font-semibold">No majors found</h3>
          <p className="text-muted-foreground mt-2 text-center text-sm">
            {searchTerm
              ? "Try adjusting your search terms"
              : showActions
              ? "Get started by adding your first major"
              : "No majors available in this faculty"}
          </p>
          {!searchTerm && showActions && (
            <Button onClick={onAddClick} className="mt-4">
              <PlusIcon className="mr-2 size-4" />
              Add Major
            </Button>
          )}
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>All Majors</CardTitle>
        <CardDescription>
          A list of all majors{facultyName && ` in ${facultyName}`}
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px] py-4">ID</TableHead>
                <TableHead className="py-4">Major Name</TableHead>
                <TableHead className="py-4 text-right">
                  {showActions ? "Actions" : "Manage"}
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {majors.map((major) => (
                <TableRow
                  key={major.id}
                  className="group cursor-pointer hover:bg-accent/50 transition-all duration-200"
                  onClick={() => onMajorClick(major.id)}
                >
                  <TableCell className="py-4 font-medium">
                    <Badge variant="outline" className="text-xs">
                      {major.id}
                    </Badge>
                  </TableCell>
                  <TableCell className="py-4">
                    <div className="flex items-center gap-3">
                      <div className="flex size-10 items-center justify-center rounded-lg bg-primary/10 group-hover:bg-primary/20 transition-colors">
                        <GraduationCap className="size-5 text-primary" />
                      </div>
                      <div className="flex flex-col">
                        <span className="font-semibold text-base group-hover:text-primary transition-colors">
                          {major.name}
                        </span>
                        <span className="text-xs text-muted-foreground">
                          Click to manage courses
                        </span>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell className="py-4 text-right">
                    {showActions ? (
                      <div
                        className="flex justify-end gap-2"
                        onClick={(e) => e.stopPropagation()}
                      >
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => onEditClick(major)}
                          className="hover:bg-primary/10"
                        >
                          <Pencil className="mr-2 size-4" />
                          Edit
                        </Button>
                        {onDeleteClick && (
                          <Button
                            variant="ghost"
                            size="sm"
                            className="text-destructive hover:bg-destructive/10"
                            onClick={() => onDeleteClick(major)}
                          >
                            <Trash2 className="mr-2 size-4" />
                            Delete
                          </Button>
                        )}
                      </div>
                    ) : (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="group-hover:bg-primary/10 group-hover:text-primary"
                        onClick={() => onMajorClick(major.id)}
                      >
                        <FolderTree className="mr-2 size-4" />
                        Manage
                        <ArrowRight className="ml-2 size-4 group-hover:translate-x-1 transition-transform" />
                      </Button>
                    )}
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
