// Shared API types
export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
  [key: string]: string | number | boolean | undefined;
}

export interface ApiError {
  message: string;
  statusCode: number;
  details?: unknown;
}

export interface ApiSuccess<T = void> {
  data: T;
  message?: string;
}
