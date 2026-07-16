import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Separator } from "@/shared/ui/separator";

import Logo from "@/assets/logo";
import AuthBackgroundShape from "@/assets/svg/auth-background-shape";
import LoginForm from "@/features/auth/components/login-form";

// Demo credentials for the "Quick Login" buttons - the seeded SuperAdmin
// account plus a student account (seeded separately). Clicking a button only
// fills the form fields; the user still clicks the real Sign In button.
const QUICK_LOGIN_CREDENTIALS = {
  user: { email: "student@knox.com", password: "Student@123456" },
  admin: { email: "admin@knox.com", password: "Admin@123456" },
} as const;

const LoginPage = () => {
  const { t } = useTranslation();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleQuickLogin = (role: "user" | "admin") => {
    const credentials = QUICK_LOGIN_CREDENTIALS[role];
    setEmail(credentials.email);
    setPassword(credentials.password);
  };

  return (
    <div className="relative flex h-auto min-h-screen items-center justify-center overflow-x-hidden px-4 py-10 sm:px-6 lg:px-8">
      <div className="absolute">
        <AuthBackgroundShape />
      </div>

      <Card className="z-1 w-full border-none shadow-md sm:max-w-lg">
        <CardHeader className="gap-6">
          <Logo className="gap-3" />

          <div>
            <CardTitle className="mb-1.5 text-2xl">
              {t("auth.login.title")}
            </CardTitle>
            <CardDescription className="text-base">
              {t("auth.login.description")}
            </CardDescription>
          </div>
        </CardHeader>

        <CardContent>
          {/* Quick Login Buttons */}
          <div className="mb-6 flex flex-wrap gap-4 sm:gap-6">
            <Button
              variant="outline"
              className="grow"
              onClick={() => handleQuickLogin("user")}
            >
              {t("auth.login.quickLogin.user")}
            </Button>
            <Button
              variant="outline"
              className="grow"
              onClick={() => handleQuickLogin("admin")}
            >
              {t("auth.login.quickLogin.admin")}
            </Button>
          </div>

          <div className="flex items-center gap-4 mb-6">
            <Separator className="flex-1" />
            <p className="text-muted-foreground text-sm">{t("auth.login.or")}</p>
            <Separator className="flex-1" />
          </div>

          {/* Login Form */}
          <div className="space-y-4">
            <LoginForm
              email={email}
              password={password}
              onEmailChange={setEmail}
              onPasswordChange={setPassword}
            />

            <p className="text-muted-foreground text-center">
              {t("auth.login.noAccount")}{" "}
              <a
                href="/auth/register"
                className="text-card-foreground hover:underline"
              >
                {t("auth.login.createAccount")}
              </a>
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default LoginPage;
