import { Pencil, School, Settings, PlusIcon } from "lucide-react";
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
import type { Faculty } from "../types";

interface FacultyTableProps {
  faculties: Faculty[];
  loading: boolean;
  searchTerm: string;
  universityName?: string;
  onFacultyClick: (facultyId: number) => void;
  onEditClick: (faculty: Faculty) => void;
  // onDeleteClick: (faculty: Faculty) => void;
  onAddClick: () => void;
}

const FacultyTableSkeleton = () => (
  <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
    {[...Array(6)].map((_, i) => (
      <Card key={i} className="overflow-hidden">
        <div className="h-32 animate-pulse bg-muted" />
      </Card>
    ))}
  </div>
);

const EmptyState = ({
  searchTerm,
  onAddClick,
}: {
  searchTerm: string;
  onAddClick: () => void;
}) => (
  <Card>
    <CardContent className="flex flex-col items-center justify-center py-16">
      <div className="flex size-20 items-center justify-center rounded-full bg-muted">
        <School className="size-10 text-muted-foreground" />
      </div>
      <h3 className="mt-4 text-lg font-semibold">No faculties found</h3>
      <p className="text-muted-foreground mt-2 text-center text-sm">
        {searchTerm
          ? "Try adjusting your search terms"
          : "Get started by adding your first faculty"}
      </p>
      {!searchTerm && (
        <Button onClick={onAddClick} className="mt-4">
          <PlusIcon className="mr-2 size-4" />
          Add Faculty
        </Button>
      )}
    </CardContent>
  </Card>
);

export const FacultyTable = ({
  faculties,
  loading,
  searchTerm,
  universityName,
  onFacultyClick,
  onEditClick,
  // onDeleteClick,
  onAddClick,
}: FacultyTableProps) => {
  if (loading) {
    return <FacultyTableSkeleton />;
  }

  if (faculties.length === 0) {
    return <EmptyState searchTerm={searchTerm} onAddClick={onAddClick} />;
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>All Faculties</CardTitle>
        <CardDescription>
          A list of all faculties in {universityName}
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px] py-4">ID</TableHead>
                <TableHead className="py-4">Faculty Name</TableHead>
                <TableHead className="py-4 text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {faculties.map((faculty) => (
                <TableRow key={faculty.id}>
                  <TableCell className="py-4 font-medium">
                    <Badge variant="outline">{faculty.id}</Badge>
                  </TableCell>
                  <TableCell className="py-4">
                    <div className="flex items-center gap-3">
                      <div className="flex size-9 items-center justify-center rounded-lg bg-primary/10">
                        <School className="size-4 text-primary" />
                      </div>
                      <span className="font-medium">{faculty.name}</span>
                    </div>
                  </TableCell>
                  <TableCell className="py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onFacultyClick(faculty.id)}
                      >
                        <Settings className="mr-2 size-4" />
                        Manage
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onEditClick(faculty)}
                      >
                        <Pencil className="mr-2 size-4" />
                        Edit
                      </Button>
                      {/* <Button
                        variant="ghost"
                        size="sm"
                        className="text-destructive hover:bg-destructive/10"
                        onClick={() => onDeleteClick(faculty)}
                      >
                        <Trash2 className="mr-2 size-4" />
                        Delete
                      </Button> */}
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
