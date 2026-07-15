import { Button } from "@/shared/ui/button";
import { ArrowRight, Sparkles, CheckCircle } from "lucide-react";
import { useNavigate } from "react-router-dom";

const benefits = [
  "Comprehensive study materials library",
  "Smart quiz bank with instant feedback",
  "AI-powered academic guidance",
  "Connect with student community",
];

const CTASection = () => {
  const navigate = useNavigate();

  return (
    <section className="relative py-24 overflow-hidden">
      {/* Background */}
      <div className="absolute inset-0 -z-10 bg-linear-to-br from-primary/5 via-background to-primary/5" />

      {/* Animated Orbs */}
      <div className="absolute inset-0 -z-10 overflow-hidden">
        <div className="absolute -left-20 top-1/2 h-72 w-72 -translate-y-1/2 animate-pulse rounded-full bg-primary/20 blur-[100px]" />
        <div className="absolute -right-20 top-1/2 h-72 w-72 -translate-y-1/2 animate-pulse rounded-full bg-primary/20 blur-[100px] delay-1000" />
      </div>

      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="relative mx-auto max-w-4xl">
          {/* Card */}
          <div className="relative overflow-hidden rounded-3xl border border-primary/20 bg-linear-to-br from-card via-card to-primary/5 p-8 shadow-2xl shadow-primary/10 sm:p-12 lg:p-16">
            {/* Background Pattern */}
            <div className="absolute inset-0 bg-[linear-gradient(to_right,hsl(var(--primary)/0.03)_1px,transparent_1px),linear-gradient(to_bottom,hsl(var(--primary)/0.03)_1px,transparent_1px)] bg-size-[2rem_2rem]" />

            {/* Glowing Orb */}
            <div className="absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary/30 blur-[80px]" />

            <div className="relative">
              {/* Badge */}
              <div className="mb-6 inline-flex items-center gap-2 rounded-full border border-primary/30 bg-primary/10 px-4 py-2">
                <Sparkles className="h-4 w-4 text-primary" />
                <span className="text-sm font-medium text-primary">
                  Start for free today
                </span>
              </div>

              {/* Heading */}
              <h2 className="mb-6 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
                Ready to Transform Your
                <span className="block mt-2 bg-linear-to-r from-primary to-primary/60 bg-clip-text text-transparent">
                  Academic Journey?
                </span>
              </h2>

              {/* Description */}
              <p className="mb-8 max-w-2xl text-lg text-muted-foreground">
                Join thousands of students who are already achieving their
                academic goals with Uni-Hub. Get started in seconds — no credit
                card required.
              </p>

              {/* Benefits List */}
              <ul className="mb-10 grid gap-3 sm:grid-cols-2">
                {benefits.map((benefit, index) => (
                  <li
                    key={index}
                    className="flex items-center gap-3 text-muted-foreground"
                  >
                    <CheckCircle className="h-5 w-5 text-primary shrink-0" />
                    <span>{benefit}</span>
                  </li>
                ))}
              </ul>

              {/* CTA Buttons */}
              <div className="flex flex-col sm:flex-row gap-4">
                <Button
                  size="lg"
                  className="group relative overflow-hidden bg-primary px-8 py-6 text-lg font-semibold shadow-lg shadow-primary/25 transition-all hover:shadow-xl hover:shadow-primary/30 hover:scale-105"
                  onClick={() => navigate("/register")}
                >
                  <span className="relative z-10 flex items-center gap-2">
                    Get Started Free
                    <ArrowRight className="h-5 w-5 transition-transform group-hover:translate-x-1" />
                  </span>
                </Button>
                <Button
                  size="lg"
                  variant="outline"
                  className="px-8 py-6 text-lg font-semibold backdrop-blur-sm transition-all hover:bg-primary/5 hover:scale-105"
                  onClick={() => navigate("/materials")}
                >
                  Browse Materials
                </Button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default CTASection;
