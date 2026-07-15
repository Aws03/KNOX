import type { ReactNode } from "react";
import { Loader2 } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Button } from "@/shared/ui/button";
import type { PaginatedState } from "../hooks/usePaginatedSelect";

type FilterSelectProps<T extends { id: number; name: string }> = {
  value: string;
  onChange: (value: string) => void;
  onOpenChange?: (open: boolean) => void;
  state: PaginatedState<T>;
  onLoadMore: () => void;
  placeholder: string;
  disabled?: boolean;
  disabledPlaceholder?: string;
  className?: string;
  renderItem?: (item: T) => ReactNode;
};

export const FilterSelect = <T extends { id: number; name: string }>({
  value,
  onChange,
  onOpenChange,
  state,
  onLoadMore,
  placeholder,
  disabled = false,
  disabledPlaceholder,
  className = "",
  renderItem,
}: FilterSelectProps<T>) => {
  const renderLoadingRow = (label: string) => (
    <div className="flex items-center gap-2 px-3 py-2 text-sm text-muted-foreground">
      <Loader2 className="h-4 w-4 animate-spin" />
      {label}
    </div>
  );

  const renderLoadMoreRow = () => (
    <div
      className="border-t px-3 py-2"
      onMouseDown={(event) => event.preventDefault()}
    >
      <Button
        variant="ghost"
        size="sm"
        className="w-full justify-center gap-2"
        disabled={state.loading}
        onClick={(event) => {
          event.preventDefault();
          onLoadMore();
        }}
      >
        {state.loading ? (
          <>
            <Loader2 className="h-4 w-4 animate-spin" /> Loading…
          </>
        ) : (
          "Load more"
        )}
      </Button>
    </div>
  );

  return (
    <Select
      value={value}
      onValueChange={onChange}
      onOpenChange={onOpenChange}
      disabled={disabled}
    >
      <SelectTrigger className={className}>
        <SelectValue
          placeholder={
            disabled && disabledPlaceholder ? disabledPlaceholder : placeholder
          }
        />
      </SelectTrigger>
      <SelectContent>
        {state.items.map((item) => (
          <SelectItem key={item.id} value={item.id.toString()}>
            {renderItem ? renderItem(item) : item.name}
          </SelectItem>
        ))}
        {state.loading && !state.items.length && renderLoadingRow("Loading…")}
        {!state.loading && !state.items.length && (
          <div className="px-3 py-2 text-sm text-muted-foreground">
            No items found
          </div>
        )}
        {state.hasNextPage && state.items.length > 0 && renderLoadMoreRow()}
      </SelectContent>
    </Select>
  );
};
