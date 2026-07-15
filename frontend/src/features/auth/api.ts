import { apiClient } from "@/lib/api/apiClient";
import type {
  LoginRequest,
  LoginResponse,
  RefreshRequest,
  RefreshResponse,
  RegisterRequest,
  RegisterResponse,
  UserInfoResponse,
} from "@/features/auth/types";
import { clearSession } from "@/hooks/authStorage";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const response = await apiClient.post<LoginResponse>("/auth/login", data);
  return response.data;
}

export async function refreshToken(
  data: RefreshRequest
): Promise<RefreshResponse> {
  const response = await apiClient.post<RefreshResponse>("/auth/refresh", data);
  return response.data;
}

export async function logout(refreshToken?: string): Promise<void> {
  try {
    if (refreshToken) {
      await apiClient.post("/auth/logout", { refreshToken });
    } else {
      await apiClient.post("/auth/logout");
    }
  } catch (error) {
    console.error("Logout API call failed:", error);
  } finally {
    clearSession();
  }
}

export async function register(
  data: RegisterRequest
): Promise<RegisterResponse> {
  const response = await apiClient.post<RegisterResponse>(
    "/auth/register",
    data
  );
  return response.data;
}

export async function getUserInfo(): Promise<UserInfoResponse> {
  const response = await apiClient.get<UserInfoResponse>("/Users/me");
  return response.data;
}
