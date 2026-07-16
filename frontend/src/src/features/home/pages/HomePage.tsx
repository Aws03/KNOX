import HeroSection from "@/features/home/components/HeroSection";
import { Feature } from "@/features/home/components/Feature";
import HowItWorksSection from "@/features/home/components/HowItWorksSection";
import CTASection from "@/features/home/components/CTASection";

export default function HomePage() {
  return (
    <div className="relative">
      <HeroSection />
      <Feature />
      <HowItWorksSection />
      <CTASection />
    </div>
  );
}
