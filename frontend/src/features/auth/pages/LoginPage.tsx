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

const LoginPage = () => {
  const { t } = useTranslation();

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
          <p className="text-muted-foreground mb-6">
            {t("auth.login.loginWith")}{" "}
            <a href="#" className="text-card-foreground hover:underline">
              {t("auth.login.magicLink")}
            </a>
          </p>

          {/* Quick Login Buttons */}
          <div className="mb-6 flex flex-wrap gap-4 sm:gap-6">
            <Button variant="outline" className="grow">
              {t("auth.login.quickLogin.user")}
            </Button>
            <Button variant="outline" className="grow">
              {t("auth.login.quickLogin.admin")}
            </Button>
          </div>

          {/* Login Form */}
          <div className="space-y-4">
            <LoginForm />

            <p className="text-muted-foreground text-center">
              {t("auth.login.noAccount")}{" "}
              <a
                href="/auth/register"
                className="text-card-foreground hover:underline"
              >
                {t("auth.login.createAccount")}
              </a>
            </p>

            <div className="flex items-center gap-4">
              <Separator className="flex-1" />
              <p>{t("auth.login.or")}</p>
              <Separator className="flex-1" />
            </div>

            <Button variant="ghost" className="w-full" asChild>
              <a href="/google">{t("auth.login.signInWithGoogle")}</a>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default LoginPage;
