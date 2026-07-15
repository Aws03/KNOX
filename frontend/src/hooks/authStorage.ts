const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const USER_KEY = "authUser";
const EXPIRY_KEY = "authExpiry";

type StoredUser = {
  id: string;
  name: string;
  email: string;
  roles: string[];
  // Extended user details from /api/Users/me
  identityUserId?: number;
  domainUserId?: number;
  fullName?: string;
  dateJoined?: string;
  role?: string;
  isActive?: boolean;
  isVerified?: boolean;
  verificationDate?: string;
  universityId?: number;
  universityName?: string;
  facultyId?: number;
  facultyName?: string;
  majorId?: number;
  majorName?: string;
};

type StoredSession = {
  token: string;
  refreshToken: string;
  user: StoredUser;
  expiresAt: number;
};

const isBrowser = typeof window !== "undefined";

function safeLocalStorage() {
  if (!isBrowser) return null;
  return window.localStorage;
}

export function persistSession(session: StoredSession) {
  const storage = safeLocalStorage();
  if (!storage) return;

  storage.setItem(ACCESS_TOKEN_KEY, session.token);
  storage.setItem(REFRESH_TOKEN_KEY, session.refreshToken);
  storage.setItem(USER_KEY, JSON.stringify(session.user));
  storage.setItem(EXPIRY_KEY, session.expiresAt.toString());
}

export function clearSession() {
  const storage = safeLocalStorage();
  if (!storage) return;

  storage.removeItem(ACCESS_TOKEN_KEY);
  storage.removeItem(REFRESH_TOKEN_KEY);
  storage.removeItem(USER_KEY);
  storage.removeItem(EXPIRY_KEY);
}

export function readSession(): StoredSession | null {
  const storage = safeLocalStorage();
  if (!storage) return null;

  const token = storage.getItem(ACCESS_TOKEN_KEY);
  const refreshToken = storage.getItem(REFRESH_TOKEN_KEY);
  const userRaw = storage.getItem(USER_KEY);
  const expiresAtRaw = storage.getItem(EXPIRY_KEY);

  if (!token || !refreshToken || !userRaw || !expiresAtRaw) {
    return null;
  }

  try {
    const user = JSON.parse(userRaw) as StoredUser;
    const expiresAt = Number(expiresAtRaw);
    if (!user || Number.isNaN(expiresAt)) {
      return null;
    }
    return { token, refreshToken, user, expiresAt };
  } catch {
    return null;
  }
}

export function getStoredToken() {
  const storage = safeLocalStorage();
  return storage?.getItem(ACCESS_TOKEN_KEY) ?? null;
}

export function getStoredRefreshToken() {
  const storage = safeLocalStorage();
  return storage?.getItem(REFRESH_TOKEN_KEY) ?? null;
}

export type { StoredUser, StoredSession };
