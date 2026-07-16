import { Button } from "@/shared/ui/button";
import { AlertCircle, RefreshCw } from "lucide-react";

interface ErrorBannerProps {
  error: string;
  onRetry: () => void;
  isRefreshing: boolean;
}

export const ErrorBanner = ({
  error,
  onRetry,
  isRefreshing,
}: ErrorBannerProps) => {
  return (
    <div className="flex items-center gap-3 p-4 bg-red-50 dark:bg-red-950/30 border border-red-200 dark:border-red-900 rounded-lg">
      <AlertCircle className="h-5 w-5 text-red-600 dark:text-red-400 shrink-0" />
      <p className="text-sm text-red-900 dark:text-red-200 flex-1">{error}</p>
      <Button
        onClick={onRetry}
        disabled={isRefreshing}
        variant="ghost"
        size="sm"
        className="text-red-700 dark:text-red-400"
      >
        {isRefreshing ? (
          <>
            <RefreshCw className="h-4 w-4 mr-2 animate-spin" />
            Refreshing...
          </>
        ) : (
          <>
            <RefreshCw className="h-4 w-4 mr-2" />
            Retry
          </>
        )}
      </Button>
    </div>
  );
};
