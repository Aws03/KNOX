# User Info Implementation

## Overview

This implementation enhances the authentication system to fetch and store comprehensive user information from the `/api/Users/me` endpoint after successful login or registration.

## What Changed

### 1. **Enhanced User Type** (`src/app/providers/AuthContext.ts`)

The `User` interface now includes all fields returned by `/api/Users/me`:

```typescript
interface User {
  // JWT basic fields
  id: string;
  name: string;
  email: string;
  roles: string[];

  // Extended fields from /api/Users/me
  identityUserId?: number;
  domainUserId?: number;
  fullName?: string;
  dateJoined?: string;
  role?: string;
  isActive?: boolean;
  isVerfied?: boolean;
  verficationDate?: string;
  universityId?: number;
  universityName?: string;
  facultyId?: number;
  facultyName?: string;
  majorId?: number;
  majorName?: string;
}
```

### 2. **New API Function** (`src/lib/axiosClient.ts`)

Added `getUserInfo()` function to fetch detailed user information:

```typescript
export async function getUserInfo(): Promise<UserInfoResponse> {
  const response = await apiClient.get<UserInfoResponse>("/Users/me");
  return response.data;
}
```

### 3. **Enhanced Storage** (`src/lib/authStorage.ts`)

Updated `StoredUser` type to match the enhanced `User` interface, allowing all user details to persist in localStorage.

### 4. **Updated Authentication Flow** (`src/app/providers/AuthProvider.tsx`)

Modified `decodeAndSetUser` function to:

1. First decode the JWT token (for basic user info and authentication)
2. Then fetch detailed user information from `/api/Users/me`
3. Merge both datasets to create a complete user profile
4. Store everything in the auth context and localStorage

## How It Works

### Login/Register Flow:

1. User logs in or registers successfully
2. Backend returns JWT access token and refresh token
3. Frontend decodes JWT for basic user info (id, email, roles)
4. **NEW:** Frontend automatically calls `/api/Users/me` to get detailed user info
5. Data is merged: JWT data + API response = complete user profile
6. Everything is stored in:
   - React Context (for immediate access via `useAuth()`)
   - localStorage (for persistence across sessions)

### Accessing User Information:

```typescript
import { useAuth } from "@/app/providers/useAuth";

function MyComponent() {
  const { user } = useAuth();

  // Access any user field
  console.log(user?.fullName); // "Mohammad Swedan"
  console.log(user?.universityName); // "jadara university"
  console.log(user?.facultyName); // "Information Technology"
  console.log(user?.majorName); // "Computer science"
  console.log(user?.role); // "Writer"
  console.log(user?.isVerfied); // true

  return <div>Welcome, {user?.fullName}!</div>;
}
```

## Error Handling

The implementation is fault-tolerant:

- If `/api/Users/me` fails, the system falls back to basic JWT data
- User can still log in and use the app with limited profile information
- A warning is logged to the console for debugging

## Benefits

1. **Complete User Profile**: Access to all user information without additional API calls
2. **Better User Experience**: Display university, faculty, and major information
3. **Role-Based Features**: Use `user.role` for feature access control
4. **Verification Status**: Check `user.isVerfied` for account verification
5. **Persistent Storage**: User info survives page refreshes
6. **Single Source of Truth**: All components use the same user data from context

## Updated Components

### UserProfile Component

Updated to display the enhanced user information including:

- Full name
- Role badge
- University, Faculty, and Major
- Member since date
- Account status (Active/Verified)

## Example Use Cases

### 1. Display User's University

```typescript
const { user } = useAuth();
return <p>You are studying at {user?.universityName}</p>;
```

### 2. Check User Role

```typescript
const { user } = useAuth();
if (user?.role === "Writer") {
  return <WriterDashboard />;
} else if (user?.role === "Admin") {
  return <AdminDashboard />;
}
```

### 3. Filter Content by Major

```typescript
const { user } = useAuth();
const courses = await fetchCourses({ majorId: user?.majorId });
```

### 4. Verify Account Status

```typescript
const { user } = useAuth();
if (!user?.isVerfied) {
  return <VerificationPrompt />;
}
```

## Testing

To test the implementation:

1. Log in with valid credentials
2. Open browser DevTools → Console
3. Look for log: `[Auth] Fetched detailed user info:` followed by the user data
4. Check localStorage for the `authUser` key - it should contain all user fields
5. Use `useAuth()` hook in any component to access user information

## Notes

- The endpoint `/api/Users/me` requires a valid Bearer token (automatically handled by axios interceptor)
- All optional fields are marked with `?` to handle cases where data might not be available
- The system prioritizes the `/api/Users/me` data over JWT claims for display purposes
- JWT roles are still used for authorization, but the `role` field from the API is used for display
