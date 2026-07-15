export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  majorId: number;
}

export interface RegisterResponse {
  message?: string;
  userId?: string;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface RefreshResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface LogoutRequest {
  refreshToken: string;
}

export interface UserInfoResponse {
  identityUserId: number;
  domainUserId: number;
  email: string;
  fullName: string;
  dateJoined: string;
  role: string;
  isActive: boolean;
  isVerified: boolean;
  verificationDate: string;
  profilePictureUrl: string | null;
  universityId: number;
  universityName: string;
  facultyId: number;
  facultyName: string;
  majorId: number;
  majorName: string;
}
