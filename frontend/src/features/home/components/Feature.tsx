import { useTranslation } from "react-i18next";
import {
  Brain,
  BookOpen,
  Trophy,
  Sparkles,
  Users,
  Zap,
  Shield,
  Clock,
  BarChart3,
  Globe,
  Headphones,
} from "lucide-react";
import { Card, CardContent } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import { cn } from "@/lib/utils";

interface BentoItem {
  icon: React.ReactNode;
  title: string;
  description: string;
  gradient: string;
  span?: "default" | "wide" | "tall" | "large";
  stats?: { value: string; label: string };
}

const BentoCard = ({ item }: { item: BentoItem }) => {
  const spanClasses = {
    default: "",
    wide: "md:col-span-2",
    tall: "md:row-span-2",
    large: "md:col-span-2 md:row-span-2",
  };

  return (
    <Card
      className={cn(
        "group relative overflow-hidden border-0 bg-linear-to-br from-card/80 to-card/40 backdrop-blur-xl transition-all duration-500 hover:shadow-2xl hover:shadow-primary/10 hover:-translate-y-1",
        spanClasses[item.span || "default"]
      )}
    >
      {/* Gradient Background */}
      <div
        className={cn(
          "absolute inset-0 bg-linear-to-br opacity-0 transition-opacity duration-500 group-hover:opacity-100",
          item.gradient
        )}
      />

      {/* Animated Border */}
      <div className="absolute inset-0 rounded-xl bg-linear-to-r from-primary/0 via-primary/50 to-primary/0 opacity-0 blur-sm transition-opacity duration-500 group-hover:opacity-30" />

      {/* Inner Border */}
      <div className="absolute inset-px rounded-xl bg-card/90 backdrop-blur-xl" />

      <CardContent className="relative flex h-full flex-col justify-between p-6">
        {/* Icon */}
        <div
          className={cn(
            "mb-4 inline-flex h-14 w-14 items-center justify-center rounded-2xl bg-linear-to-br transition-all duration-300 group-hover:scale-110",
            item.gradient
          )}
        >
          <span className="text-foreground">{item.icon}</span>
        </div>

        {/* Content */}
        <div className="flex-1">
          <h3 className="mb-2 text-xl font-bold text-foreground">
            {item.title}
          </h3>
          <p className="text-muted-foreground leading-relaxed">
            {item.description}
          </p>
        </div>

        {/* Stats */}
        {item.stats && (
          <div className="mt-4 pt-4 border-t border-border/50">
            <div className="flex items-baseline gap-2">
              <span className="text-3xl font-bold bg-linear-to-r from-primary to-primary/60 bg-clip-text text-transparent">
                {item.stats.value}
              </span>
              <span className="text-sm text-muted-foreground">
                {item.stats.label}
              </span>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};

export const Feature = () => {
  const { t } = useTranslation();

  const bentoItems: BentoItem[] = [
    {
      icon: <BookOpen className="h-8 w-8" />,
      title: t("home.features.studyMaterials.title"),
      description: t("home.features.studyMaterials.description"),
      gradient: "from-blue-500/20 to-cyan-500/20",
      span: "wide",
    },
    {
      icon: <Trophy className="h-8 w-8" />,
      title: t("home.features.quizBank.title"),
      description: t("home.features.quizBank.description"),
      gradient: "from-amber-500/20 to-orange-500/20",
      span: "default",
    },
    {
      icon: <Brain className="h-8 w-8" />,
      title: t("home.features.aiSupervisor.title"),
      description: t("home.features.aiSupervisor.description"),
      gradient: "from-purple-500/20 to-pink-500/20",
      span: "tall",
    },
    {
      icon: <Users className="h-8 w-8" />,
      title: t("home.features.community.title"),
      description: t("home.features.community.description"),
      gradient: "from-green-500/20 to-emerald-500/20",
      span: "default",
    },
    {
      icon: <BarChart3 className="h-8 w-8" />,
      title: t("home.features.progressTracking.title"),
      description: t("home.features.progressTracking.description"),
      gradient: "from-rose-500/20 to-red-500/20",
      span: "default",
    },
    {
      icon: <Globe className="h-8 w-8" />,
      title: t("home.features.universityNetwork.title"),
      description: t("home.features.universityNetwork.description"),
      gradient: "from-indigo-500/20 to-blue-500/20",
      span: "wide",
    },
  ];

  const features = [
    {
      icon: <Zap className="h-5 w-5" />,
      title: t("home.features.lightningFast.title"),
      description: t("home.features.lightningFast.description"),
    },
    {
      icon: <Shield className="h-5 w-5" />,
      title: t("home.features.securePrivate.title"),
      description: t("home.features.securePrivate.description"),
    },
    {
      icon: <Clock className="h-5 w-5" />,
      title: t("home.features.alwaysUpdated.title"),
      description: t("home.features.alwaysUpdated.description"),
    },
    {
      icon: <Headphones className="h-5 w-5" />,
      title: t("home.features.support247.title"),
      description: t("home.features.support247.description"),
    },
  ];

  return (
    <section className="relative py-24 overflow-hidden">
      {/* Background Elements */}
      <div className="absolute inset-0 -z-10">
        <div className="absolute left-0 top-1/4 h-96 w-96 rounded-full bg-primary/10 blur-[120px]" />
        <div className="absolute right-0 bottom-1/4 h-96 w-96 rounded-full bg-primary/10 blur-[120px]" />
      </div>

      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section Header */}
        <div className="mb-16 text-center">
          <Badge
            variant="secondary"
            className="mb-4 px-4 py-1.5 text-sm font-medium"
          >
            <Sparkles className="mr-2 h-4 w-4" />
            Powerful Features
          </Badge>
          <h2 className="mb-6 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
            Everything You Need to
            <span className="block mt-2 bg-linear-to-r from-primary via-primary/80 to-primary bg-clip-text text-transparent">
              Excel in Your Studies
            </span>
          </h2>
          <p className="mx-auto max-w-2xl text-lg text-muted-foreground">
            Uni-Hub combines all the tools you need for academic success into
            one seamless platform designed specifically for university students.
          </p>
        </div>

        {/* Bento Grid */}
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3 md:gap-6">
          {bentoItems.map((item, index) => (
            <BentoCard key={index} item={item} />
          ))}
        </div>

        {/* Mini Features */}
        <div className="mt-16 grid grid-cols-2 gap-4 sm:grid-cols-4">
          {features.map((feature, index) => (
            <div
              key={index}
              className="group flex flex-col items-center gap-3 rounded-2xl border border-border/50 bg-card/50 backdrop-blur-sm p-6 text-center transition-all duration-300 hover:border-primary/30 hover:bg-card/80"
            >
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10 text-primary transition-all duration-300 group-hover:bg-primary group-hover:text-primary-foreground group-hover:scale-110">
                {feature.icon}
              </div>
              <h4 className="font-semibold text-foreground">{feature.title}</h4>
              <p className="text-sm text-muted-foreground">
                {feature.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
