import { Dialog, DialogContent } from "@/shared/ui/dialog";
import { X, ZoomIn, ZoomOut } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { useState } from "react";

interface QuizImageViewerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  imageUrl: string;
  alt?: string;
}

export const QuizImageViewer = ({
  open,
  onOpenChange,
  imageUrl,
  alt = "Image",
}: QuizImageViewerProps) => {
  const [zoom, setZoom] = useState(1);

  const handleZoomIn = () => {
    setZoom((prev) => Math.min(prev + 0.25, 3));
  };

  const handleZoomOut = () => {
    setZoom((prev) => Math.max(prev - 0.25, 0.5));
  };

  const handleClose = () => {
    setZoom(1);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-5xl p-0 overflow-hidden bg-black/95 border-0">
        {/* Controls */}
        <div className="absolute top-4 right-4 z-50 flex items-center gap-2">
          <Button
            onClick={handleZoomOut}
            variant="ghost"
            size="icon"
            className="rounded-full bg-black/50 hover:bg-black/70 text-white"
            disabled={zoom <= 0.5}
          >
            <ZoomOut className="h-5 w-5" />
          </Button>
          <span className="text-white text-sm font-medium px-2">
            {Math.round(zoom * 100)}%
          </span>
          <Button
            onClick={handleZoomIn}
            variant="ghost"
            size="icon"
            className="rounded-full bg-black/50 hover:bg-black/70 text-white"
            disabled={zoom >= 3}
          >
            <ZoomIn className="h-5 w-5" />
          </Button>
          <Button
            onClick={handleClose}
            variant="ghost"
            size="icon"
            className="rounded-full bg-black/50 hover:bg-black/70 text-white ml-2"
          >
            <X className="h-5 w-5" />
          </Button>
        </div>

        {/* Image */}
        <div className="relative w-full h-full flex items-center justify-center p-8 overflow-auto">
          <img
            src={imageUrl}
            alt={alt}
            className="max-w-full max-h-[85vh] object-contain rounded-lg transition-transform duration-200"
            style={{ transform: `scale(${zoom})` }}
          />
        </div>
      </DialogContent>
    </Dialog>
  );
};
