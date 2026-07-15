import { Button } from "@/shared/ui/button";
import { Badge } from "@/shared/ui/badge";
import {
  Mail,
  Shield,
  CheckCircle2,
  LogOut,
  ShieldCheck,
  Edit3,
  Hash,
  Copy,
  Check,
} from "lucide-react";
import { useState } from "react";
import { Link } from "react-router-dom";
import type { User } from "@/app/providers/AuthContext";
import { getInitials, getRoleBadgeVariant } from "../utils/profileUtils";
import { ProfilePictureMenu } from "./ProfilePictureMenu";
import { ImageViewerDialog } from "./ImageViewerDialog";

interface ProfileHeaderProps {
  user: User;
  onLogout: () => void;
  onEditPhoto?: () => void;
  onDeletePhoto?: () => void;
  isDeletingPhoto?: boolean;
}

export const ProfileHeader = ({
  user,
  onLogout,
  onEditPhoto,
  onDeletePhoto,
  isDeletingPhoto = false,
}: ProfileHeaderProps) => {
  const displayName = user.fullName || user.name;
  const [copied, setCopied] = useState(false);
  const [isImageViewerOpen, setIsImageViewerOpen] = useState(false);

  const copyUserId = () => {
    navigator.clipboard.writeText(user.id);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleViewPhoto = () => {
    if (user.profilePictureUrl) {
      setIsImageViewerOpen(true);
    }
  };

  const handleEditPhoto = () => {
    if (onEditPhoto) {
      onEditPhoto();
    }
  };

  const handleDeletePhoto = () => {
    if (onDeletePhoto) {
      onDeletePhoto();
    }
  };

  return (
    <div className="relative overflow-hidden rounded-2xl bg-linear-to-br from-primary/10 via-primary/5 to-transparent dark:from-primary/20 dark:via-primary/10 border border-slate-200/50 dark:border-slate-800/50 shadow-xl">
      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-[0.03] dark:opacity-[0.05]">
        <div
          className="absolute inset-0"
          style={{
            backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23000000' fill-opacity='1'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
          }}
        />
      </div>

      <div className="relative p-8">
        {/* Top Section: User ID Badge */}
        <div className="flex justify-between items-start mb-6">
          <button
            onClick={copyUserId}
            className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-slate-100/80 dark:bg-slate-800/80 backdrop-blur-sm text-xs font-mono text-slate-600 dark:text-slate-400 hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors"
          >
            <Hash className="h-3 w-3" />
            <span className="max-w-[120px] truncate">{user.id}</span>
            {copied ? (
              <Check className="h-3 w-3 text-green-500" />
            ) : (
              <Copy className="h-3 w-3" />
            )}
          </button>

          {/* Action Buttons - Desktop */}
          <div className="hidden sm:flex gap-2">
            <Button
              variant="outline"
              size="sm"
              asChild
              className="bg-white/80 dark:bg-slate-900/80 backdrop-blur-sm"
            >
              <Link to="/profile/edit">
                <Edit3 className="h-4 w-4 mr-2" />
                Edit Profile
              </Link>
            </Button>
            <Button
              onClick={onLogout}
              variant="outline"
              size="sm"
              className="bg-white/80 dark:bg-slate-900/80 backdrop-blur-sm text-red-600 hover:text-red-700 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-950/50"
            >
              <LogOut className="h-4 w-4 mr-2" />
              Logout
            </Button>
          </div>
        </div>

        {/* Main Content */}
        <div className="flex flex-col sm:flex-row items-center sm:items-start gap-6">
          {/* Avatar with Dropdown Menu */}
          <div className="relative">
            <ProfilePictureMenu
              profilePictureUrl={user.profilePictureUrl || undefined}
              displayName={displayName}
              initials={getInitials(displayName)}
              onView={handleViewPhoto}
              onEdit={handleEditPhoto}
              onDelete={handleDeletePhoto}
              isDeleting={isDeletingPhoto}
              size="lg"
              showHoverEffect={true}
            />

            {/* Status indicator */}
            {user.isActive && (
              <div className="absolute -bottom-1 -right-1 h-6 w-6 bg-green-500 rounded-full border-4 border-white dark:border-slate-900 flex items-center justify-center">
                <Check className="h-3 w-3 text-white" />
              </div>
            )}
          </div>

          {/* User Info */}
          <div className="flex-1 text-center sm:text-left min-w-0">
            <h1 className="text-3xl sm:text-4xl font-bold text-slate-900 dark:text-white mb-2 tracking-tight">
              {displayName}
            </h1>
            <div className="flex items-center justify-center sm:justify-start gap-2 text-slate-600 dark:text-slate-400 mb-4">
              <Mail className="h-4 w-4 shrink-0" />
              <span className="text-sm truncate">{user.email}</span>
            </div>

            {/* Badges */}
            <div className="flex flex-wrap justify-center sm:justify-start gap-2">
              {user.role && (
                <Badge
                  variant="outline"
                  className={`${getRoleBadgeVariant(user.role)} px-3 py-1`}
                >
                  <Shield className="h-3.5 w-3.5 mr-1.5" />
                  {user.role === "User" ? "Student" : user.role}
                </Badge>
              )}
              {user.isActive && (
                <Badge
                  variant="outline"
                  className="bg-green-500/10 text-green-700 dark:text-green-400 border-green-200 dark:border-green-800 px-3 py-1"
                >
                  <CheckCircle2 className="h-3.5 w-3.5 mr-1.5" />
                  Active
                </Badge>
              )}
              {user.isVerified && (
                <Badge
                  variant="outline"
                  className="bg-blue-500/10 text-blue-700 dark:text-blue-400 border-blue-200 dark:border-blue-800 px-3 py-1"
                >
                  <ShieldCheck className="h-3.5 w-3.5 mr-1.5" />
                  Verified
                </Badge>
              )}
            </div>
          </div>
        </div>

        {/* Mobile Action Buttons */}
        <div className="flex sm:hidden gap-2 mt-6">
          <Button
            variant="outline"
            size="sm"
            asChild
            className="flex-1 bg-white/80 dark:bg-slate-900/80"
          >
            <Link to="/profile/edit">
              <Edit3 className="h-4 w-4 mr-2" />
              Edit Profile
            </Link>
          </Button>
          <Button
            onClick={onLogout}
            variant="outline"
            size="sm"
            className="flex-1 bg-white/80 dark:bg-slate-900/80 text-red-600 hover:text-red-700 hover:bg-red-50 dark:text-red-400"
          >
            <LogOut className="h-4 w-4 mr-2" />
            Logout
          </Button>
        </div>
      </div>

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
  );
};
