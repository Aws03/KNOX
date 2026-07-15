import React, { useState, useEffect, useCallback, useRef } from "react";
import { jwtDecode } from "jwt-decode";
import {
  login as apiLogin,
  logout as apiLogout,
  refreshToken as apiRefreshToken,
  getUserInfo,
} from "@/features/auth/api";
import { AuthContext, type User, type AuthContextType } from "./AuthContext";
import {
  clearSession,
  persistSession,
  readSession,
  getStoredRefreshToken,
} from "@/hooks/authStorage";

interface JWTPayload {
  sub: string;
  unique_name?: string;
  name?: string;
  email?: string;
  Email?: string;
  jti: string;
  iat: number;
  role?: string | string[];
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?:
    | string
    | string[];
  nbf: number;
  exp: number;
  iss: string;
  aud: string;
}

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const isInitialized = useRef(false);
  const refreshTimeoutRef = useRef<number | null>(null);
  const isRefreshing = useRef(false);
  // Latches once a session is torn down (e.g. after a failed refresh) so
  // nothing already in flight or scheduled can restart the refresh cycle.
  // Cleared again on a fresh, successful login.
  const isSessionTerminated = useRef(false);

  const clearRefreshTimer = useCallback(() => {
    if (refreshTimeoutRef.current && typeof window !== "undefined") {
      window.clearTimeout(refreshTimeoutRef.current);
    }
    refreshTimeoutRef.current = null;
  }, []);

  const handleSessionExpired = useCallback(() => {
    // Idempotent: multiple callers (a failed refresh, a 401 elsewhere) can
    // race here - only the first one should actually tear anything down.
    if (isSessionTerminated.current) return;
    isSessionTerminated.current = true;

    clearRefreshTimer();
    setUser(null);
    clearSession();
    // Redirect to login page if not already there
    if (
      typeof window !== "undefined" &&
      !window.location.pathname.includes("/auth/login")
    ) {
      window.location.href = "/auth/login";
    }
  }, [clearRefreshTimer]);

  // Refresh the access token before it expires
  const scheduleTokenRefresh = useCallback(
    async (expiresAt: number, currentRefreshToken: string) => {
      // Once a session has been torn down, nothing may schedule or attempt
      // another refresh - this is what stops the loop from restarting itself.
      if (isSessionTerminated.current) {
        return;
      }

      clearRefreshTimer();

      const now = Date.now();
      const timeUntilExpiry = expiresAt - now;

      // Refresh 5 minutes before expiry (or halfway through if token is short-lived)
      const refreshBuffer = Math.min(5 * 60 * 1000, timeUntilExpiry / 2);
      const timeUntilRefresh = timeUntilExpiry - refreshBuffer;

      if (timeUntilRefresh <= 0) {
        // Token is already expired or about to expire, refresh immediately
        if (isRefreshing.current) {
          return;
        }

        isRefreshing.current = true;
        try {
          const response = await apiRefreshToken({
            refreshToken: currentRefreshToken,
          });

          if (response.accessToken) {
            await decodeAndSetUser(
              response.accessToken,
              response.refreshToken
            );
          }
        } catch (error) {
          console.error("[Auth] Failed to refresh token:", error);
          handleSessionExpired();
        } finally {
          isRefreshing.current = false;
        }
        return;
      }

      // Schedule refresh
      const MAX_TIMEOUT = 2147483647; // ~24.8 days
      const safeTimeout = Math.min(timeUntilRefresh, MAX_TIMEOUT);

      if (typeof window !== "undefined") {
        refreshTimeoutRef.current = window.setTimeout(async () => {
          if (isSessionTerminated.current || isRefreshing.current) {
            return;
          }

          isRefreshing.current = true;

          try {
            const response = await apiRefreshToken({
              refreshToken: currentRefreshToken,
            });

            if (response.accessToken) {
              await decodeAndSetUser(
                response.accessToken,
                response.refreshToken
              );
            }
          } catch (error) {
            console.error("[Auth] Failed to refresh token:", error);
            handleSessionExpired();
          } finally {
            isRefreshing.current = false;
          }
        }, safeTimeout);
      }
    },
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [clearRefreshTimer, handleSessionExpired]
  );

  // Decode JWT and extract user info and store in localStorage
  const decodeAndSetUser = useCallback(
    async (accessToken: string, refreshToken: string) => {
      try {
        const decoded = jwtDecode<JWTPayload>(accessToken);

        // Handle roles
        const rolesClaim =
          decoded.role ||
          decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ];
        const roles = rolesClaim
          ? Array.isArray(rolesClaim)
            ? rolesClaim
            : [rolesClaim]
          : [];

        // Basic user data from JWT
        let userData: User = {
          id: decoded.sub,
          name: decoded.unique_name || decoded.name || "",
          email: decoded.email || decoded.Email || "",
          roles,
        };

        const expiresAt = decoded.exp * 1000;
        const now = Date.now();
        const timeUntilExpiry = expiresAt - now;

        // Check if token is already expired
        if (timeUntilExpiry <= 0) {
          console.error("[Auth] Token is already expired");
          handleSessionExpired();
          throw new Error("Token is already expired");
        }

        // Persist token first so axios interceptor can use it
        persistSession({
          token: accessToken,
          refreshToken,
          user: userData,
          expiresAt,
        });

        // Fetch detailed user info from /api/Users/me
        try {
          const userInfo = await getUserInfo();

          // Merge detailed user info with JWT data
          userData = {
            ...userData,
            identityUserId: userInfo.identityUserId,
            domainUserId: userInfo.domainUserId,
            fullName: userInfo.fullName,
            dateJoined: userInfo.dateJoined,
            role: userInfo.role,
            isActive: userInfo.isActive,
            isVerified: userInfo.isVerified,
            verificationDate: userInfo.verificationDate,
            profilePictureUrl: userInfo.profilePictureUrl,
            universityId: userInfo.universityId,
            universityName: userInfo.universityName,
            facultyId: userInfo.facultyId,
            facultyName: userInfo.facultyName,
            majorId: userInfo.majorId,
            majorName: userInfo.majorName,
          };

          // Update name if fullName is available
          if (userInfo.fullName) {
            userData.name = userInfo.fullName;
          }

          // Update session with complete user data
          persistSession({
            token: accessToken,
            refreshToken,
            user: userData,
            expiresAt,
          });
        } catch (error) {
          console.warn(
            "[Auth] Failed to fetch detailed user info, using JWT data only:",
            error
          );
          // Continue with basic JWT data if fetching detailed info fails
        }

        // Set user state
        setUser(userData);

        // Schedule token refresh
        await scheduleTokenRefresh(expiresAt, refreshToken);
      } catch (error) {
        console.error("[Auth] Failed to decode token:", error);
        throw error;
      }
    },
    [handleSessionExpired, scheduleTokenRefresh]
  );

  // Login function
  const login = useCallback(
    async (email: string, password: string) => {
      try {
        setIsLoading(true);
        // A fresh login starts a new session lifecycle - if the previous one
        // was terminated (e.g. failed refresh) without a full page reload
        // (already on the login page), the refresh cycle must be able to run again.
        isSessionTerminated.current = false;
        const response = await apiLogin({ email, password });

        if (response.accessToken && response.refreshToken) {
          await decodeAndSetUser(response.accessToken, response.refreshToken);
        } else {
          throw new Error("No access token in response");
        }
      } catch (error) {
        console.error("[Auth] Login failed:", error);
        handleSessionExpired();
        throw error;
      } finally {
        setIsLoading(false);
      }
    },
    [decodeAndSetUser, handleSessionExpired]
  );

  // Logout function
  const logout = useCallback(async () => {
    try {
      const refreshToken = getStoredRefreshToken();
      await apiLogout(refreshToken || undefined);
    } catch (error) {
      console.error("[Auth] Logout error:", error);
    } finally {
      // Clear filter storage on logout
      localStorage.removeItem("course_filter_state");
      handleSessionExpired();
    }
  }, [handleSessionExpired]);

  // Initialize auth on mount
  useEffect(() => {
    if (isInitialized.current) return;
    isInitialized.current = true;

    const initAuth = async () => {
      try {
        const existingSession = readSession();
        if (existingSession) {
          const isExpired = existingSession.expiresAt <= Date.now();

          if (isExpired) {
            // Try to refresh with refresh token
            try {
              const response = await apiRefreshToken({
                refreshToken: existingSession.refreshToken,
              });

              if (response.accessToken && response.refreshToken) {
                await decodeAndSetUser(
                  response.accessToken,
                  response.refreshToken
                );
              } else {
                handleSessionExpired();
              }
            } catch (error) {
              console.error("[Auth] Failed to refresh expired token:", error);
              handleSessionExpired();
            }
          } else {
            // Session still valid
            setUser(existingSession.user);
            await scheduleTokenRefresh(
              existingSession.expiresAt,
              existingSession.refreshToken
            );
          }
        }
      } catch (error) {
        console.error("[Auth] Failed to initialize auth:", error);
        handleSessionExpired();
      } finally {
        setIsLoading(false);
      }
    };

    initAuth();

    return () => {
      clearRefreshTimer();
    };
  }, [
    clearRefreshTimer,
    decodeAndSetUser,
    handleSessionExpired,
    scheduleTokenRefresh,
  ]);

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
