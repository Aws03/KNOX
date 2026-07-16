import { useState } from "react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from "@/shared/ui/dropdown-menu";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/shared/ui/alert-dialog";
import { Avatar, AvatarFallback, AvatarImage } from "@/shared/ui/avatar";
import { Camera, Eye, Trash2, Upload } from "lucide-react";

interface ProfilePictureMenuProps {
  profilePictureUrl?: string;
  displayName: string;
  initials: string;
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
  isDeleting?: boolean;
  size?: "sm" | "md" | "lg";
  showHoverEffect?: boolean;
}

export const ProfilePictureMenu = ({
  profilePictureUrl,
  displayName,
  initials,
  onView,
  onEdit,
  onDelete,
  isDeleting = false,
  size = "lg",
  showHoverEffect = true,
}: ProfilePictureMenuProps) => {
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const sizeClasses = {
    sm: "h-16 w-16",
    md: "h-24 w-24",
    lg: "h-32 w-32",
  };

  const handleDeleteClick = () => {
    setIsDropdownOpen(false);
    setIsDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = () => {
    onDelete();
    setIsDeleteDialogOpen(false);
  };

  const handleViewClick = () => {
    setIsDropdownOpen(false);
    onView();
  };

  const handleEditClick = () => {
    setIsDropdownOpen(false);
    onEdit();
  };

  return (
    <>
      <DropdownMenu open={isDropdownOpen} onOpenChange={setIsDropdownOpen}>
        <DropdownMenuTrigger asChild>
          <div className="relative group cursor-pointer">
            <Avatar
              className={`${
                sizeClasses[size]
              } ring-4 ring-white dark:ring-slate-900 shadow-2xl transition-all ${
                showHoverEffect ? "group-hover:ring-primary/50" : ""
              }`}
            >
              {profilePictureUrl && (
                <AvatarImage
                  src={profilePictureUrl}
                  alt={displayName}
                  className="object-cover"
                />
              )}
              <AvatarFallback className="bg-linear-to-br from-primary to-primary/70 text-white text-3xl font-bold">
                {initials}
              </AvatarFallback>
            </Avatar>

            {/* Hover overlay */}
            {showHoverEffect && (
              <div className="absolute inset-0 flex items-center justify-center bg-black/50 rounded-full opacity-0 group-hover:opacity-100 transition-opacity">
                <Camera className="h-8 w-8 text-white" />
              </div>
            )}
          </div>
        </DropdownMenuTrigger>

        <DropdownMenuContent align="center" className="w-48">
          {profilePictureUrl && (
            <>
              <DropdownMenuItem onClick={handleViewClick}>
                <Eye className="h-4 w-4 mr-2" />
                View Photo
              </DropdownMenuItem>
              <DropdownMenuSeparator />
            </>
          )}
          <DropdownMenuItem onClick={handleEditClick}>
            {profilePictureUrl ? (
              <>
                <Camera className="h-4 w-4 mr-2" />
                Change Photo
              </>
            ) : (
              <>
                <Upload className="h-4 w-4 mr-2" />
                Upload Photo
              </>
            )}
          </DropdownMenuItem>
          {profilePictureUrl && (
            <>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={handleDeleteClick}
                className="text-red-600 focus:text-red-600 dark:text-red-400 dark:focus:text-red-400"
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Remove Photo
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>

      {/* Delete Confirmation Dialog */}
      <AlertDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Remove Profile Picture?</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to remove your profile picture? This action
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteConfirm}
              disabled={isDeleting}
              className="bg-red-600 hover:bg-red-700 focus:ring-red-600"
            >
              {isDeleting ? "Removing..." : "Remove"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
