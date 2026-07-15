import type { ReactNode } from "react";
import { Loader2 } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { SelectItem } from "@/shared/ui/select";
import type { PaginatedState } from "../types";

type RenderSelectContentProps<T extends { id: number; name: string }> = {
  state: PaginatedState<T>;
  hasParent: boolean;
  onLoadMore: () => void;
  emptyMessage?: string;
};

export const renderSelectContent = <T extends { id: number; name: string }>({
  state,
  hasParent,
  onLoadMore,
  emptyMessage = "No items found",
}: RenderSelectContentProps<T>): ReactNode => {
  return (
    <>
      {state.items.map((item) => (
        <SelectItem key={item.id} value={item.id.toString()}>
          {item.name}
        </SelectItem>
      ))}
      {state.loading && !state.items.length && hasParent && (
        <div className="flex items-center gap-2 px-3 py-2 text-sm text-muted-foreground">
          <Loader2 className="h-4 w-4 animate-spin" />
          Loading...
        </div>
      )}
      {!state.loading && hasParent && !state.items.length && (
        <div className="px-3 py-2 text-sm text-muted-foreground">
          {emptyMessage}
        </div>
      )}
      {state.hasNextPage && state.items.length > 0 && (
        <div
          className="border-t px-3 py-2"
          onMouseDown={(e) => e.preventDefault()}
        >
          <Button
            variant="ghost"
            size="sm"
            className="w-full justify-center gap-2"
            disabled={state.loading}
            onClick={(e) => {
              e.preventDefault();
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
      )}
    </>
  );
};
