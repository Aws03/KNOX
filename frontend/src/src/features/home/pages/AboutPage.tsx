import { useState } from "react";
import {
  BookOpen,
  Users,
  Award,
  Globe,
  Zap,
  Shield,
  Check,
  ArrowRight,
  TrendingUp,
  Clock,
  FileText,
  MessageSquare,
} from "lucide-react";
import { useAuth } from "@/app/providers/useAuth";
import { useNavigate } from "react-router-dom";

const AboutPage = () => {
  const [phoneNumber, setPhoneNumber] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleContributeSubmit = async () => {
    if (!isAuthenticated) {
      alert("Please log in to submit your application");
      return;
    }

    if (!phoneNumber || phoneNumber.trim().length < 10) {
      alert("Please enter a valid phone number");
      return;
    }

    setIsSubmitting(true);
    await new Promise((resolve) => setTimeout(resolve, 1000));
    alert("Application submitted successfully!");
    setPhoneNumber("");
    setIsSubmitting(false);
  };

  const features = [
    {
      icon: BookOpen,
      title: "Centralized Learning",
      description:
        "Access all your courses, materials, and resources in one unified platform",
    },
    {
      icon: Users,
      title: "Collaborative Community",
      description:
        "Connect with students, educators, and content creators across universities",
    },
    {
      icon: Award,
      title: "Interactive Assessments",
      description:
        "Test your knowledge with quizzes and track your progress in real-time",
    },
    {
      icon: Globe,
      title: "Accessible Anywhere",
      description:
        "Study on any device with our mobile-responsive platform design",
    },
    {
      icon: Zap,
      title: "Efficient Content Management",
      description:
        "Organized folders and categories make finding resources effortless",
    },
    {
      icon: Shield,
      title: "Secure & Reliable",
      description:
        "Your data and academic progress are protected with industry-standard security",
    },
  ];

  const howItWorks = [
    {
      step: "1",
      title: "Create Your Account",
      description: "Sign up in seconds and get instant access to our platform",
    },
    {
      step: "2",
      title: "Browse & Enroll",
      description: "Explore courses and materials relevant to your studies",
    },
    {
      step: "3",
      title: "Learn & Practice",
      description: "Access materials, take quizzes, and track your progress",
    },
    {
      step: "4",
      title: "Succeed & Grow",
      description: "Achieve your academic goals with organized learning",
    },
  ];

  return (
    <div className="min-h-screen bg-background">
      {/* Hero Section */}
      <div className="bg-primary py-20">
        <div className="container mx-auto px-4 text-center">
          <h1 className="mb-6 text-5xl font-bold tracking-tight md:text-6xl text-primary-foreground">
            Welcome to Uni-Hub
          </h1>
          <p className="mx-auto max-w-3xl text-xl md:text-2xl text-primary-foreground/90 mb-8">
            Your all-in-one university learning management platform. Centralize
            your studies, collaborate with peers, and excel academically.
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            <button className="bg-background text-foreground px-8 py-3 rounded-lg font-semibold hover:bg-background/90 transition flex items-center gap-2">
              Get Started <ArrowRight className="w-5 h-5" />
            </button>
            <button className="border-2 border-primary-foreground text-primary-foreground px-8 py-3 rounded-lg font-semibold hover:bg-primary-foreground/10 transition">
              Learn More
            </button>
          </div>
        </div>
      </div>

      {/* Value Proposition Cards */}
      <div className="container mx-auto px-4 -mt-12">
        <div className="grid md:grid-cols-3 gap-6">
          <div className="bg-card rounded-xl shadow-lg p-6 border">
            <BookOpen className="w-10 h-10 text-primary mb-3" />
            <h3 className="font-bold text-lg mb-2">All in One Place</h3>
            <p className="text-muted-foreground text-sm">
              No more switching between multiple platforms. Everything you need
              for your studies is centralized here.
            </p>
          </div>
          <div className="bg-card rounded-xl shadow-lg p-6 border">
            <Zap className="w-10 h-10 text-primary mb-3" />
            <h3 className="font-bold text-lg mb-2">Save Time</h3>
            <p className="text-muted-foreground text-sm">
              Quick access to organized materials means less time searching and
              more time learning.
            </p>
          </div>
          <div className="bg-card rounded-xl shadow-lg p-6 border">
            <Award className="w-10 h-10 text-primary mb-3" />
            <h3 className="font-bold text-lg mb-2">Study Smarter</h3>
            <p className="text-muted-foreground text-sm">
              Interactive quizzes and organized resources help you prepare
              effectively for exams.
            </p>
          </div>
        </div>
      </div>

      {/* Mission Section */}
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-4xl mx-auto text-center mb-16">
          <h2 className="text-4xl font-bold mb-6">Our Mission</h2>
          <p className="text-xl text-muted-foreground leading-relaxed">
            We're on a mission to eliminate the chaos of scattered educational
            resources. Uni-Hub provides students with a{" "}
            <span className="text-primary font-semibold">
              single, reliable destination
            </span>{" "}
            for all their academic needs while empowering educators with
            powerful content management tools.
          </p>
        </div>

        {/* Features Grid */}
        <div className="mb-16">
          <h2 className="text-3xl font-bold text-center mb-12">
            Why Choose Uni-Hub?
          </h2>
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {features.map((feature, index) => (
              <div
                key={index}
                className="bg-card border p-6 rounded-xl shadow-sm hover:shadow-lg transition-all hover:-translate-y-1"
              >
                <feature.icon className="w-12 h-12 text-primary mb-4" />
                <h3 className="text-xl font-bold mb-3">{feature.title}</h3>
                <p className="text-muted-foreground">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* How It Works */}
      <div className="bg-muted/50 py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold text-center mb-12">How It Works</h2>
          <div className="grid md:grid-cols-4 gap-8 max-w-5xl mx-auto">
            {howItWorks.map((item, index) => (
              <div key={index} className="text-center relative">
                <div className="w-16 h-16 bg-primary text-primary-foreground rounded-full flex items-center justify-center text-2xl font-bold mx-auto mb-4">
                  {item.step}
                </div>
                <h3 className="font-bold text-lg mb-2">{item.title}</h3>
                <p className="text-muted-foreground text-sm">
                  {item.description}
                </p>
                {index < howItWorks.length - 1 && (
                  <ArrowRight className="hidden md:block absolute top-8 -right-4 text-muted-foreground/30 w-8 h-8" />
                )}
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* What You Can Do Section */}
      <div className="container mx-auto px-4 py-16">
        <h2 className="text-3xl font-bold text-center mb-12">
          What You Can Do on Uni-Hub
        </h2>
        <div className="grid md:grid-cols-2 gap-8 max-w-5xl mx-auto">
          <div className="bg-card border p-8 rounded-xl shadow-sm hover:shadow-lg transition">
            <Users className="w-12 h-12 text-primary mb-4" />
            <h3 className="text-2xl font-bold mb-6">Access & Learn</h3>
            <ul className="space-y-3">
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Browse and access course materials organized by subject
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Download PDFs, notes, and study guides for offline use
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Take quizzes to test your knowledge and track your scores
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Search and filter resources to find exactly what you need
                </span>
              </li>
            </ul>
          </div>

          <div className="bg-card border p-8 rounded-xl shadow-sm hover:shadow-lg transition">
            <FileText className="w-12 h-12 text-primary mb-4" />
            <h3 className="text-2xl font-bold mb-6">Contribute & Share</h3>
            <ul className="space-y-3">
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Upload your own study materials to help fellow students
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Create and publish quizzes for practice and revision
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Organize content in folders for easy navigation
                </span>
              </li>
              <li className="flex items-start gap-3">
                <Check className="w-5 h-5 text-primary shrink-0 mt-0.5" />
                <span className="text-muted-foreground">
                  Build a library of resources for your courses
                </span>
              </li>
            </ul>
          </div>
        </div>
      </div>

      {/* Key Features Highlight */}
      <div className="bg-muted/30 py-16">
        <div className="container mx-auto px-4">
          <div className="max-w-4xl mx-auto">
            <h2 className="text-3xl font-bold text-center mb-12">
              Everything You Need to Succeed
            </h2>
            <div className="grid md:grid-cols-2 gap-6">
              <div className="bg-card border p-6 rounded-lg shadow-sm">
                <Clock className="w-10 h-10 text-primary mb-3" />
                <h3 className="font-bold text-lg mb-2">Self-Paced Learning</h3>
                <p className="text-muted-foreground">
                  Study at your own pace, anytime and anywhere. Access materials
                  24/7 and learn on your schedule.
                </p>
              </div>
              <div className="bg-card border p-6 rounded-lg shadow-sm">
                <TrendingUp className="w-10 h-10 text-primary mb-3" />
                <h3 className="font-bold text-lg mb-2">Progress Tracking</h3>
                <p className="text-muted-foreground">
                  Monitor your performance with detailed analytics and insights
                  on quiz scores and course completion.
                </p>
              </div>
              <div className="bg-card border p-6 rounded-lg shadow-sm">
                <FileText className="w-10 h-10 text-primary mb-3" />
                <h3 className="font-bold text-lg mb-2">Rich Content Library</h3>
                <p className="text-muted-foreground">
                  Access thousands of study materials, lecture notes, PDFs, and
                  resources organized by course and topic.
                </p>
              </div>
              <div className="bg-card border p-6 rounded-lg shadow-sm">
                <MessageSquare className="w-10 h-10 text-primary mb-3" />
                <h3 className="font-bold text-lg mb-2">Active Community</h3>
                <p className="text-muted-foreground">
                  Connect with fellow students and educators. Share knowledge
                  and collaborate on learning.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Contribute Section */}
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-3xl mx-auto">
          <div className="bg-primary p-10 rounded-2xl shadow-2xl">
            <div className="text-center mb-8">
              <h2 className="text-3xl font-bold mb-4 text-primary-foreground">
                Become a Content Creator
              </h2>
              <p className="text-lg text-primary-foreground/90">
                Share your expertise and help thousands of students succeed.
                Join our community of educators and content creators today!
              </p>
            </div>

            <div className="space-y-6">
              <div>
                <label
                  className="block text-sm font-medium mb-2 text-primary-foreground"
                  htmlFor="phone"
                >
                  Phone Number
                </label>
                <input
                  id="phone"
                  type="tel"
                  placeholder="Enter your phone number"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  disabled={isSubmitting}
                  className="w-full px-4 py-3 rounded-lg text-foreground bg-background focus:ring-2 focus:ring-primary/50 outline-none"
                />
                <p className="text-sm text-primary-foreground/75 mt-2">
                  We'll contact you within 24-48 hours to discuss your
                  application and provide creator access.
                </p>
              </div>

              <button
                onClick={handleContributeSubmit}
                disabled={isSubmitting}
                className="w-full bg-background text-foreground py-3 rounded-lg font-semibold hover:bg-background/90 transition disabled:opacity-50"
              >
                {isSubmitting ? "Submitting..." : "Apply to Become a Creator"}
              </button>

              {!isAuthenticated && (
                <p className="text-center text-sm text-primary-foreground/75">
                  Note: You need to be logged in to submit your application
                </p>
              )}
            </div>

            <div className="mt-8 pt-8 border-t border-primary-foreground/20">
              <h3 className="font-bold text-lg mb-4 text-primary-foreground">
                As a Creator, You'll Get:
              </h3>
              <div className="grid md:grid-cols-2 gap-4 text-primary-foreground">
                <div className="flex items-start gap-3">
                  <Check className="w-5 h-5 shrink-0 mt-0.5" />
                  <span>Full content management dashboard</span>
                </div>
                <div className="flex items-start gap-3">
                  <Check className="w-5 h-5 shrink-0 mt-0.5" />
                  <span>Analytics on content performance</span>
                </div>
                <div className="flex items-start gap-3">
                  <Check className="w-5 h-5 shrink-0 mt-0.5" />
                  <span>Recognition in creator community</span>
                </div>
                <div className="flex items-start gap-3">
                  <Check className="w-5 h-5 shrink-0 mt-0.5" />
                  <span>Priority support and training</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* CTA Section */}
      <div className="bg-card border-t py-16">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-4">
            Ready to Transform Your Learning?
          </h2>
          <p className="text-xl text-muted-foreground mb-8 max-w-2xl mx-auto">
            Join thousands of students already benefiting from organized,
            accessible, and comprehensive learning resources.
          </p>
          <div className="flex flex-wrap justify-center gap-4">
            {!isAuthenticated && (
              <>
                <button
                  onClick={() => navigate("/auth/register")}
                  className="bg-primary text-primary-foreground px-8 py-3 rounded-lg font-semibold hover:opacity-90 transition"
                >
                  Create Free Account
                </button>
                <button
                  onClick={() => navigate("/auth/login")}
                  className="border-2 border-primary text-primary px-8 py-3 rounded-lg font-semibold hover:bg-primary/10 transition"
                >
                  Sign In
                </button>
              </>
            )}
            <button
              onClick={() => navigate("/courses")}
              className="bg-secondary text-secondary-foreground px-8 py-3 rounded-lg font-semibold hover:opacity-90 transition"
            >
              Browse Courses
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AboutPage;
