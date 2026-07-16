import { apiClient } from "@/lib/api/apiClient";
import type {
  ChangePasswordRequest,
  ChangePasswordResponse,
  ForgotPasswordRequest,
  ForgotPasswordResponse,
  LoginRequest,
  LoginResponse,
  RefreshRequest,
  RefreshResponse,
  RegisterRequest,
  RegisterResponse,
  ResetPasswordRequest,
  ResetPasswordResponse,
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

// There's no dedicated /auth/forgot-password endpoint - send-verification-otp
// is the same generic "email an OTP to this address" mechanism the account
// verification flow uses, and it's what the reset-password flow relies on too.
export async function forgotPassword(
  data: ForgotPasswordRequest
): Promise<ForgotPasswordResponse> {
  const response = await apiClient.post<ForgotPasswordResponse>(
    "/auth/send-verification-otp",
    data
  );
  return response.data;
}

export async function resetPassword(
  data: ResetPasswordRequest
): Promise<ResetPasswordResponse> {
  const response = await apiClient.post<ResetPasswordResponse>(
    "/auth/reset-password",
    data
  );
  return response.data;
}

// Requires an authenticated session - apiClient's request interceptor attaches
// the access token automatically, same as every other authenticated call.
export async function changePassword(
  data: ChangePasswordRequest
): Promise<ChangePasswordResponse> {
  const response = await apiClient.post<ChangePasswordResponse>(
    "/auth/change-password",
    data
  );
  return response.data;
}
