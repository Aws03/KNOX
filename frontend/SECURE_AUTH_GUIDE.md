# Secure Authentication Implementation

## Overview

This authentication system implements a **secure token-based authentication flow** following industry best practices:

- ✅ **No refresh tokens stored on client** - Refresh tokens are HttpOnly cookies only
- ✅ **Access tokens in memory only** - Never persisted to localStorage
- ✅ **Automatic token refresh** - Axios interceptor handles 401 errors
- ✅ **CSRF protection** - SameSite=Strict cookies
- ✅ **XSS protection** - No tokens accessible from JavaScript

## Architecture

### 1. Token Storage Strategy

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT (React App)                        │
├─────────────────────────────────────────────────────────────┤
│  Access Token:  ✓ Memory (React state + axios variable)     │
│  Refresh Token: ✗ NEVER STORED (HttpOnly cookie only)       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    BACKEND (API Server)                      │
├─────────────────────────────────────────────────────────────┤
│  Refresh Token: ✓ HttpOnly, Secure, SameSite=Strict cookie │
└─────────────────────────────────────────────────────────────┘
```

### 2. Authentication Flow

#### Login Flow

```
1. User submits credentials
2. POST /auth/login { email, password }
3. Backend responds with:
   - JSON: { accessToken: "..." }
   - Cookie: refreshToken (HttpOnly, Secure, SameSite=Strict)
4. Client stores accessToken in memory only
5. Client decodes JWT to extract user info
```

#### Token Refresh Flow

```
1. API request returns 401 Unauthorized
2. Axios interceptor catches error
3. POST /auth/refresh (cookie sent automatically)
4. Backend validates HttpOnly cookie
5. Backend responds with new accessToken
6. Original request retried with new token
```

#### Logout Flow

```
1. POST /auth/logout
2. Backend clears HttpOnly cookie
3. Client clears memory state
4. User redirected to login
```

## Files Structure

```
src/
├── lib/
│   └── axiosClient.ts          # Axios instance with 401 interceptor
├── app/
│   └── providers/
│       ├── AuthContext.ts       # Auth context types
│       ├── AuthProvider.tsx     # Auth state management
│       └── useAuth.ts          # Auth hook
└── shared/
    └── components/
        └── auth/
            └── ProtectedRoute.tsx  # Route guard with refresh
```

## Key Components

### 1. Axios Client (`lib/axiosClient.ts`)

**Features:**

- Automatic access token attachment to requests
- 401 error interceptor with automatic refresh
- Request queuing during refresh
- Redirect to login on refresh failure

**Security:**

- `withCredentials: true` - Sends HttpOnly cookies
- Access token in memory only
- No localStorage/sessionStorage usage

### 2. AuthProvider (`app/providers/AuthProvider.tsx`)

**Features:**

- User state management
- Login/logout functions
- Automatic session restoration on page load
- JWT decoding for user info extraction

**Security:**

- No token persistence
- Refresh token never accessible from JS
- Memory-only storage

### 3. ProtectedRoute (`shared/components/auth/ProtectedRoute.tsx`)

**Features:**

- Route access control
- Automatic refresh attempt on first load
- Role-based access control
- Loading states

## Usage Examples

### Using the Auth Hook

```typescript
import { useAuth } from "@/app/providers/useAuth";

function MyComponent() {
  const { user, isAuthenticated, login, logout } = useAuth();

  const handleLogin = async () => {
    try {
      await login("user@example.com", "password");
      // User is now logged in
    } catch (error) {
      // Handle error
    }
  };

  return (
    <div>
      {isAuthenticated ? (
        <>
          <p>Welcome {user?.name}</p>
          <button onClick={logout}>Logout</button>
        </>
      ) : (
        <button onClick={handleLogin}>Login</button>
      )}
    </div>
  );
}
```

### Protected Routes

```typescript
import { ProtectedRoute } from '@/shared/components/auth/ProtectedRoute';

// Simple protection
<ProtectedRoute>
  <DashboardPage />
</ProtectedRoute>

// Role-based protection
<ProtectedRoute requiredRoles={['admin']}>
  <AdminPanel />
</ProtectedRoute>
```

### Making Authenticated API Calls

```typescript
import apiClient from "@/lib/axiosClient";

// Access token is automatically attached
// 401 errors automatically trigger refresh
async function fetchUserData() {
  const response = await apiClient.get("/user/profile");
  return response.data;
}
```

## Backend Requirements

Your backend must implement these endpoints:

### POST /auth/login

```typescript
Request: { email: string, password: string }
Response: {
  accessToken: string,  // JWT with short expiry (15 min)
  user?: {
    id: string,
    name: string,
    email: string,
    roles: string[]
  }
}
Set-Cookie: refreshToken=...; HttpOnly; Secure; SameSite=Strict; Max-Age=604800
```

### POST /auth/refresh

```typescript
Request: {} // Empty body, uses HttpOnly cookie
Cookie: refreshToken (sent automatically)
Response: {
  accessToken: string  // New JWT
}
```

### POST /auth/logout

```typescript
Request: {}
Response: { success: true }
Set-Cookie: refreshToken=; HttpOnly; Secure; SameSite=Strict; Max-Age=0
```

## Security Features

### 1. XSS Protection

- Refresh token in HttpOnly cookie - **cannot be accessed by JavaScript**
- Access token in memory - **cleared on page reload/close**
- No tokens in localStorage/sessionStorage

### 2. CSRF Protection

- SameSite=Strict cookie - **only sent to same origin**
- Short-lived access tokens - **limited exposure window**

### 3. Token Rotation

- Access tokens expire quickly (15 min recommended)
- Refresh tokens can be rotated on each use
- Automatic cleanup on logout

### 4. Network Security

- All requests use HTTPS (Secure cookie flag)
- Credentials included only when needed
- No sensitive data in URLs

## Testing Checklist

- [ ] Login with valid credentials succeeds
- [ ] Login with invalid credentials fails
- [ ] Access token is attached to API requests
- [ ] 401 error triggers automatic refresh
- [ ] Failed refresh redirects to login
- [ ] Logout clears all auth state
- [ ] Page reload attempts session restore via HttpOnly cookie
- [ ] Protected routes require authentication
- [ ] Role-based access control works
- [ ] Tokens never appear in localStorage

## Common Issues

### Issue: "No valid session found" on page reload

**Cause:** Refresh token cookie expired or not sent
**Solution:** Check backend cookie settings and ensure `withCredentials: true`

### Issue: Redirect to login after successful login

**Cause:** Access token not properly stored
**Solution:** Check if `setAccessToken()` is called after login

### Issue: 401 errors not triggering refresh

**Cause:** Axios interceptor not configured
**Solution:** Verify axios instance is imported from `axiosClient.ts`

## Migration from localStorage-based Auth

If migrating from localStorage:

1. Remove all `localStorage.setItem/getItem` calls for tokens
2. Update backend to set HttpOnly refresh token cookie
3. Update API client to use axios with interceptors
4. Remove manual token refresh logic
5. Test thoroughly with browser DevTools → Application → Cookies

## Performance Considerations

- **Memory overhead:** Minimal - single access token string
- **Network overhead:** Automatic refresh adds one extra request per session
- **Browser support:** All modern browsers support HttpOnly cookies
- **Mobile:** Works on all React Native apps with WebView

## Future Enhancements

- [ ] Token expiry warning before automatic refresh
- [ ] Multi-tab synchronization
- [ ] Remember me functionality (longer refresh token expiry)
- [ ] Biometric authentication integration
- [ ] Rate limiting on refresh endpoint
