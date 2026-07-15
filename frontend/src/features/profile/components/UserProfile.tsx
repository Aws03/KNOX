import { useAuth } from "@/app/providers/useAuth";
import { useEffect, useState } from "react";
import { useUserRefresh } from "../hooks/useUserRefresh";
import { ErrorBanner } from "./ErrorBanner";
import { LoadingBanner } from "./LoadingBanner";
import { ProfileHeader } from "./ProfileHeader";
import { AcademicInfoCard } from "./AcademicInfoCard";
import { AccountDetailsCard } from "./AccountDetailsCard";
import { ImageCropper } from "./ImageCropper";
import { uploadProfilePicture, deleteProfilePicture } from "../api";
import { toast } from "sonner";

export const UserProfile = () => {
  const { user, logout } = useAuth();
  const { isRefreshing, error, refetchUserInfo, checkAndRefetchIfNeeded } =
    useUserRefresh(user);
  const [isImageCropperOpen, setIsImageCropperOpen] = useState(false);
  const [isUploadingImage, setIsUploadingImage] = useState(false);
  const [isDeletingImage, setIsDeletingImage] = useState(false);

  useEffect(() => {
    checkAndRefetchIfNeeded(user);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleCropComplete = async (croppedBlob: Blob) => {
    try {
      setIsUploadingImage(true);
      const file = new File([croppedBlob], "profile-picture.jpg", {
        type: "image/jpeg",
      });
      await uploadProfilePicture(file);
      await refetchUserInfo();
      toast.success("Profile picture updated successfully!");
    } catch (err) {
      console.error("Failed to upload profile picture:", err);
      toast.error("Failed to upload profile picture");
    } finally {
      setIsUploadingImage(false);
    }
  };

  const handleDeleteProfilePicture = async () => {
    try {
      setIsDeletingImage(true);
      await deleteProfilePicture();
      await refetchUserInfo();
      toast.success("Profile picture removed successfully!");
    } catch (err) {
      console.error("Failed to delete profile picture:", err);
      toast.error("Failed to remove profile picture");
    } finally {
      setIsDeletingImage(false);
    }
  };

  if (!user) {
    return null;
  }

  return (
    <div className="w-full mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8 max-w-5xl">
      {/* Status Banners */}
      {error && (
        <ErrorBanner
          error={error}
          onRetry={refetchUserInfo}
          isRefreshing={isRefreshing}
        />
      )}
      {(isRefreshing || isUploadingImage || isDeletingImage) && !error && (
        <LoadingBanner />
      )}

      {/* Profile Header */}
      <ProfileHeader
        user={user}
        onLogout={logout}
        onEditPhoto={() => setIsImageCropperOpen(true)}
        onDeletePhoto={handleDeleteProfilePicture}
        isDeletingPhoto={isDeletingImage}
      />

      {/* Information Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <AcademicInfoCard user={user} />
        <AccountDetailsCard user={user} />
      </div>

      {/* Image Cropper Dialog */}
      <ImageCropper
        open={isImageCropperOpen}
        onOpenChange={setIsImageCropperOpen}
        onCropComplete={handleCropComplete}
      />
    </div>
  );
};
