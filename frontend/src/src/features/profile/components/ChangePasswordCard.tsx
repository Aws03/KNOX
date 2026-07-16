import { useState, type FormEvent } from "react";
import { EyeIcon, EyeOffIcon, KeyRound, Loader2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/shared/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { changePassword } from "@/features/auth/api";

const ChangePasswordCard = () => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isCurrentVisible, setIsCurrentVisible] = useState(false);
  const [isNewVisible, setIsNewVisible] = useState(false);
  const [isConfirmVisible, setIsConfirmVisible] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    if (newPassword !== confirmPassword) {
      setError("New password and confirmation do not match.");
      return;
    }

    setIsSaving(true);
    try {
      await changePassword({ currentPassword, newPassword });
      toast.success("Password changed successfully!");
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
    } catch (err) {
      console.error("Failed to change password:", err);
      let errorMessage = "Failed to change password. Please try again.";

      if (err && typeof err === "object" && "response" in err) {
        const axiosError = err as {
          response?: { data?: { message?: string } };
        };
        errorMessage = axiosError.response?.data?.message || errorMessage;
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
      toast.error("Couldn't change password", { description: errorMessage });
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Card className="relative overflow-hidden border-slate-200/50 dark:border-slate-800/50 shadow-lg">
      <div className="absolute top-0 left-0 w-full h-1 bg-linear-to-r from-rose-500 to-pink-500" />
      <CardHeader>
        <CardTitle className="flex items-center gap-3 text-lg font-semibold">
          <div className="flex items-center justify-center h-10 w-10 rounded-xl bg-linear-to-br from-rose-500/10 to-pink-500/10 dark:from-rose-500/20 dark:to-pink-500/20">
            <KeyRound className="h-5 w-5 text-rose-600 dark:text-rose-400" />
          </div>
          Change Password
        </CardTitle>
      </CardHeader>
      <CardContent>
        <form className="space-y-4" onSubmit={handleSubmit}>
          {error && (
            <div className="rounded-md bg-destructive/15 p-3 text-sm text-destructive">
              {error}
            </div>
          )}

          <div className="space-y-2">
            <Label htmlFor="currentPassword" className="text-sm font-medium">
              Current Password*
            </Label>
            <div className="relative">
              <Input
                id="currentPassword"
                type={isCurrentVisible ? "text" : "password"}
                placeholder="••••••••••••••••"
                className="h-11 pr-9"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                required
                disabled={isSaving}
              />
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={() => setIsCurrentVisible((prev) => !prev)}
                className="text-muted-foreground focus-visible:ring-ring/50 absolute inset-y-0 right-0 rounded-l-none hover:bg-transparent"
                disabled={isSaving}
              >
                {isCurrentVisible ? <EyeOffIcon /> : <EyeIcon />}
                <span className="sr-only">
                  {isCurrentVisible ? "Hide password" : "Show password"}
                </span>
              </Button>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="newPassword" className="text-sm font-medium">
              New Password*
            </Label>
            <div className="relative">
              <Input
                id="newPassword"
                type={isNewVisible ? "text" : "password"}
                placeholder="••••••••••••••••"
                className="h-11 pr-9"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
                disabled={isSaving}
              />
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={() => setIsNewVisible((prev) => !prev)}
                className="text-muted-foreground focus-visible:ring-ring/50 absolute inset-y-0 right-0 rounded-l-none hover:bg-transparent"
                disabled={isSaving}
              >
                {isNewVisible ? <EyeOffIcon /> : <EyeIcon />}
                <span className="sr-only">
                  {isNewVisible ? "Hide password" : "Show password"}
                </span>
              </Button>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="confirmNewPassword" className="text-sm font-medium">
              Confirm New Password*
            </Label>
            <div className="relative">
              <Input
                id="confirmNewPassword"
                type={isConfirmVisible ? "text" : "password"}
                placeholder="••••••••••••••••"
                className="h-11 pr-9"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
                disabled={isSaving}
              />
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={() => setIsConfirmVisible((prev) => !prev)}
                className="text-muted-foreground focus-visible:ring-ring/50 absolute inset-y-0 right-0 rounded-l-none hover:bg-transparent"
                disabled={isSaving}
              >
                {isConfirmVisible ? <EyeOffIcon /> : <EyeIcon />}
                <span className="sr-only">
                  {isConfirmVisible ? "Hide password" : "Show password"}
                </span>
              </Button>
            </div>
          </div>

          <Button type="submit" disabled={isSaving} className="h-11">
            {isSaving ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Changing...
              </>
            ) : (
              "Change Password"
            )}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
};

export default ChangePasswordCard;
