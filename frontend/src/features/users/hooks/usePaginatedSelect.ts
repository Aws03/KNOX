import { useCallback, useState } from "react";
import { getPaginated } from "@/lib/api/pagination";

export type PaginatedState<T> = {
  items: T[];
  pageNumber: number;
  hasNextPage: boolean;
  loading: boolean;
  parentId: number | null;
};

export const createInitialPaginatedState = <T>(): PaginatedState<T> => ({
  items: [],
  pageNumber: 0,
  hasNextPage: true,
  loading: false,
  parentId: null,
});

type UsePaginatedSelectOptions = {
  pageSize?: number;
};

export const usePaginatedSelect = <T extends { id: number }>(
  options: UsePaginatedSelectOptions = {}
) => {
  const { pageSize = 50 } = options;
  const [state, setState] = useState<PaginatedState<T>>(() =>
    createInitialPaginatedState<T>()
  );

  const fetchData = useCallback(
    async (
      endpoint: string,
      page = 1,
      append = false,
      parentId: number | null = null
    ) => {
      setState((prev) => ({
        ...prev,
        loading: true,
        parentId: parentId ?? prev.parentId,
      }));

      try {
        const res = await getPaginated<T>(endpoint, page, pageSize);
        setState((prev) => {
          // Prevent stale updates if parent changed
          if (parentId !== null && prev.parentId !== parentId) {
            return prev;
          }
          return {
            items: append ? [...prev.items, ...res.items] : res.items,
            pageNumber: res.pageNumber,
            hasNextPage: res.hasNextPage,
            loading: false,
            parentId: prev.parentId,
          };
        });
      } catch (error) {
        console.error(`Failed to load data from ${endpoint}`, error);
        setState((prev) => ({ ...prev, loading: false }));
      }
    },
    [pageSize]
  );

  const reset = useCallback(() => {
    setState(createInitialPaginatedState<T>());
  }, []);

  const loadMore = useCallback(
    (endpoint: string, parentId: number | null = null) => {
      if (state.loading || !state.hasNextPage) return;
      fetchData(endpoint, state.pageNumber + 1, true, parentId);
    },
    [state.loading, state.hasNextPage, state.pageNumber, fetchData]
  );

  return {
    state,
    fetchData,
    loadMore,
    reset,
  };
};
