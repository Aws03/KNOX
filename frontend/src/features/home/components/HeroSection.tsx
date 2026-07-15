import { useTranslation } from "react-i18next";
import { Badge } from "@/shared/ui/badge";
import { Button } from "@/shared/ui/button";
import { useNavigate } from "react-router-dom";
import {
  BookOpen,
  Brain,
  GraduationCap,
  Sparkles,
  ArrowRight,
  Play,
} from "lucide-react";

const HeroSection = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <section className="relative min-h-dvh overflow-hidden">
      {/* Animated Background */}
      <div className="absolute inset-0 -z-10">
        {/* Gradient Mesh Background */}
        <div className="absolute inset-0 bg-linear-to-br from-background via-background to-primary/5" />

        {/* Animated Gradient Orbs */}
        <div className="absolute -left-40 -top-40 h-80 w-80 animate-pulse rounded-full bg-primary/30 blur-[100px]" />
        <div className="absolute -right-40 top-1/4 h-96 w-96 animate-pulse rounded-full bg-primary/20 blur-[120px] delay-1000" />
        <div className="absolute bottom-0 left-1/3 h-72 w-72 animate-pulse rounded-full bg-primary/25 blur-[80px] delay-500" />

        {/* Grid Pattern */}
        <div className="absolute inset-0 bg-[linear-gradient(to_right,hsl(var(--primary)/0.03)_1px,transparent_1px),linear-gradient(to_bottom,hsl(var(--primary)/0.03)_1px,transparent_1px)] bg-size-[4rem_4rem]" />

        {/* Radial Fade */}
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,transparent_0%,hsl(var(--background))_70%)]" />
      </div>

      {/* Floating Elements */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute left-[10%] top-[20%] animate-float">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-primary/10 backdrop-blur-sm border border-primary/20 shadow-lg shadow-primary/10">
            <BookOpen className="h-7 w-7 text-primary" />
          </div>
        </div>
        <div className="absolute right-[15%] top-[25%] animate-float delay-1000">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10 backdrop-blur-sm border border-primary/20 shadow-lg shadow-primary/10">
            <Brain className="h-6 w-6 text-primary" />
          </div>
        </div>
        <div className="absolute left-[20%] bottom-[25%] animate-float delay-500">
          <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-primary/10 backdrop-blur-sm border border-primary/20 shadow-lg shadow-primary/10">
            <GraduationCap className="h-8 w-8 text-primary" />
          </div>
        </div>
        <div className="absolute right-[20%] bottom-[30%] animate-float delay-700">
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-primary/10 backdrop-blur-sm border border-primary/20 shadow-lg shadow-primary/10">
            <Sparkles className="h-5 w-5 text-primary" />
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="relative flex min-h-dvh flex-col items-center justify-center px-4 pt-16 pb-24 sm:px-6 lg:px-8">
        <div className="mx-auto max-w-5xl text-center">
          {/* Announcement Badge */}
          <div className="mb-8 inline-flex animate-fade-in">
            <div className="group relative inline-flex items-center gap-3 rounded-full border border-primary/20 bg-background/80 backdrop-blur-xl px-4 py-2 shadow-lg shadow-primary/5 transition-all hover:border-primary/40 hover:shadow-primary/10">
              <span className="absolute inset-0 rounded-full bg-linear-to-r from-primary/10 via-transparent to-primary/10 opacity-0 transition-opacity group-hover:opacity-100" />
              <Badge className="bg-primary text-primary-foreground border-0 px-2.5 py-0.5">
                <Sparkles className="mr-1 h-3 w-3" />
                {t("home.hero.badge.new")}
              </Badge>
              <span className="text-sm font-medium text-foreground">
                {t("home.hero.badge.text")}
              </span>
              <ArrowRight className="h-4 w-4 text-muted-foreground transition-transform group-hover:translate-x-1" />
            </div>
          </div>

          {/* Main Heading */}
          <h1 className="mb-6 text-4xl font-bold tracking-tight sm:text-5xl md:text-6xl lg:text-7xl animate-fade-in-up">
            <span className="block text-foreground">
              {t("home.hero.title.part1")}
            </span>
            <span className="relative mt-2 block">
              <span className="bg-linear-to-r from-primary via-primary/80 to-primary bg-clip-text text-transparent">
                {t("home.hero.title.part2")}
              </span>
              {/* Animated Underline */}
              <span className="absolute -bottom-2 left-1/2 h-1 w-0 -translate-x-1/2 rounded-full bg-linear-to-r from-transparent via-primary to-transparent animate-expand-width" />
            </span>
          </h1>

          {/* Subtitle */}
          <p className="mx-auto mb-10 max-w-2xl text-lg text-muted-foreground sm:text-xl animate-fade-in-up delay-200">
            {t("home.hero.subtitle")}
          </p>

          {/* CTA Buttons */}
          <div className="flex flex-col sm:flex-row items-center justify-center gap-4 animate-fade-in-up delay-300">
            <Button
              size="lg"
              className="group relative overflow-hidden bg-primary px-8 py-6 text-lg font-semibold shadow-lg shadow-primary/25 transition-all hover:shadow-xl hover:shadow-primary/30 hover:scale-105"
              onClick={() => navigate("/register")}
            >
              <span className="relative z-10 flex items-center gap-2">
                Get Started Free
                <ArrowRight className="h-5 w-5 transition-transform group-hover:translate-x-1" />
              </span>
              <span className="absolute inset-0 bg-linear-to-r from-primary via-primary/90 to-primary opacity-0 transition-opacity group-hover:opacity-100" />
            </Button>
            <Button
              size="lg"
              variant="outline"
              className="group px-8 py-6 text-lg font-semibold backdrop-blur-sm transition-all hover:bg-primary/5 hover:scale-105"
              onClick={() => navigate("/materials")}
            >
              <Play className="mr-2 h-5 w-5" />
              Watch Demo
            </Button>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
