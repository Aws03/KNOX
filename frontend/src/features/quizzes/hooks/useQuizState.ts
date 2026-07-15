import { useState, useEffect } from "react";
import type { QuizDetails } from "../types";
import { fetchQuizById } from "../api";

interface UseQuizStateReturn {
  quiz: QuizDetails | null;
  loading: boolean;
  error: string | null;
  isLoggedIn: boolean;
  loadQuizDetails: () => Promise<void>;
  updateQuizLikes: (likes: number) => void;
  updateQuizDislikes: (dislikes: number) => void;
}

export const useQuizState = (
  quizId: string | undefined,
  isUserLoggedIn: () => boolean
): UseQuizStateReturn => {
  const [quiz, setQuiz] = useState<QuizDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isLoggedIn] = useState(() => isUserLoggedIn());

  const loadQuizDetails = async () => {
    if (!quizId) return;

    setLoading(true);
    setError(null);

    try {
      const data = await fetchQuizById(quizId);
      setQuiz(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load quiz");
    } finally {
      setLoading(false);
    }
  };

  const updateQuizLikes = (likes: number) => {
    if (quiz) {
      setQuiz({ ...quiz, likes });
    }
  };

  const updateQuizDislikes = (dislikes: number) => {
    if (quiz) {
      setQuiz({ ...quiz, dislikes });
    }
  };

  useEffect(() => {
    loadQuizDetails();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [quizId]);

  return {
    quiz,
    loading,
    error,
    isLoggedIn,
    loadQuizDetails,
    updateQuizLikes,
    updateQuizDislikes,
  };
};
