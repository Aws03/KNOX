import { Star, Quote } from "lucide-react";
import { Card, CardContent } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";

interface Testimonial {
  name: string;
  role: string;
  university: string;
  content: string;
  rating: number;
  avatar: string;
}

const testimonials: Testimonial[] = [
  {
    name: "Sarah Chen",
    role: "Medical Student",
    university: "Stanford University",
    content:
      "Uni-Hub completely transformed how I study. The AI supervisor helped me understand complex anatomy concepts that I was struggling with for months. My grades improved significantly!",
    rating: 5,
    avatar: "SC",
  },
  {
    name: "Michael Roberts",
    role: "Engineering Student",
    university: "MIT",
    content:
      "The quiz bank is incredible! I practiced with thousands of questions and felt fully prepared for my exams. The instant feedback feature is a game-changer.",
    rating: 5,
    avatar: "MR",
  },
  {
    name: "Emma Thompson",
    role: "Law Student",
    university: "Harvard Law School",
    content:
      "Having all my study materials organized in one place saved me countless hours. The platform is intuitive and the content quality is exceptional.",
    rating: 5,
    avatar: "ET",
  },
  {
    name: "David Park",
    role: "Computer Science Student",
    university: "UC Berkeley",
    content:
      "The community feature helped me connect with other CS students. We share resources and help each other out. It's like having a study group available 24/7.",
    rating: 5,
    avatar: "DP",
  },
  {
    name: "Lisa Anderson",
    role: "Psychology Major",
    university: "Oxford University",
    content:
      "I was skeptical at first, but the AI Academic Supervisor genuinely understands my questions and provides detailed, accurate explanations. Worth every penny!",
    rating: 5,
    avatar: "LA",
  },
  {
    name: "James Wilson",
    role: "Business Student",
    university: "Wharton School",
    content:
      "Uni-Hub helped me prepare for my MBA exams efficiently. The structured learning paths and progress tracking kept me motivated throughout my journey.",
    rating: 5,
    avatar: "JW",
  },
];

const TestimonialCard = ({ testimonial }: { testimonial: Testimonial }) => (
  <Card className="group relative overflow-hidden border-0 bg-card/50 backdrop-blur-xl transition-all duration-500 hover:shadow-2xl hover:shadow-primary/10 hover:-translate-y-1">
    {/* Gradient Border Effect */}
    <div className="absolute inset-0 rounded-xl bg-linear-to-br from-primary/20 via-transparent to-primary/20 opacity-0 transition-opacity duration-500 group-hover:opacity-100" />
    <div className="absolute inset-px rounded-xl bg-card" />

    <CardContent className="relative p-6">
      {/* Quote Icon */}
      <div className="absolute -top-2 -right-2 opacity-10">
        <Quote className="h-20 w-20 text-primary" />
      </div>

      {/* Rating */}
      <div className="mb-4 flex gap-1">
        {Array.from({ length: testimonial.rating }).map((_, i) => (
          <Star key={i} className="h-4 w-4 fill-yellow-500 text-yellow-500" />
        ))}
      </div>

      {/* Content */}
      <p className="mb-6 text-muted-foreground leading-relaxed">
        "{testimonial.content}"
      </p>

      {/* Author */}
      <div className="flex items-center gap-4">
        <div className="flex h-12 w-12 items-center justify-center rounded-full bg-linear-to-br from-primary to-primary/60 text-primary-foreground font-semibold">
          {testimonial.avatar}
        </div>
        <div>
          <p className="font-semibold text-foreground">{testimonial.name}</p>
          <p className="text-sm text-muted-foreground">
            {testimonial.role} • {testimonial.university}
          </p>
        </div>
      </div>
    </CardContent>
  </Card>
);

const TestimonialsSection = () => {
  return (
    <section className="relative py-24 overflow-hidden">
      {/* Background */}
      <div className="absolute inset-0 -z-10">
        <div className="absolute right-0 top-0 h-96 w-96 rounded-full bg-primary/10 blur-[120px]" />
        <div className="absolute left-0 bottom-0 h-96 w-96 rounded-full bg-primary/10 blur-[120px]" />
      </div>

      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section Header */}
        <div className="mb-16 text-center">
          <Badge
            variant="secondary"
            className="mb-4 px-4 py-1.5 text-sm font-medium"
          >
            <Star className="mr-2 h-4 w-4 fill-yellow-500 text-yellow-500" />
            Student Stories
          </Badge>
          <h2 className="mb-6 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
            Loved by Students
            <span className="block mt-2 bg-linear-to-r from-primary via-primary/80 to-primary bg-clip-text text-transparent">
              Worldwide
            </span>
          </h2>
          <p className="mx-auto max-w-2xl text-lg text-muted-foreground">
            Join thousands of students who have transformed their academic
            journey with Uni-Hub.
          </p>
        </div>

        {/* Testimonials Grid */}
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {testimonials.map((testimonial, index) => (
            <TestimonialCard key={index} testimonial={testimonial} />
          ))}
        </div>
      </div>
    </section>
  );
};

export default TestimonialsSection;
