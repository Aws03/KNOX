import { apiClient } from "@/lib/api/apiClient";
import type { AnalyticsData } from "./types";

export async function fetchAnalyticsData(): Promise<AnalyticsData> {
  const response = await apiClient.get<AnalyticsData>("/dashboard/statistics");
  return response.data;
}
