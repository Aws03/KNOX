import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/app/providers/useAuth";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { ArrowLeft, Camera, Save, Loader2, User } from "lucide-react";
import { toast } from "sonner";
import { getInitials } from "../utils/profileUtils";
import { ImageCropper } from "../components/ImageCropper";
import { ProfilePictureMenu } from "../components/ProfilePictureMenu";
import { ImageViewerDialog } from "../components/ImageViewerDialog";
import {
  updateProfile,
  uploadProfilePicture,
  deleteProfilePicture,
} from "../api";
import { useUserRefresh } from "../hooks/useUserRefresh";
import { CascadingSelects } from "@/shared/components/forms/CascadingSelects";
import { useAcademicSelection } from "@/features/auth/hooks/useAcademicSelection";
import type {
  University,
  Faculty,
  Major,
} from "@/shared/components/forms/CascadingSelects";

const EditProfilePage = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { refetchUserInfo } = useUserRefresh(user);

  const [fullName, setFullName] = useState(user?.fullName || user?.name || "");
  const [isImageCropperOpen, setIsImageCropperOpen] = useState(false);
  const [isImageViewerOpen, setIsImageViewerOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isUploadingImage, setIsUploadingImage] = useState(false);
  const [isDeletingImage, setIsDeletingImage] = useState(false);

  const {
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    setSelectedUniversity,
    setSelectedFaculty,
    setSelectedMajor,
    fetchUniversities,
    fetchFaculties,
    fetchMajors,
  } = useAcademicSelection();

  // Initialize academic selections from user data
  useEffect(() => {
    if (user) {
      if (user.universityId && user.universityName) {
        setSelectedUniversity({
          id: user.universityId,
          name: user.universityName,
        });
      }
      if (user.facultyId && user.facultyName) {
        setSelectedFaculty({
          id: user.facultyId,
          name: user.facultyName,
          universityId: user.universityId || 0,
        } as Faculty);
      }
      if (user.majorId && user.majorName) {
        setSelectedMajor({
          id: user.majorId,
          name: user.majorName,
          facultyId: user.facultyId || 0,
        } as Major);
      }
    }
  }, [user, setSelectedUniversity, setSelectedFaculty, setSelectedMajor]);

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

  const handleSave = async () => {
    try {
      setIsSaving(true);
      await updateProfile({
        fullName: fullName.trim() || undefined,
        universityId: selectedUniversity?.id,
        facultyId: selectedFaculty?.id,
        majorId: selectedMajor?.id,
      });
      await refetchUserInfo();
      toast.success("Profile updated successfully!");
      navigate("/profile");
    } catch (err) {
      console.error("Failed to update profile:", err);
      toast.error("Failed to update profile");
    } finally {
      setIsSaving(false);
    }
  };

  if (!user) {
    return null;
  }

  const displayName = user.fullName || user.name;

  return (
    <div className="min-h-[calc(100vh-4rem)] bg-linear-to-b from-slate-50 to-white dark:from-slate-950 dark:to-slate-900">
      <div className="container mx-auto py-8 lg:py-12 px-4 sm:px-6 lg:px-8 max-w-3xl">
        {/* Header */}
        <div className="flex items-center gap-4 mb-8">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate("/profile")}
            className="rounded-full"
          >
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div>
            <h1 className="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">
              Edit Profile
            </h1>
            <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">
              Update your personal information
            </p>
          </div>
        </div>

        <div className="space-y-6">
          {/* Profile Picture Card */}
          <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg">
            <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-primary to-primary/70" />
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-primary/10 to-primary/5 dark:from-primary/20 dark:to-primary/10">
                  <Camera className="h-5 w-5 text-primary" />
                </div>
                Profile Picture
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex flex-col sm:flex-row items-center gap-6">
                <div className="flex items-center justify-center">
                  {isUploadingImage || isDeletingImage ? (
                    <div className="h-32 w-32 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center">
                      <Loader2 className="h-8 w-8 animate-spin text-primary" />
                    </div>
                  ) : (
                    <ProfilePictureMenu
                      profilePictureUrl={user.profilePictureUrl || undefined}
                      displayName={displayName}
                      initials={getInitials(displayName)}
                      onView={() => setIsImageViewerOpen(true)}
                      onEdit={() => setIsImageCropperOpen(true)}
                      onDelete={handleDeleteProfilePicture}
                      isDeleting={isDeletingImage}
                      size="lg"
                      showHoverEffect={true}
                    />
                  )}
                </div>

                <div className="flex flex-col gap-3 text-center sm:text-left">
                  <p className="text-sm text-slate-600 dark:text-slate-400">
                    Click on your avatar to view, upload, or change your profile
                    picture.
                    <br />
                    <span className="text-xs text-slate-400">
                      Recommended: Square image, at least 1000x1000px
                    </span>
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Personal Info Card */}
          <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg">
            <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-blue-500 to-cyan-500" />
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-blue-500/10 to-cyan-500/10 dark:from-blue-500/20 dark:to-cyan-500/20">
                  <User className="h-5 w-5 text-blue-600 dark:text-blue-400" />
                </div>
                Personal Information
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="space-y-2">
                <Label htmlFor="fullName" className="text-sm font-medium">
                  Full Name
                </Label>
                <Input
                  id="fullName"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  placeholder="Enter your full name"
                  className="h-11"
                />
              </div>

              <div className="space-y-2">
                <Label className="text-sm font-medium text-slate-500">
                  Email Address
                </Label>
                <Input
                  value={user.email}
                  disabled
                  className="h-11 bg-slate-50 dark:bg-slate-800/50"
                />
                <p className="text-xs text-slate-400">
                  Email cannot be changed
                </p>
              </div>
            </CardContent>
          </Card>

          {/* Academic Info Card */}
          <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg">
            <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-amber-500 to-orange-500" />
            <CardHeader>
              <CardTitle className="flex items-center gap-3 text-lg font-semibold">
                <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-amber-500/10 to-orange-500/10 dark:from-amber-500/20 dark:to-orange-500/20">
                  <svg
                    className="h-5 w-5 text-amber-600 dark:text-amber-400"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={2}
                  >
                    <path d="M12 14l9-5-9-5-9 5 9 5z" />
                    <path d="M12 14l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14z" />
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M12 14l9-5-9-5-9 5 9 5zm0 0l6.16-3.422a12.083 12.083 0 01.665 6.479A11.952 11.952 0 0012 20.055a11.952 11.952 0 00-6.824-2.998 12.078 12.078 0 01.665-6.479L12 14zm-4 6v-7.5l4-2.222"
                    />
                  </svg>
                </div>
                Academic Information
              </CardTitle>
            </CardHeader>
            <CardContent>
              <CascadingSelects
                selectedUniversity={selectedUniversity as University | null}
                selectedFaculty={selectedFaculty}
                selectedMajor={selectedMajor}
                onUniversityChange={setSelectedUniversity}
                onFacultyChange={setSelectedFaculty}
                onMajorChange={setSelectedMajor}
                fetchUniversities={fetchUniversities}
                fetchFaculties={fetchFaculties}
                fetchMajors={fetchMajors}
              />
            </CardContent>
          </Card>

          {/* Action Buttons */}
          <div className="flex flex-col sm:flex-row gap-3 pt-4">
            <Button
              variant="outline"
              onClick={() => navigate("/profile")}
              className="flex-1 h-11"
            >
              Cancel
            </Button>
            <Button
              onClick={handleSave}
              disabled={isSaving}
              className="flex-1 h-11"
            >
              {isSaving ? (
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
              ) : (
                <Save className="h-4 w-4 mr-2" />
              )}
              Save Changes
            </Button>
          </div>
        </div>

        {/* Image Cropper Dialog */}
        <ImageCropper
          open={isImageCropperOpen}
          onOpenChange={setIsImageCropperOpen}
          onCropComplete={handleCropComplete}
        />

        {/* Image Viewer Dialog */}
        {user.profilePictureUrl && (
          <ImageViewerDialog
            open={isImageViewerOpen}
            onOpenChange={setIsImageViewerOpen}
            imageUrl={user.profilePictureUrl}
            displayName={displayName}
          />
        )}
      </div>
    </div>
  );
};

export default EditProfilePage;
