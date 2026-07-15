# Quick Reference: Accessing User Information

## Import the Hook

```typescript
import { useAuth } from "@/app/providers/useAuth";
```

## Available User Fields

### Basic Information (from JWT)

```typescript
const { user } = useAuth();

user?.id; // string - User's unique identifier from JWT
user?.name; // string - Name from JWT (fallback)
user?.email; // string - User's email address
user?.roles; // string[] - User roles from JWT
```

### Extended Information (from /api/Users/me)

```typescript
user?.identityUserId; // number - Identity user ID
user?.domainUserId; // number - Domain user ID
user?.fullName; // string - User's full name (preferred over name)
user?.role; // string - User role (e.g., "Writer", "Admin")
user?.dateJoined; // string - ISO date when user joined
user?.isActive; // boolean - Is the user account active?
user?.isVerfied; // boolean - Is the user verified?
user?.verficationDate; // string - ISO date of verification

// Academic Information
user?.universityId; // number - University ID
user?.universityName; // string - e.g., "jadara university"
user?.facultyId; // number - Faculty ID
user?.facultyName; // string - e.g., "Information Technology"
user?.majorId; // number - Major ID
user?.majorName; // string - e.g., "Computer science"
```

## Common Usage Examples

### 1. Display Welcome Message

```typescript
function WelcomeHeader() {
  const { user } = useAuth();
  return <h1>Welcome back, {user?.fullName || user?.name}!</h1>;
}
```

### 2. Show University Information

```typescript
function UniversityBadge() {
  const { user } = useAuth();
  return (
    <div>
      <p>{user?.universityName}</p>
      <p>
        {user?.facultyName} - {user?.majorName}
      </p>
    </div>
  );
}
```

### 3. Role-Based Rendering

```typescript
function Dashboard() {
  const { user } = useAuth();

  if (user?.role === "Admin") {
    return <AdminDashboard />;
  }

  if (user?.role === "Writer") {
    return <WriterDashboard />;
  }

  return <StudentDashboard />;
}
```

### 4. Conditional Features

```typescript
function QuizSection() {
  const { user } = useAuth();

  if (!user?.isVerfied) {
    return <VerificationRequired />;
  }

  return <QuizList />;
}
```

### 5. Filter by Major

```typescript
function MyCourses() {
  const { user } = useAuth();
  const [courses, setCourses] = useState([]);

  useEffect(() => {
    if (user?.majorId) {
      fetchCourses(user.majorId).then(setCourses);
    }
  }, [user?.majorId]);

  return <CourseList courses={courses} />;
}
```

### 6. Profile Display

```typescript
function ProfileCard() {
  const { user } = useAuth();

  return (
    <Card>
      <CardHeader>
        <CardTitle>{user?.fullName}</CardTitle>
        <CardDescription>{user?.email}</CardDescription>
      </CardHeader>
      <CardContent>
        <div>
          <Badge>{user?.role}</Badge>
          {user?.isVerfied && <Badge variant="success">Verified</Badge>}
        </div>
        <Separator />
        <div className="space-y-2">
          <p>
            <strong>University:</strong> {user?.universityName}
          </p>
          <p>
            <strong>Faculty:</strong> {user?.facultyName}
          </p>
          <p>
            <strong>Major:</strong> {user?.majorName}
          </p>
          <p>
            <strong>Member Since:</strong>{" "}
            {new Date(user?.dateJoined || "").toLocaleDateString()}
          </p>
        </div>
      </CardContent>
    </Card>
  );
}
```

### 7. Check Authentication Status

```typescript
function ProtectedComponent() {
  const { isAuthenticated, user, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/auth/login" />;
  }

  return <div>Protected content for {user?.fullName}</div>;
}
```

### 8. Use in API Requests

```typescript
async function submitQuiz(answers: Answer[]) {
  const { user } = useAuth();

  const submission = {
    answers,
    userId: user?.domainUserId,
    majorId: user?.majorId,
    submittedAt: new Date().toISOString(),
  };

  await api.post("/quizzes/submit", submission);
}
```

## Type Safety Tips

All user fields are optional (`?`), so always use optional chaining:

```typescript
// ✅ Good
user?.fullName;
user?.universityId;
user?.isVerfied;

// ❌ Bad (will cause errors if user is null)
user.fullName;
user.universityId;
user.isVerfied;
```

## Default Values

Use nullish coalescing for fallback values:

```typescript
const displayName = user?.fullName || user?.name || "Guest";
const universityName = user?.universityName || "Not specified";
```

## Checking Specific Fields

```typescript
// Check if user has university info
if (user?.universityId && user?.universityName) {
  console.log(`Student at ${user.universityName}`);
}

// Check verification status
if (user?.isVerfied) {
  // Show verified-only features
}

// Check if user is active
if (user?.isActive) {
  // Allow actions
} else {
  // Show account suspended message
}
```

## Where User Data is Stored

1. **React Context**: Available via `useAuth()` hook throughout the app
2. **localStorage**: Key `authUser` (automatically synced)
3. **Token**: Access token stored separately for API requests

## When User Data is Updated

- ✅ After successful login
- ✅ After successful registration (auto-login)
- ✅ After token refresh
- ✅ On app initialization (if valid session exists)
- ❌ Does NOT auto-update when user changes profile (need to re-fetch or refresh token)

## Debugging

Check user data in browser console:

```javascript
// Get from localStorage
JSON.parse(localStorage.getItem("authUser"));

// Or in React DevTools
// Find AuthProvider component and check its state
```
