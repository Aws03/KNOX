import { RefreshCw } from "lucide-react";

export const LoadingBanner = () => {
  return (
    <div className="flex items-center gap-3 p-4 bg-blue-50 dark:bg-blue-950/30 border border-blue-200 dark:border-blue-900 rounded-lg">
      <RefreshCw className="h-5 w-5 text-blue-600 dark:text-blue-400 animate-spin shrink-0" />
      <p className="text-sm text-blue-900 dark:text-blue-200">
        Loading complete user information...
      </p>
    </div>
  );
};
