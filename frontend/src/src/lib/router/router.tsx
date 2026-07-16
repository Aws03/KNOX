import { createBrowserRouter } from "react-router-dom";
import App from "../../app/App";
import HomePage from "../../features/home/pages/HomePage";
import AboutPage from "../../features/home/pages/AboutPage";
import NotFoundPage from "@/features/home/pages/NotFoundPage";
import LoginPage from "@/features/auth/pages/LoginPage";
import RegisterPage from "@/features/auth/pages/RegisterPage";
import ForgotPasswordPage from "@/features/auth/pages/ForgotPasswordPage";
import ResetPasswordPage from "@/features/auth/pages/ResetPasswordPage";
import AuthLayout from "@/shared/layout/AuthLayout";
import CoursesPage from "@/features/courses/pages/CoursesPage";
import CourseInfoPage from "@/features/courses/pages/CourseInfoPage";
import FolderPage from "@/features/materials/pages/FolderPage";
import QuizzesPage from "@/features/quizzes/pages/QuizzesPage";
import QuizQuestionsPage from "@/features/quizzes/pages/QuizQuestionPage";
import AddQuizPage from "@/features/quizzes/pages/AddQuizPage";
import ProfilePage from "@/features/profile/pages/ProfilePage";
import EditProfilePage from "@/features/profile/pages/EditProfilePage";
import { ProtectedRoute } from "@/lib/router/ProtectedRoute";
import DashboardLayout from "@/shared/layout/DashboardLayout";
import AnalyticsPage from "@/features/dashboard/pages/AnalyticsPage";
import ManageUniversitiesPage from "@/features/universities/pages/ManageUniversitiesPage";
import UniversityDetailsPage from "@/features/universities/pages/UniversityDetailsPage";
import FacultyDetailsPage from "@/features/universities/pages/FacultyDetailsPage";
import ManageUsersPage from "@/features/users/pages/ManageUsersPage";
import WriterStatisticsPage from "@/features/dashboard/pages/WriterStatisticsPage";
import MajorCoursesPage from "@/features/courses/pages/MajorCoursesPage";
import MaterialExplorerPage from "@/features/materials/pages/MaterialExplorerPage";
import ManageResourcesPage from "@/features/courses/pages/ManageResourcesPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />, // Has Header + Footer
    children: [
      { index: true, element: <HomePage /> },
      { path: "about", element: <AboutPage /> },
      { path: "*", element: <NotFoundPage /> },
      { path: "courses", element: <CoursesPage /> },
      { path: "courses/:courseId/info", element: <CourseInfoPage /> },
      {
        path: "courses/:courseId/materials",
        element: <MaterialExplorerPage />,
      },
      { path: "courses/:courseId/folder/:folderId", element: <FolderPage /> },
      { path: "courses/:courseId/quizzes", element: <QuizzesPage /> },
      { path: "courses/:courseId/quizzes/add", element: <AddQuizPage /> },
      { path: "courses/:courseId/quizzes/new", element: <QuizQuestionsPage /> },
      {
        path: "courses/:courseId/quizzes/:quizId",
        element: <QuizQuestionsPage />,
      },
      {
        path: "profile",
        element: (
          <ProtectedRoute>
            <ProfilePage />
          </ProtectedRoute>
        ),
      },
      {
        path: "profile/edit",
        element: (
          <ProtectedRoute>
            <EditProfilePage />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    path: "/auth",
    element: <AuthLayout />, // No Header/Footer, just Outlet
    children: [
      { path: "login", element: <LoginPage /> },
      { path: "register", element: <RegisterPage /> },
      { path: "forgot-password", element: <ForgotPasswordPage /> },
      { path: "reset-password", element: <ResetPasswordPage /> },
    ],
  },
  {
    path: "/dashboard",
    element: (
      <ProtectedRoute requiredRoles={["Writer", "Admin", "SuperAdmin"]}>
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: (
          <ProtectedRoute requiredRoles={["SuperAdmin"]}>
            <AnalyticsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "universities",
        element: (
          <ProtectedRoute requiredRoles={["SuperAdmin"]}>
            <ManageUniversitiesPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "universities/:id",
        element: (
          <ProtectedRoute requiredRoles={["SuperAdmin"]}>
            <UniversityDetailsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "universities/:universityId/faculties/:facultyId",
        element: <FacultyDetailsPage />,
      },
      {
        path: "universities/:universityId/faculties/:facultyId/majors/:majorId",
        element: <MajorCoursesPage />,
      },
      // Dashboard management routes for materials, quizzes, and resources
      {
        path: "courses/:courseId/materials",
        element: <MaterialExplorerPage mode="manage" />,
      },
      {
        path: "courses/:courseId/folder/:folderId",
        element: <FolderPage mode="manage" />,
      },
      {
        path: "courses/:courseId/quizzes",
        element: <QuizzesPage mode="manage" />,
      },
      {
        path: "courses/:courseId/quizzes/add",
        element: <AddQuizPage />,
      },
      {
        path: "courses/:courseId/resources",
        element: <ManageResourcesPage />,
      },
      {
        path: "users",
        element: (
          <ProtectedRoute requiredRoles={["SuperAdmin"]}>
            <ManageUsersPage />
          </ProtectedRoute>
        ),
      },
      { path: "writer-statistics", element: <WriterStatisticsPage /> },
    ],
  },
]);
