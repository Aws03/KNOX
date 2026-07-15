# Authentication Implementation

## Overview

This implementation provides a complete authentication system with JWT token management, including automatic token refresh and role-based access control.

## Features

### ✅ Implemented

- **JWT Authentication**: Login with email and password
- **Token Decoding**: Automatic decoding of JWT tokens to extract user information
- **Role Management**: Extracts and stores user roles from JWT
- **Automatic Token Refresh**: Refreshes access token 5 minutes before expiration
- **Persistent Sessions**: Stores tokens in localStorage for session persistence
- **Protected Routes**: Component for protecting routes based on authentication and roles
- **User Profile**: Display user information from decoded JWT

## API Integration

### Login Endpoint

```typescript
POST /api/Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**

```typescript
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "5Bi2Hf1V...",
  "expiresAt": "2025-12-04T17:21:38.147519Z"
}
```

### Refresh Token Endpoint

```typescript
POST /api/Auth/refresh
Content-Type: application/json

{
  "refreshToken": "5Bi2Hf1V..."
}
```

## JWT Token Structure

The access token contains the following claims:

- `sub`: User ID
- `unique_name`: User's full name
- `email`: User's email address
- `role`: Array of roles (e.g., ["SuperAdmin", "Admin", "Writer", "User"])
- `exp`: Token expiration timestamp
- `iat`: Token issued at timestamp

## Usage

### 1. Using the Auth Hook

```typescript
import { useAuth } from "@/app/providers/AuthProvider";

function MyComponent() {
  const { user, isAuthenticated, isLoading, login, logout } = useAuth();

  const handleLogin = async () => {
    try {
      await login("email@example.com", "password");
      // Redirect or show success
    } catch (error) {
      // Handle error
    }
  };

  return (
    <div>
      {isAuthenticated ? (
        <>
          <p>Welcome, {user?.name}!</p>
          <p>Roles: {user?.roles.join(", ")}</p>
          <button onClick={logout}>Logout</button>
        </>
      ) : (
        <button onClick={handleLogin}>Login</button>
      )}
    </div>
  );
}
```

### 2. Protected Routes

```typescript
import { ProtectedRoute } from "@/shared/components/auth/ProtectedRoute";

// In your router
<Route
  path="/admin"
  element={
    <ProtectedRoute requiredRoles={["Admin", "SuperAdmin"]}>
      <AdminPage />
    </ProtectedRoute>
  }
/>

// Or without role requirement
<Route
  path="/dashboard"
  element={
    <ProtectedRoute>
      <DashboardPage />
    </ProtectedRoute>
  }
/>
```

### 3. User Profile Component

```typescript
import { UserProfile } from "@/shared/components/auth/UserProfile";

function ProfilePage() {
  return <UserProfile />;
}
```

### 4. Making Authenticated API Calls

The `api.ts` file automatically includes the access token in all requests:

```typescript
import api from "@/lib/api";

// The token is automatically added to headers
const data = await api.request("/protected-endpoint");
```

## Token Storage

### Access Token

- Stored in memory via `setAccessToken()` function
- Also stored in localStorage as backup
- Automatically included in API request headers
- Refreshed automatically before expiration

### Refresh Token

- Stored in localStorage
- Used to obtain new access tokens
- Should also be set as httpOnly cookie by backend (recommended)

## Security Notes

### ⚠️ Important

1. **Refresh Token**: While stored in localStorage, your backend should ALSO set the refresh token as an httpOnly cookie for better security
2. **HTTPS**: Always use HTTPS in production
3. **Token Expiration**: Access tokens are automatically refreshed 5 minutes before expiration
4. **Logout**: Clears all tokens from memory and localStorage

## Token Refresh Flow

1. On app initialization, checks for stored access token
2. Validates token expiration
3. If expired but refresh token exists, automatically refreshes
4. Sets up automatic refresh 5 minutes before token expiration
5. On refresh, updates access token and refresh token

## File Structure

```
src/
├── app/
│   └── providers/
│       └── AuthProvider.tsx          # Main auth context and logic
├── lib/
│   └── api.ts                        # API client with auth integration
└── shared/
    └── components/
        ├── auth/
        │   ├── ProtectedRoute.tsx    # Route protection component
        │   └── UserProfile.tsx       # User profile display
        └── login/
            └── login-form.tsx        # Login form component
```

## Dependencies

- `jwt-decode`: For decoding JWT tokens (not for validation)
- `react-router-dom`: For navigation and routing

## Example: Complete Login Flow

1. User enters email and password in login form
2. Form submits to `POST /api/Auth/login`
3. Backend returns `accessToken`, `refreshToken`, and `expiresAt`
4. `AuthProvider` decodes the access token using `jwt-decode`
5. User information (name, email, roles) extracted from token
6. Tokens stored in localStorage
7. Access token stored in memory for API requests
8. User redirected to homepage
9. Timer set to refresh token 5 minutes before expiration

## Testing

Use these test credentials (from your example):

```
Email: mohammadswedan2003@gmail.com
Password: 0775268718Pp.
```

## Troubleshooting

### Token Not Persisting

- Check if localStorage is enabled
- Check browser console for errors
- Verify token format from backend

### Auto-Refresh Not Working

- Check token expiration time
- Verify refresh token is valid
- Check network requests for refresh endpoint

### Roles Not Working

- Verify JWT contains `role` claim
- Check if role is string or array
- Ensure roles match required roles in ProtectedRoute
