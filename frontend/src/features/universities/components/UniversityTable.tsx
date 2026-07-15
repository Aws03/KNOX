import { Pencil, Building2, Settings } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/ui/table";
import type { University } from "../types";

interface UniversityTableProps {
  universities: University[];
  loading: boolean;
  searchTerm: string;
  onManageClick: (universityId: number) => void;
  onEditClick: (university: University) => void;
  // onDeleteClick: (university: University) => void;
}

const UniversityTableSkeleton = () => (
  <div className="space-y-2">
    {[...Array(5)].map((_, i) => (
      <div key={i} className="h-16 animate-pulse rounded bg-muted" />
    ))}
  </div>
);

const EmptyState = ({ searchTerm }: { searchTerm: string }) => (
  <div className="flex flex-col items-center justify-center py-16">
    <div className="flex size-20 items-center justify-center rounded-full bg-muted">
      <Building2 className="size-10 text-muted-foreground" />
    </div>
    <h3 className="mt-4 text-lg font-semibold">No universities found</h3>
    <p className="text-muted-foreground mt-2 text-center text-sm">
      {searchTerm
        ? "Try adjusting your search terms"
        : "Get started by adding your first university"}
    </p>
  </div>
);

export const UniversityTable = ({
  universities,
  loading,
  searchTerm,
  onManageClick,
  onEditClick,
}: // onDeleteClick,
UniversityTableProps) => {
  if (loading) {
    return <UniversityTableSkeleton />;
  }

  if (universities.length === 0) {
    return <EmptyState searchTerm={searchTerm} />;
  }

  return (
    <div className="overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="w-[100px]">ID</TableHead>
            <TableHead>University Name</TableHead>
            <TableHead className="text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {universities.map((university) => (
            <TableRow key={university.id}>
              <TableCell className="font-medium">
                <Badge variant="outline">{university.id}</Badge>
              </TableCell>
              <TableCell>
                <div className="flex items-center gap-3">
                  <div className="flex size-9 items-center justify-center rounded-lg bg-primary/10">
                    <Building2 className="size-4 text-primary" />
                  </div>
                  <span className="font-medium">{university.name}</span>
                </div>
              </TableCell>
              <TableCell className="text-right">
                <div className="flex justify-end gap-2">
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => onManageClick(university.id)}
                  >
                    <Settings className="mr-2 size-4" />
                    Manage
                  </Button>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => onEditClick(university)}
                  >
                    <Pencil className="mr-2 size-4" />
                    Edit
                  </Button>
                  {/* <Button
                    variant="ghost"
                    size="sm"
                    className="text-destructive hover:bg-destructive/10"
                    onClick={() => onDeleteClick(university)}
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
  );
};
