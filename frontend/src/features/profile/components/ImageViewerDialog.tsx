import { Dialog, DialogContent } from "@/shared/ui/dialog";
import { X } from "lucide-react";
import { Button } from "@/shared/ui/button";

interface ImageViewerDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  imageUrl: string;
  displayName: string;
}

export const ImageViewerDialog = ({
  open,
  onOpenChange,
  imageUrl,
  displayName,
}: ImageViewerDialogProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl p-0 overflow-hidden bg-black/95">
        {/* Close button */}
        <Button
          onClick={() => onOpenChange(false)}
          variant="ghost"
          size="icon"
          className="absolute top-4 right-4 z-50 rounded-full bg-black/50 hover:bg-black/70 text-white"
        >
          <X className="h-5 w-5" />
        </Button>

        {/* Image */}
        <div className="relative w-full h-full flex items-center justify-center p-4">
          <img
            src={imageUrl}
            alt={displayName}
            className="max-w-full max-h-[85vh] object-contain rounded-lg"
          />
        </div>
      </DialogContent>
    </Dialog>
  );
};
