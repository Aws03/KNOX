import type { ReactNode } from "react";
import { Card, CardContent, CardHeader } from "@/shared/ui/card";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Plus, Search } from "lucide-react";

type CRUDPageLayoutProps = {
  title: string;
  description?: string;
  onAdd?: () => void;
  addButtonLabel?: string;
  searchValue?: string;
  onSearchChange?: (value: string) => void;
  searchPlaceholder?: string;
  children: ReactNode;
  headerActions?: ReactNode;
  showSearch?: boolean;
  showAddButton?: boolean;
};

export function CRUDPageLayout({
  title,
  description,
  onAdd,
  addButtonLabel = "Add New",
  searchValue = "",
  onSearchChange,
  searchPlaceholder = "Search...",
  children,
  headerActions,
  showSearch = true,
  showAddButton = true,
}: CRUDPageLayoutProps) {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">{title}</h1>
          {description && (
            <p className="text-muted-foreground mt-1">{description}</p>
          )}
        </div>
        <div className="flex items-center gap-2">
          {headerActions}
          {showAddButton && onAdd && (
            <Button onClick={onAdd}>
              <Plus className="mr-2 size-4" />
              {addButtonLabel}
            </Button>
          )}
        </div>
      </div>

      <Card>
        <CardHeader>
          {showSearch && onSearchChange && (
            <div className="flex items-center gap-4">
              <div className="relative flex-1">
                <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
                <Input
                  placeholder={searchPlaceholder}
                  value={searchValue}
                  onChange={(e) => onSearchChange(e.target.value)}
                  className="pl-9"
                />
              </div>
            </div>
          )}
        </CardHeader>
        <CardContent>{children}</CardContent>
      </Card>
    </div>
  );
}
