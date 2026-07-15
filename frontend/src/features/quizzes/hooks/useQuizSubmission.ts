import { useState } from "react";
import { calculateQuizScore, isQuestionCorrect } from "../utils";
import { submitQuizScore, isUserLoggedIn } from "../api";
import type { QuizQuestion, UserAnswer } from "../types";

interface UseQuizSubmissionReturn {
  showResults: boolean;
  score: number;
  percentage: number;
  isSubmitting: boolean;
  showConfirmDialog: boolean;
  handleSubmitClick: () => void;
  handleConfirmSubmit: (
    questions: QuizQuestion[],
    userAnswers: Map<number, UserAnswer>,
    quizId?: string
  ) => void;
  handleCancelSubmit: () => void;
  isQuestionCorrect: (
    question: QuizQuestion,
    userAnswer?: UserAnswer
  ) => boolean;
  resetResults: () => void;
}

export const useQuizSubmission = (): UseQuizSubmissionReturn => {
  const [showResults, setShowResults] = useState(false);
  const [score, setScore] = useState(0);
  const [percentage, setPercentage] = useState(0);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);

  const handleSubmitClick = () => {
    setShowConfirmDialog(true);
  };

  const handleConfirmSubmit = async (
    questions: QuizQuestion[],
    userAnswers: Map<number, UserAnswer>,
    quizId?: string
  ) => {
    setShowConfirmDialog(false);
    setIsSubmitting(true);

    try {
      // Calculate the score
      const result = calculateQuizScore(questions, userAnswers);

      // Submit score to API if user is logged in
      if (quizId && isUserLoggedIn()) {
        await submitQuizScore(quizId, result.percentage);
      }

      setScore(result.score);
      setPercentage(result.percentage);
      setShowResults(true);
    } catch (error) {
      console.error("Error during submission:", error);
      // Still show results even if API call fails
      const result = calculateQuizScore(questions, userAnswers);
      setScore(result.score);
      setPercentage(result.percentage);
      setShowResults(true);
    } finally {
      setIsSubmitting(false);
      window.scrollTo({ top: 0, behavior: "smooth" });
    }
  };

  const handleCancelSubmit = () => {
    setShowConfirmDialog(false);
  };

  const resetResults = () => {
    setShowResults(false);
    setScore(0);
    setPercentage(0);
  };

  return {
    showResults,
    score,
    percentage,
    isSubmitting,
    showConfirmDialog,
    handleSubmitClick,
    handleConfirmSubmit,
    handleCancelSubmit,
    isQuestionCorrect,
    resetResults,
  };
};
