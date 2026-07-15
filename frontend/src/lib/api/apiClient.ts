import axios from "axios";
import type { AxiosInstance, InternalAxiosRequestConfig } from "axios";
import { getStoredToken, clearSession } from "@/hooks/authStorage";

export const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5001/api";

/**
 * Configured axios instance for API requests
 * - Automatically attaches auth tokens
 * - Handles 401 unauthorized responses
 * - Supports credentials for refresh tokens
 */
export const apiClient: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

// Request interceptor: Attach access token from localStorage to every request
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getStoredToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Guards against multiple in-flight requests each independently clearing the
// session and redirecting when the token expires (they'd all 401 at once).
// Never reset: window.location.href below fully reloads the page, which
// resets this module's state anyway, so one trip per page life is correct.
let isHandlingUnauthorized = false;

// Response interceptor: Handle 401 errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Don't clear session on login/refresh failures - the auth flow itself
    // (AuthProvider) is responsible for deciding what a failed refresh means.
    const isAuthEndpoint = error.config?.url?.includes("/auth/");

    if (
      error.response?.status === 401 &&
      !isAuthEndpoint &&
      !isHandlingUnauthorized
    ) {
      isHandlingUnauthorized = true;
      console.error("[API] Unauthorized - clearing session");
      clearSession();
      window.location.href = "/auth/login";
    }

    return Promise.reject(error);
  }
);
