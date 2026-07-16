import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { Toaster } from "sonner";
import { I18nProvider } from "./providers/I18nProvider.tsx";
import { QueryProvider } from "./providers/QueryProvider.tsx";
import { RouterProviderApp } from "./providers/RouterProviderApp.tsx";
import { ThemeProvider } from "./providers/ThemeProvider.tsx";
// Switched to Axios-based AuthProvider implementation
import { AuthProvider } from "@/app/providers/AuthProvider.tsx";
import { ErrorBoundary } from "@/shared/components/ErrorBoundary.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <AuthProvider>
          <I18nProvider>
            <QueryProvider>
              <RouterProviderApp />
              <Toaster richColors position="top-right" />
            </QueryProvider>
          </I18nProvider>
        </AuthProvider>
      </ThemeProvider>
    </ErrorBoundary>
  </StrictMode>
);
