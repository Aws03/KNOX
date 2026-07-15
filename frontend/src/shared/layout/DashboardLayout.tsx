import { useLocation, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import {
  ChartBarIcon,
  FolderTree,
  Users,
  PenTool,
  HomeIcon,
} from "lucide-react";
import { useAuth } from "@/app/providers/useAuth";
import { useUserRole } from "@/hooks/useUserRole";
import Logo from "@/assets/logo";
import { fetchMajorById } from "@/features/courses/api";
import { useTranslation } from "react-i18next";

import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/shared/ui/breadcrumb";

import LanguageToggle from "../components/header/LanguageToggle";
import { ModeToggle } from "../ui/modetoggle";

import { Avatar, AvatarFallback, AvatarImage } from "@/shared/ui/avatar";
import { Button } from "@/shared/ui/button";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
} from "@/shared/ui/sidebar";

import ProfileDropdown from "@/features/dashboard/componenets/dropdown-profile";
import { Outlet } from "react-router-dom";

const DashboardLayout = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { user } = useAuth();
  const { isSuperAdmin, canManageContent, hasRole } = useUserRole();
  const [majorName, setMajorName] = useState<string | null>(null);
  const { i18n } = useTranslation();

  // Determine sidebar side based on language direction
  const isRTL = i18n.language === "ar";
  const sidebarSide = isRTL ? "right" : "left";

  // Generate initials from user name
  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  const displayName = user?.fullName || user?.name || "User";
  const avatarUrl = user?.profilePictureUrl || undefined;
  const initials = getInitials(displayName);

  // Redirect non-SuperAdmin users to their faculty page
  useEffect(() => {
    if (!isSuperAdmin && user) {
      // Redirect from /dashboard root or /dashboard/universities to their faculty
      if (
        location.pathname === "/dashboard" ||
        location.pathname === "/dashboard/universities"
      ) {
        if (user.universityId && user.facultyId) {
          navigate(
            `/dashboard/universities/${user.universityId}/faculties/${user.facultyId}`,
            { replace: true }
          );
        }
      }
    }
  }, [location.pathname, isSuperAdmin, user, navigate]);

  // Fetch major name from URL if present
  useEffect(() => {
    const fetchMajorName = async () => {
      const pathParts = location.pathname.split("/").filter(Boolean);
      const majorIndex = pathParts.indexOf("majors");

      if (majorIndex !== -1 && pathParts[majorIndex + 1]) {
        const majorId = parseInt(pathParts[majorIndex + 1]);
        if (!isNaN(majorId)) {
          try {
            const major = await fetchMajorById(majorId);
            setMajorName(major.name);
          } catch (error) {
            console.error("Failed to fetch major name:", error);
            setMajorName(null);
          }
        }
      } else {
        setMajorName(null);
      }
    };

    fetchMajorName();
  }, [location.pathname]);

  const getActivePage = () => {
    if (location.pathname === "/dashboard") return "analytics";
    if (location.pathname.startsWith("/dashboard/universities"))
      return "universities";
    if (location.pathname.startsWith("/dashboard/courses"))
      return "universities";
    if (location.pathname.startsWith("/dashboard/users")) return "users";
    if (location.pathname.startsWith("/dashboard/writer-statistics"))
      return "writer-statistics";
    return "analytics";
  };

  const getBreadcrumbs = () => {
    const path = location.pathname;
    const breadcrumbs: { label: string; href?: string }[] = [];

    if (path === "/dashboard") {
      breadcrumbs.push({ label: "Platform Statistics" });
    } else if (path.startsWith("/dashboard/users")) {
      breadcrumbs.push({ label: "Manage Users" });
    } else if (path.startsWith("/dashboard/writer-statistics")) {
      breadcrumbs.push({ label: "Writer Statistics" });
    } else if (path.startsWith("/dashboard/courses")) {
      // Handle courses materials and folders
      const parts = path.split("/").filter(Boolean);
      breadcrumbs.push({ label: "Content Management" });

      // Add university and faculty if available from user
      if (user?.universityName) {
        breadcrumbs.push({ label: user.universityName });
      }
      if (user?.facultyName) {
        breadcrumbs.push({ label: user.facultyName });
      }

      if (parts.includes("materials")) {
        breadcrumbs.push({ label: "Materials" });
      } else if (parts.includes("folder")) {
        breadcrumbs.push({
          label: "Materials",
          href: `/dashboard/courses/${parts[2]}/materials`,
        });
        breadcrumbs.push({ label: "Folder" });
      } else if (parts.includes("quizzes")) {
        breadcrumbs.push({ label: "Quizzes" });
      }
    } else if (path.includes("/universities")) {
      const parts = path.split("/").filter(Boolean);
      breadcrumbs.push({ label: "Content Management" });

      if (parts.length === 2) {
        // /dashboard/universities - SuperAdmin view
        breadcrumbs.push({ label: "All Universities" });
      } else if (parts.length === 3) {
        // /dashboard/universities/:id - University details
        if (isSuperAdmin) {
          breadcrumbs.push({
            label: "Universities",
            href: "/dashboard/universities",
          });
        }
        breadcrumbs.push({ label: user?.universityName || "University" });
      } else if (parts.length >= 5 && parts[3] === "faculties") {
        // /dashboard/universities/:id/faculties/:facultyId
        const universityId = parts[2];
        if (isSuperAdmin) {
          breadcrumbs.push({
            label: "Universities",
            href: "/dashboard/universities",
          });
          breadcrumbs.push({
            label: user?.universityName || "University",
            href: `/dashboard/universities/${universityId}`,
          });
        } else {
          // For Writer/Admin, show their university name
          breadcrumbs.push({ label: user?.universityName || "University" });
        }
        breadcrumbs.push({ label: user?.facultyName || "Faculty" });

        // Check if there's a major in the path
        if (parts.length >= 7 && parts[5] === "majors") {
          breadcrumbs.push({ label: majorName || "Major" });
        }
      }
    }

    return breadcrumbs;
  };

  const breadcrumbs = getBreadcrumbs();

  return (
    <div className="flex min-h-dvh w-full">
      <SidebarProvider>
        <Sidebar side={sidebarSide}>
          {/* Sidebar Header with Logo */}
          <SidebarHeader className="border-b px-4 py-4">
            <div className="flex items-center gap-3">
              <Logo className="size-9" />
              <span className="text-xl font-bold tracking-tight">KNOX</span>
            </div>
          </SidebarHeader>

          <SidebarContent>
            <SidebarGroup>
              <SidebarGroupContent>
                <SidebarMenu>
                  {/* Platform Statistics - SuperAdmin only */}
                  {isSuperAdmin && (
                    <SidebarMenuItem>
                      <SidebarMenuButton
                        isActive={getActivePage() === "analytics"}
                        onClick={() => navigate("/dashboard")}
                      >
                        <ChartBarIcon />
                        <span>Platform Statistics</span>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  )}

                  {/* Content Management - Writer, Admin, SuperAdmin */}
                  {canManageContent() && (
                    <SidebarMenuItem>
                      <SidebarMenuButton
                        isActive={getActivePage() === "universities"}
                        onClick={() => {
                          if (isSuperAdmin) {
                            navigate("/dashboard/universities");
                          } else if (user?.universityId && user?.facultyId) {
                            navigate(
                              `/dashboard/universities/${user.universityId}/faculties/${user.facultyId}`
                            );
                          }
                        }}
                      >
                        <FolderTree />
                        <span>Content Management</span>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  )}

                  {/* Manage Users - SuperAdmin only */}
                  {isSuperAdmin && (
                    <SidebarMenuItem>
                      <SidebarMenuButton
                        isActive={getActivePage() === "users"}
                        onClick={() => navigate("/dashboard/users")}
                      >
                        <Users />
                        <span>Manage Users</span>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  )}

                  {/* Writer Statistics - Writer, Admin, SuperAdmin */}
                  {hasRole(["Writer", "Admin", "SuperAdmin"]) && (
                    <SidebarMenuItem>
                      <SidebarMenuButton
                        isActive={getActivePage() === "writer-statistics"}
                        onClick={() => navigate("/dashboard/writer-statistics")}
                      >
                        <PenTool />
                        <span>Writer Statistics</span>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  )}
                </SidebarMenu>
              </SidebarGroupContent>
            </SidebarGroup>
          </SidebarContent>

          {/* Sidebar Footer with Exit Button */}
          <SidebarFooter className="mt-auto border-t p-2">
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton
                  onClick={() => navigate("/")}
                  className="w-full  hover:bg-red-700 hover:text-white text-red-500 font-bold hover:cursor-pointer focus-visible:ring-red-600"
                >
                  <HomeIcon strokeWidth={2.9} />
                  <span>Exit Dashboard</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarFooter>
        </Sidebar>
        <div className="flex flex-1 flex-col">
          <header className="bg-card sticky top-0 z-50 border-b">
            <div className="mx-auto flex max-w-7xl items-center justify-between gap-6 px-4 py-3 sm:px-6">
              <div className="flex items-center gap-4">
                <SidebarTrigger className="[&_svg]:size-5!" />
                {breadcrumbs.length > 0 && (
                  <Breadcrumb>
                    <BreadcrumbList>
                      {breadcrumbs.map((crumb, index) => (
                        <div key={index} className="flex items-center gap-2">
                          {index > 0 && <BreadcrumbSeparator />}
                          <BreadcrumbItem>
                            {crumb.href ? (
                              <BreadcrumbLink
                                onClick={() => navigate(crumb.href!)}
                                className="cursor-pointer"
                              >
                                {crumb.label}
                              </BreadcrumbLink>
                            ) : (
                              <BreadcrumbPage>{crumb.label}</BreadcrumbPage>
                            )}
                          </BreadcrumbItem>
                        </div>
                      ))}
                    </BreadcrumbList>
                  </Breadcrumb>
                )}
              </div>
              <div className="flex items-center gap-1.5">
                <LanguageToggle />
                <div className="mr-2">
                  <ModeToggle />
                </div>
                <ProfileDropdown
                  trigger={
                    <Button variant="ghost" size="icon" className="size-9.5">
                      <Avatar className="size-9.5 rounded-md">
                        <AvatarImage src={avatarUrl} alt={displayName} />
                        <AvatarFallback>{initials}</AvatarFallback>
                      </Avatar>
                    </Button>
                  }
                />
              </div>
            </div>
          </header>
          <main className="mx-auto size-full max-w-7xl flex-1 px-4 py-6 sm:px-6">
            <Outlet />
          </main>
          {/* <footer>
            <div className="text-muted-foreground mx-auto flex size-full max-w-7xl items-center justify-between gap-3 px-4 py-3 max-sm:flex-col sm:gap-6 sm:px-6">
              <p className="text-sm text-balance max-sm:text-center">
                {`©${new Date().getFullYear()}`}{" "}
                <a href="#" className="text-primary">
                  Uni-Hub
                </a>
                , Empowering students worldwide
              </p>
              <div className="flex items-center gap-5">
                <a href="#">
                  <FacebookIcon className="size-4" />
                </a>
                <a href="#">
                  <InstagramIcon className="size-4" />
                </a>
                <a href="#">
                  <LinkedinIcon className="size-4" />
                </a>
                <a href="#">
                  <TwitterIcon className="size-4" />
                </a>
              </div>
            </div>
          </footer> */}
        </div>
      </SidebarProvider>
    </div>
  );
};

export default DashboardLayout;
