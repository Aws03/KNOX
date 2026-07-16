import { useState, useRef, useCallback, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/shared/ui/dialog";
import { Button } from "@/shared/ui/button";
import { Slider } from "@/shared/ui/slider";
import { Camera, Upload, ZoomIn, ZoomOut, RotateCw, X } from "lucide-react";

interface ImageCropperProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCropComplete: (croppedBlob: Blob) => void;
  aspectRatio?: number;
}

interface CropState {
  scale: number;
  rotation: number;
  translateX: number;
  translateY: number;
}

export const ImageCropper = ({
  open,
  onOpenChange,
  onCropComplete,
  aspectRatio = 1,
}: ImageCropperProps) => {
  const [imageSrc, setImageSrc] = useState<string | null>(null);
  const [cropState, setCropState] = useState<CropState>({
    scale: 1,
    rotation: 0,
    translateX: 0,
    translateY: 0,
  });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });

  const fileInputRef = useRef<HTMLInputElement>(null);
  const imageRef = useRef<HTMLImageElement | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const cropSize = 280;

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (!file.type.startsWith("image/")) {
      return;
    }

    const reader = new FileReader();
    reader.onload = (e) => {
      setImageSrc(e.target?.result as string);
      setCropState({
        scale: 1,
        rotation: 0,
        translateX: 0,
        translateY: 0,
      });
    };
    reader.readAsDataURL(file);
    event.target.value = "";
  };

  const handleMouseDown = (e: React.MouseEvent) => {
    if (!imageSrc) return;
    setIsDragging(true);
    setDragStart({
      x: e.clientX - cropState.translateX,
      y: e.clientY - cropState.translateY,
    });
  };

  const handleMouseMove = useCallback(
    (e: MouseEvent) => {
      if (!isDragging) return;
      setCropState((prev) => ({
        ...prev,
        translateX: e.clientX - dragStart.x,
        translateY: e.clientY - dragStart.y,
      }));
    },
    [isDragging, dragStart]
  );

  const handleMouseUp = useCallback(() => {
    setIsDragging(false);
  }, []);

  useEffect(() => {
    if (isDragging) {
      window.addEventListener("mousemove", handleMouseMove);
      window.addEventListener("mouseup", handleMouseUp);
      return () => {
        window.removeEventListener("mousemove", handleMouseMove);
        window.removeEventListener("mouseup", handleMouseUp);
      };
    }
  }, [isDragging, handleMouseMove, handleMouseUp]);

  const handleTouchStart = (e: React.TouchEvent) => {
    if (!imageSrc) return;
    const touch = e.touches[0];
    setIsDragging(true);
    setDragStart({
      x: touch.clientX - cropState.translateX,
      y: touch.clientY - cropState.translateY,
    });
  };

  const handleTouchMove = (e: React.TouchEvent) => {
    if (!isDragging) return;
    const touch = e.touches[0];
    setCropState((prev) => ({
      ...prev,
      translateX: touch.clientX - dragStart.x,
      translateY: touch.clientY - dragStart.y,
    }));
  };

  const handleTouchEnd = () => {
    setIsDragging(false);
  };

  const handleZoomChange = (value: number[]) => {
    setCropState((prev) => ({ ...prev, scale: value[0] }));
  };

  const handleRotate = () => {
    setCropState((prev) => ({ ...prev, rotation: (prev.rotation + 90) % 360 }));
  };

  const getCroppedImage = async (): Promise<Blob | null> => {
    if (!imageSrc || !imageRef.current) return null;

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    if (!ctx) return null;

    const outputSize = 1024; // Increased from 400 to 1024 for higher resolution
    canvas.width = outputSize;
    canvas.height = outputSize / aspectRatio;

    const img = imageRef.current;
    const containerSize = cropSize;

    // Calculate the crop area in the original image coordinates
    const scaleRatio =
      Math.min(
        containerSize / img.naturalWidth,
        containerSize / img.naturalHeight
      ) * cropState.scale;
    const scaledWidth = img.naturalWidth * scaleRatio;
    const scaledHeight = img.naturalHeight * scaleRatio;

    // Center of the container
    const containerCenterX = containerSize / 2;
    const containerCenterY = containerSize / 2;

    // Image center after translation
    const imageCenterX = containerCenterX + cropState.translateX;
    const imageCenterY = containerCenterY + cropState.translateY;

    // The crop area (the visible circle/square area)
    const cropLeft = (containerSize - cropSize) / 2;
    const cropTop = (containerSize - cropSize) / 2;

    // Calculate source rectangle
    const sourceX = (cropLeft - imageCenterX + scaledWidth / 2) / scaleRatio;
    const sourceY = (cropTop - imageCenterY + scaledHeight / 2) / scaleRatio;
    const sourceWidth = cropSize / scaleRatio;
    const sourceHeight = cropSize / aspectRatio / scaleRatio;

    // Handle rotation
    ctx.save();
    ctx.translate(canvas.width / 2, canvas.height / 2);
    ctx.rotate((cropState.rotation * Math.PI) / 180);
    ctx.translate(-canvas.width / 2, -canvas.height / 2);

    ctx.drawImage(
      img,
      sourceX,
      sourceY,
      sourceWidth,
      sourceHeight,
      0,
      0,
      canvas.width,
      canvas.height
    );

    ctx.restore();

    return new Promise((resolve) => {
      canvas.toBlob(
        (blob) => {
          resolve(blob);
        },
        "image/jpeg",
        0.95 // Increased from 0.9 to 0.95 for better quality
      );
    });
  };

  const handleConfirm = async () => {
    const blob = await getCroppedImage();
    if (blob) {
      onCropComplete(blob);
      setImageSrc(null);
      onOpenChange(false);
    }
  };

  const handleCancel = () => {
    setImageSrc(null);
    onOpenChange(false);
  };

  const handleImageLoad = (e: React.SyntheticEvent<HTMLImageElement>) => {
    imageRef.current = e.currentTarget;
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Camera className="h-5 w-5" />
            {imageSrc ? "Adjust Photo" : "Upload Photo"}
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          {!imageSrc ? (
            <div
              onClick={() => fileInputRef.current?.click()}
              className="flex flex-col items-center justify-center h-64 border-2 border-dashed border-slate-300 dark:border-slate-700 rounded-xl cursor-pointer hover:border-primary transition-colors bg-slate-50 dark:bg-slate-900"
            >
              <Upload className="h-12 w-12 text-slate-400 mb-4" />
              <p className="text-sm text-slate-600 dark:text-slate-400 text-center">
                Click to upload your photo
              </p>
              <p className="text-xs text-slate-400 mt-1">
                JPG, PNG or GIF (max 5MB)
              </p>
            </div>
          ) : (
            <>
              <div
                ref={containerRef}
                className="relative w-full aspect-square max-w-[320px] mx-auto bg-slate-900 rounded-xl overflow-hidden"
                style={{ cursor: isDragging ? "grabbing" : "grab" }}
                onMouseDown={handleMouseDown}
                onTouchStart={handleTouchStart}
                onTouchMove={handleTouchMove}
                onTouchEnd={handleTouchEnd}
              >
                {/* The image */}
                <div
                  className="absolute inset-0 flex items-center justify-center"
                  style={{
                    transform: `translate(${cropState.translateX}px, ${cropState.translateY}px) scale(${cropState.scale}) rotate(${cropState.rotation}deg)`,
                    transformOrigin: "center",
                    transition: isDragging ? "none" : "transform 0.1s ease-out",
                  }}
                >
                  <img
                    src={imageSrc}
                    alt="Crop preview"
                    className="max-w-none pointer-events-none select-none"
                    style={{
                      maxWidth: cropSize,
                      maxHeight: cropSize,
                      objectFit: "contain",
                    }}
                    onLoad={handleImageLoad}
                    draggable={false}
                  />
                </div>

                {/* Overlay with circular cutout */}
                <div className="absolute inset-0 pointer-events-none">
                  {/* Dark overlay */}
                  <div className="absolute inset-0 bg-black/60" />

                  {/* Circular transparent area */}
                  <div
                    className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-white shadow-[0_0_0_9999px_rgba(0,0,0,0.6)]"
                    style={{
                      width: cropSize * 0.85,
                      height: cropSize * 0.85,
                      boxShadow:
                        "0 0 0 9999px rgba(0,0,0,0.6), inset 0 0 0 1px rgba(255,255,255,0.3)",
                    }}
                  />

                  {/* Face guide - dashed oval */}
                  <div
                    className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-dashed border-white/40"
                    style={{
                      width: cropSize * 0.55,
                      height: cropSize * 0.7,
                    }}
                  />

                  {/* Helper text */}
                  <div className="absolute bottom-4 left-0 right-0 text-center">
                    <p className="text-xs text-white/70">
                      Drag to reposition • Pinch or use slider to zoom
                    </p>
                  </div>
                </div>
              </div>

              {/* Controls */}
              <div className="space-y-4 px-4">
                <div className="flex items-center gap-4">
                  <ZoomOut className="h-4 w-4 text-slate-400" />
                  <Slider
                    value={[cropState.scale]}
                    onValueChange={handleZoomChange}
                    min={0.5}
                    max={3}
                    step={0.01}
                    className="flex-1"
                  />
                  <ZoomIn className="h-4 w-4 text-slate-400" />
                </div>

                <div className="flex justify-center gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={handleRotate}
                    className="gap-2"
                  >
                    <RotateCw className="h-4 w-4" />
                    Rotate
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => fileInputRef.current?.click()}
                    className="gap-2"
                  >
                    <Upload className="h-4 w-4" />
                    Change Photo
                  </Button>
                </div>
              </div>
            </>
          )}
        </div>

        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          onChange={handleFileSelect}
          title="Select profile image"
          aria-label="Select profile image"
          className="hidden"
        />

        <DialogFooter className="gap-2">
          <Button variant="outline" onClick={handleCancel}>
            <X className="h-4 w-4 mr-2" />
            Cancel
          </Button>
          {imageSrc && (
            <Button onClick={handleConfirm}>
              <Camera className="h-4 w-4 mr-2" />
              Save Photo
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
