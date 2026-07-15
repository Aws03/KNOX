import { useState } from "react";
import { Share2, Check } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/shared/ui/tooltip";

type ShareButtonProps = {
  url: string;
  title?: string;
  variant?: "default" | "outline" | "ghost" | "link" | "secondary";
  size?: "default" | "sm" | "lg" | "icon";
  className?: string;
  showText?: boolean;
};

export default function ShareButton({
  url,
  title = "Share this link",
  variant = "outline",
  size = "sm",
  className = "",
  showText = false,
}: ShareButtonProps) {
  const [copied, setCopied] = useState(false);

  const handleShare = async (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();

    // Check if URL is already a full URL (starts with http:// or https://)
    const isExternalUrl =
      url.startsWith("http://") || url.startsWith("https://");
    const fullUrl = isExternalUrl ? url : `${window.location.origin}${url}`;

    // Try native share API first (mobile devices)
    if (navigator.share) {
      try {
        await navigator.share({
          title: title,
          url: fullUrl,
        });
        return;
      } catch (err) {
        // User cancelled or error occurred, fall back to copy
        if ((err as Error).name !== "AbortError") {
          console.error("Share failed:", err);
        }
      }
    }

    // Fallback to clipboard copy
    try {
      await navigator.clipboard.writeText(fullUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error("Failed to copy:", err);
      // Final fallback - create a temporary input
      const textArea = document.createElement("textarea");
      textArea.value = fullUrl;
      textArea.style.position = "fixed";
      textArea.style.left = "-999999px";
      document.body.appendChild(textArea);
      textArea.select();
      try {
        document.execCommand("copy");
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
      } catch (e) {
        console.error("Fallback copy failed:", e);
      }
      document.body.removeChild(textArea);
    }
  };

  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger asChild>
          <Button
            variant={variant}
            size={size}
            onClick={handleShare}
            className={className}
          >
            {copied ? (
              <Check className="h-4 w-4" />
            ) : (
              <Share2 className="h-4 w-4" />
            )}
            {showText && (
              <span className="ml-2">{copied ? "Copied!" : "Share"}</span>
            )}
          </Button>
        </TooltipTrigger>
        <TooltipContent>
          <p>{copied ? "Link copied!" : "Share link"}</p>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  );
}
