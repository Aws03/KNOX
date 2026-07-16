import { Button } from "./button";
import { useTheme } from "../../app/providers/ThemeProvider";
import { useState } from "react";

export function ModeToggle() {
  const { theme, setTheme } = useTheme();
  const [isAnimating, setIsAnimating] = useState(false);

  // Determine the actual theme (resolving 'system' to actual value)
  const resolvedTheme =
    theme === "system"
      ? typeof window !== "undefined" &&
        window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "dark"
        : "light"
      : theme;

  const isDark = resolvedTheme === "dark";

  const toggleTheme = () => {
    setIsAnimating(true);
    setTimeout(() => {
      setTheme(isDark ? "light" : "dark");
      setTimeout(() => setIsAnimating(false), 200);
    }, 150);
  };

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={toggleTheme}
      className="relative h-9 w-9 rounded-full bg-accent/50 hover:bg-accent/70 border border-border/50 overflow-hidden transition-colors duration-300"
      aria-label={`Switch to ${isDark ? "light" : "dark"} mode`}
    >
      <div
        className={`absolute inset-0 flex items-center justify-center transition-all duration-500 ease-out ${
          isAnimating
            ? "scale-0 rotate-180 opacity-0"
            : "scale-100 rotate-0 opacity-100"
        }`}
      >
        {isDark ? (
          /* Sun icon - shows in dark mode */
          <div className="relative">
            <svg viewBox="0 0 24 24" fill="none" className="h-[18px] w-[18px]">
              {/* Sun center */}
              <circle cx="12" cy="12" r="4" className="fill-white" />
              {/* Sun rays */}
              <g className="stroke-white" strokeWidth="2" strokeLinecap="round">
                <line x1="12" y1="2" x2="12" y2="4" />
                <line x1="12" y1="20" x2="12" y2="22" />
                <line x1="4" y1="12" x2="2" y2="12" />
                <line x1="22" y1="12" x2="20" y2="12" />
                <line x1="5.64" y1="5.64" x2="4.22" y2="4.22" />
                <line x1="19.78" y1="19.78" x2="18.36" y2="18.36" />
                <line x1="5.64" y1="18.36" x2="4.22" y2="19.78" />
                <line x1="19.78" y1="4.22" x2="18.36" y2="5.64" />
              </g>
            </svg>
          </div>
        ) : (
          /* Moon icon - shows in light mode */
          <svg
            viewBox="0 0 24 24"
            fill="none"
            className="h-[18px] w-[18px] text-blue-400"
          >
            <path
              d="M21.752 15.002A9.718 9.718 0 0118 15.75c-5.385 0-9.75-4.365-9.75-9.75 0-1.33.266-2.597.748-3.752A9.753 9.753 0 003 11.25C3 16.635 7.365 21 12.75 21a9.753 9.753 0 009.002-5.998z"
              fill="currentColor"
              className="drop-shadow-sm"
            />
          </svg>
        )}
      </div>
      <span className="sr-only">Toggle theme</span>
    </Button>
  );
}
