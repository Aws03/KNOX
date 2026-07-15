import { Badge } from "@/shared/ui/badge";
import { UserPlus, Search, BookOpen, Trophy, ArrowRight } from "lucide-react";

const steps = [
  {
    icon: <UserPlus className="h-6 w-6" />,
    number: "01",
    title: "Create Account",
    description:
      "Sign up for free in seconds. No credit card required to get started with Uni-Hub.",
  },
  {
    icon: <Search className="h-6 w-6" />,
    number: "02",
    title: "Find Your Courses",
    description:
      "Search and select your university and courses. We'll personalize your learning experience.",
  },
  {
    icon: <BookOpen className="h-6 w-6" />,
    number: "03",
    title: "Access Materials",
    description:
      "Explore comprehensive study materials, notes, and resources curated for your courses.",
  },
  {
    icon: <Trophy className="h-6 w-6" />,
    number: "04",
    title: "Excel & Succeed",
    description:
      "Practice with quizzes, get AI guidance, and track your progress to academic success.",
  },
];

const HowItWorksSection = () => {
  return (
    <section className="relative py-24 overflow-hidden bg-linear-to-b from-background to-primary/5">
      {/* Background */}
      <div className="absolute inset-0 -z-10">
        <div className="absolute left-1/2 top-0 h-px w-[80%] -translate-x-1/2 bg-linear-to-r from-transparent via-primary/30 to-transparent" />
        <div className="absolute left-1/2 bottom-0 h-px w-[80%] -translate-x-1/2 bg-linear-to-r from-transparent via-primary/30 to-transparent" />
      </div>

      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section Header */}
        <div className="mb-16 text-center">
          <Badge
            variant="secondary"
            className="mb-4 px-4 py-1.5 text-sm font-medium"
          >
            Simple Process
          </Badge>
          <h2 className="mb-6 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
            How It
            <span className="ml-3 bg-linear-to-r from-primary via-primary/80 to-primary bg-clip-text text-transparent">
              Works
            </span>
          </h2>
          <p className="mx-auto max-w-2xl text-lg text-muted-foreground">
            Get started with Uni-Hub in four simple steps and begin your journey
            to academic excellence.
          </p>
        </div>

        {/* Steps */}
        <div className="relative">
          {/* Connection Line - Desktop */}
          <div className="absolute left-0 right-0 top-1/2 hidden h-0.5 -translate-y-1/2 bg-linear-to-r from-transparent via-primary/30 to-transparent lg:block" />

          <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-4">
            {steps.map((step, index) => (
              <div key={index} className="group relative">
                {/* Arrow - Desktop */}
                {index < steps.length - 1 && (
                  <div className="absolute -right-4 top-1/2 z-10 hidden -translate-y-1/2 lg:block">
                    <ArrowRight className="h-6 w-6 text-primary/50" />
                  </div>
                )}

                <div className="relative flex flex-col items-center text-center">
                  {/* Step Number */}
                  <div className="relative mb-6">
                    {/* Glow Effect */}
                    <div className="absolute inset-0 rounded-full bg-primary/30 blur-xl opacity-0 transition-opacity duration-500 group-hover:opacity-100" />

                    {/* Circle */}
                    <div className="relative flex h-20 w-20 items-center justify-center rounded-full border-2 border-primary/30 bg-card transition-all duration-500 group-hover:border-primary group-hover:scale-110">
                      {/* Inner Circle */}
                      <div className="flex h-14 w-14 items-center justify-center rounded-full bg-linear-to-br from-primary/20 to-primary/10 text-primary transition-all duration-500 group-hover:from-primary group-hover:to-primary/80 group-hover:text-primary-foreground">
                        {step.icon}
                      </div>

                      {/* Step Number Badge */}
                      <div className="absolute -right-2 -top-2 flex h-8 w-8 items-center justify-center rounded-full bg-primary text-xs font-bold text-primary-foreground shadow-lg">
                        {step.number}
                      </div>
                    </div>
                  </div>

                  {/* Content */}
                  <h3 className="mb-3 text-xl font-bold text-foreground">
                    {step.title}
                  </h3>
                  <p className="text-muted-foreground max-w-[250px]">
                    {step.description}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
};

export default HowItWorksSection;
