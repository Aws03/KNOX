import { useState, useCallback, useEffect, useRef } from "react";
import { Loader2 } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Button } from "@/shared/ui/button";
import { Label } from "@/shared/ui/label";

export type PaginatedItem = {
  id: number;
  name: string;
};

type PaginatedSelectProps<T extends PaginatedItem> = {
  label: string;
  placeholder?: string;
  value: T | null;
  onChange: (item: T | null) => void;
  fetchPage: (pageNumber: number) => Promise<{
    items: T[];
    hasNextPage: boolean;
  }>;
  disabled?: boolean;
  parentId?: number | null;
  pageSize?: number;
  required?: boolean;
  error?: string;
};

export function PaginatedSelect<T extends PaginatedItem>({
  label,
  placeholder = `Select ${label.toLowerCase()}`,
  value,
  onChange,
  fetchPage,
  disabled = false,
  parentId = null,
  required = false,
  error,
}: PaginatedSelectProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [hasNextPage, setHasNextPage] = useState(true);
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const loadingRef = useRef(false);
  const valueRef = useRef(value);

  // Keep valueRef in sync with value prop
  useEffect(() => {
    valueRef.current = value;
  }, [value]);

  const loadPage = useCallback(
    async (page: number, append = false) => {
      if (loadingRef.current) return;

      loadingRef.current = true;
      setLoading(true);
      try {
        const result = await fetchPage(page);
        setItems((prev) => {
          const newItems = append ? [...prev, ...result.items] : result.items;

          // If we have a pre-selected value and it's not in the fetched items, prepend it
          const currentValue = valueRef.current;
          if (currentValue && !append && page === 1) {
            const valueExists = newItems.some(
              (item) => item.id === currentValue.id
            );
            if (!valueExists) {
              return [currentValue, ...newItems];
            }
          }

          return newItems;
        });
        setHasNextPage(result.hasNextPage);
        setPageNumber(page);
      } catch (error) {
        console.error(`Failed to load ${label}:`, error);
      } finally {
        loadingRef.current = false;
        setLoading(false);
      }
    },
    [fetchPage, label]
  );

  // Load first page when parent changes or on mount
  useEffect(() => {
    setItems([]);
    setPageNumber(1);
    setHasNextPage(true);

    if (!disabled) {
      loadPage(1, false);
    }
  }, [parentId, disabled, loadPage]);

  const handleLoadMore = () => {
    if (hasNextPage && !loading) {
      loadPage(pageNumber + 1, true);
    }
  };

  const handleValueChange = (itemId: string) => {
    const selectedItem = items.find((item) => item.id.toString() === itemId);
    onChange(selectedItem || null);
  };

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open);
    // Load first page when opening if empty
    if (open && items.length === 0 && !loading && !disabled) {
      loadPage(1, false);
    }
  };

  return (
    <div className="space-y-2">
      <Label htmlFor={`select-${label.toLowerCase().replace(/\s+/g, "-")}`}>
        {label}
        {required && <span className="text-destructive ml-1">*</span>}
      </Label>
      <Select
        value={value?.id.toString() || ""}
        onValueChange={handleValueChange}
        disabled={disabled || loading}
        open={isOpen}
        onOpenChange={handleOpenChange}
      >
        <SelectTrigger
          id={`select-${label.toLowerCase().replace(/\s+/g, "-")}`}
          className={error ? "border-destructive" : ""}
        >
          <SelectValue placeholder={placeholder}>
            {value?.name || placeholder}
          </SelectValue>
        </SelectTrigger>
        <SelectContent>
          {loading && items.length === 0 ? (
            <div className="flex items-center justify-center py-6">
              <Loader2 className="size-4 animate-spin" />
              <span className="ml-2 text-sm text-muted-foreground">
                Loading {label.toLowerCase()}...
              </span>
            </div>
          ) : items.length === 0 ? (
            <div className="py-6 text-center text-sm text-muted-foreground">
              No {label.toLowerCase()} found
            </div>
          ) : (
            <>
              {items.map((item) => (
                <SelectItem key={item.id} value={item.id.toString()}>
                  {item.name}
                </SelectItem>
              ))}
              {hasNextPage && (
                <div className="border-t p-2">
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={handleLoadMore}
                    disabled={loading}
                    className="w-full"
                  >
                    {loading ? (
                      <>
                        <Loader2 className="mr-2 size-4 animate-spin" />
                        Loading...
                      </>
                    ) : (
                      "Load More"
                    )}
                  </Button>
                </div>
              )}
            </>
          )}
        </SelectContent>
      </Select>
      {error && <p className="text-sm text-destructive">{error}</p>}
    </div>
  );
}
