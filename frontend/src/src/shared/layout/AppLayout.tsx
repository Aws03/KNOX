import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { toast } from "sonner";

import Header from "@/shared/components/header/Header";
import Footer from "@/shared/components/footer/Footer";
import type { NavigationSection } from "@/shared/components/header/Header";
import { useAuth } from "@/app/providers/useAuth";
import { useUserRole } from "@/hooks/useUserRole";

interface AppLayoutProps {
  children: React.ReactNode;
}

type RoutedToast = {
  type: "success" | "error" | "info";
  title: string;
  description?: string;
  duration?: number;
};

const AppLayout = ({ children }: AppLayoutProps) => {
  const { isAuthenticated } = useAuth();
  const { hasRole } = useUserRole();

  // Only show Dashboard link if user is authenticated and has Writer or above permissions
  const canAccessDashboard =
    isAuthenticated && hasRole(["Writer", "Admin", "SuperAdmin"]);

  const navigationData: NavigationSection[] = [
    {
      title: "Home",
      href: "/",
    },
    {
      title: "About Us",
      href: "/about",
    },
    // {
    //   title: "Materials",
    //   href: "/materials",
    // },
    {
      title: "Courses",
      href: "/courses",
    },
    // {
    //   title: "Supervisor",
    //   href: "/supervisor",
    // },
    ...(canAccessDashboard
      ? [
          {
            title: "Dashboard",
            href: "/dashboard",
          },
        ]
      : []),
  ];

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const state = location.state as { toast?: RoutedToast } | null;
    const routedToast = state?.toast;
    if (!routedToast) return;

    const presenter =
      routedToast.type === "error"
        ? toast.error
        : routedToast.type === "info"
        ? toast.info
        : toast.success;

    presenter(routedToast.title, {
      description: routedToast.description,
      duration: routedToast.duration ?? 4000,
    });

    const rest = { ...(state ?? {}) } as Record<string, unknown>;
    delete rest.toast;
    navigate(
      {
        pathname: location.pathname,
        search: location.search,
        hash: location.hash,
      },
      {
        replace: true,
        state: Object.keys(rest).length ? rest : null,
      }
    );
  }, [location, navigate]);
  //
  return (
    <div className="flex min-h-screen flex-col">
      {/* Header */}
      <Header navigationData={navigationData} />

      {/* Main Content */}
      <main className="flex-1">{children}</main>

      {/* Footer */}
      <Footer />
    </div>
  );
};

export default AppLayout;
