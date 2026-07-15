import { LayoutGrid, List } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { PageSizeSelector } from "@/shared/components/pagination/PageSizeSelector";

interface QuizzesToolbarProps {
  totalCount: number;
  currentPage: number;
  pageSize: number;
  viewMode: "grid" | "list";
  onViewModeChange: (mode: "grid" | "list") => void;
  onPageSizeChange: (size: number) => void;
}

export const QuizzesToolbar = ({
  totalCount,
  currentPage,
  pageSize,
  viewMode,
  onViewModeChange,
  onPageSizeChange,
}: QuizzesToolbarProps) => {
  return (
    <div className="mb-4 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-4">
        <p className="text-sm text-muted-foreground">
          {totalCount > 0 ? (
            <>
              Showing{" "}
              <span className="font-semibold text-foreground">
                {(currentPage - 1) * pageSize + 1}-
                {Math.min(currentPage * pageSize, totalCount)}
              </span>{" "}
              of{" "}
              <span className="font-semibold text-foreground">
                {totalCount}
              </span>{" "}
              quizzes
            </>
          ) : (
            "No quizzes to display"
          )}
        </p>
        <PageSizeSelector pageSize={pageSize} onChange={onPageSizeChange} />
      </div>

      <div className="flex items-center rounded-lg border p-1 bg-background shadow-sm">
        <Button
          variant={viewMode === "grid" ? "secondary" : "ghost"}
          size="sm"
          onClick={() => onViewModeChange("grid")}
          className="h-8 px-3"
        >
          <LayoutGrid className="h-4 w-4" />
        </Button>
        <Button
          variant={viewMode === "list" ? "secondary" : "ghost"}
          size="sm"
          onClick={() => onViewModeChange("list")}
          className="h-8 px-3"
        >
          <List className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );
};
