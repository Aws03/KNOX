// Question types matching the backend
export const QuestionType = {
  SingleChoice: 1,
  MultipleChoice: 2,
  TrueFalse: 3,
  ShortAnswer: 4,
} as const;

export type QuestionTypeValue =
  (typeof QuestionType)[keyof typeof QuestionType];

export type QuestionTypeString =
  | "SingleChoice"
  | "MultipleChoice"
  | "TrueFalse"
  | "ShortAnswer";

// Add Quiz Page Types
export type Choice = {
  id: string;
  text: string;
  imageUrl: string | null;
  isCorrect: boolean;
  uploadingImage: boolean;
};

export type Question = {
  id: string;
  text: string;
  imageUrl: string | null;
  type: QuestionTypeValue;
  choices: Choice[];
  uploadingImage: boolean;
};

// Quiz Question Page Types
export type QuestionChoice = {
  id: number;
  questionId: number;
  text: string;
  isCorrect: boolean;
  imageUrl: string | null;
};

export type QuizQuestion = {
  id: number;
  quizId: number;
  type: QuestionTypeString;
  text: string;
  imageUrl: string | null;
  choices: QuestionChoice[];
};

export type QuizDetails = {
  id: number;
  courseId: number;
  writerId: number;
  writerName: string;
  title: string;
  description: string;
  likes: number;
  dislikes: number;
  createdAt: string;
  questions: QuizQuestion[];
};

export type UserAnswer = {
  questionId: number;
  selectedChoiceIds?: number[];
  shortAnswer?: string;
};

// Quizzes List Page Types
export type QuizListItem = {
  id: number;
  title: string;
  likes: number;
  writerName: string;
  createdAt: string;
};

// Create Quiz Payload Types
export type CreateQuizChoicePayload = {
  text: string;
  imageUrl: string | null;
  isCorrect: boolean;
};

export type CreateQuizQuestionPayload = {
  text: string;
  quizId: number;
  imageUrl: string | null;
  type: QuestionTypeValue;
  choices: CreateQuizChoicePayload[];
};

export type CreateQuizPayload = {
  title: string;
  writerId: number;
  courseId: number;
  description: string | null;
  questions: CreateQuizQuestionPayload[];
};

// Helper function to get question type label
export const getQuestionTypeLabel = (type: QuestionTypeValue): string => {
  switch (type) {
    case QuestionType.SingleChoice:
      return "Single Choice";
    case QuestionType.MultipleChoice:
      return "Multiple Choice";
    case QuestionType.TrueFalse:
      return "True/False";
    case QuestionType.ShortAnswer:
      return "Short Answer";
    default:
      return "Unknown";
  }
};

// Utility functions for date formatting
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

export const getTimeAgo = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInMs = now.getTime() - date.getTime();
  const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

  if (diffInDays === 0) return "Today";
  if (diffInDays === 1) return "Yesterday";
  if (diffInDays < 7) return `${diffInDays} days ago`;
  if (diffInDays < 30) return `${Math.floor(diffInDays / 7)} weeks ago`;
  if (diffInDays < 365) return `${Math.floor(diffInDays / 30)} months ago`;
  return `${Math.floor(diffInDays / 365)} years ago`;
};
