"use client";

import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { EyeIcon, EyeOffIcon, Loader2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/shared/ui/button";
import { Checkbox } from "@/shared/ui/checkbox";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { useAuth } from "@/app/providers/useAuth";

type LoginFormProps = {
  email: string;
  password: string;
  onEmailChange: (email: string) => void;
  onPasswordChange: (password: string) => void;
};

const LoginForm = ({
  email,
  password,
  onEmailChange,
  onPasswordChange,
}: LoginFormProps) => {
  const { t } = useTranslation();
  const [isVisible, setIsVisible] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      await login(email, password);
      navigate("/", {
        replace: true,
        state: {
          toast: {
            type: "success" as const,
            title: t("auth.login.success"),
            description: t("auth.login.welcomeBack"),
          },
        },
      });
    } catch (err) {
      console.error("Login error:", err);
      const errorMessage =
        err instanceof Error ? err.message : t("auth.login.loginFailed");
      setError(errorMessage);
      toast.error(t("auth.login.loginFailed"), {
        description: errorMessage,
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      {error && (
        <div className="rounded-md bg-destructive/15 p-3 text-sm text-destructive">
          {error}
        </div>
      )}

      {/* Email */}
      <div className="space-y-1">
        <Label htmlFor="userEmail" className="leading-5">
          {t("auth.login.emailLabel")}*
        </Label>
        <Input
          type="email"
          id="userEmail"
          placeholder={t("auth.login.emailPlaceholder")}
          value={email}
          onChange={(e) => onEmailChange(e.target.value)}
          required
          disabled={isLoading}
        />
      </div>

      {/* Password */}
      <div className="w-full space-y-1">
        <Label htmlFor="password" className="leading-5">
          {t("auth.login.passwordLabel")}*
        </Label>
        <div className="relative">
          <Input
            id="password"
            type={isVisible ? "text" : "password"}
            placeholder={t("auth.login.passwordPlaceholder")}
            className="pr-9"
            value={password}
            onChange={(e) => onPasswordChange(e.target.value)}
            required
            disabled={isLoading}
          />
          <Button
            type="button"
            variant="ghost"
            size="icon"
            onClick={() => setIsVisible((prevState) => !prevState)}
            className="text-muted-foreground focus-visible:ring-ring/50 absolute inset-y-0 right-0 rounded-l-none hover:bg-transparent"
            disabled={isLoading}
          >
            {isVisible ? <EyeOffIcon /> : <EyeIcon />}
            <span className="sr-only">
              {isVisible
                ? t("auth.login.hidePassword")
                : t("auth.login.showPassword")}
            </span>
          </Button>
        </div>
      </div>

      {/* Remember Me and Forgot Password */}
      <div className="flex items-center justify-between gap-y-2">
        <div className="flex items-center gap-3">
          <Checkbox
            id="rememberMe"
            className="size-6"
            checked={rememberMe}
            onCheckedChange={(checked) => setRememberMe(checked === true)}
            disabled={isLoading}
          />
          <Label htmlFor="rememberMe" className="text-muted-foreground">
            {" "}
            {t("auth.login.rememberMe")}
          </Label>
        </div>

        <Link to="/auth/forgot-password" className="hover:underline">
          {t("auth.login.forgotPassword")}
        </Link>
      </div>

      <Button className="w-full" type="submit" disabled={isLoading}>
        {isLoading ? (
          <>
            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            {t("auth.login.signingIn")}
          </>
        ) : (
          t("auth.login.submitButton")
        )}
      </Button>
    </form>
  );
};

export default LoginForm;
