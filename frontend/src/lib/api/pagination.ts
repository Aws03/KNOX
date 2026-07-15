import { apiClient } from "./apiClient";
import type { PaginatedResponse, PaginationParams } from "./types";

/**
 * Helper function for making paginated API requests
 * @param path - API endpoint path
 * @param pageNumber - Current page number (default: 1)
 * @param pageSize - Number of items per page (default: 50)
 * @param extra - Additional query parameters as URL search string
 */
export async function getPaginated<T>(
  path: string,
  pageNumber = 1,
  pageSize = 50,
  extra = ""
): Promise<PaginatedResponse<T>> {
  const params: PaginationParams = { pageNumber, pageSize };

  // Parse extra query params if provided
  if (extra) {
    const extraParams = new URLSearchParams(extra);
    extraParams.forEach((value, key) => {
      params[key] = value;
    });
  }

  const response = await apiClient.get<PaginatedResponse<T>>(path, { params });
  return response.data;
}
