import { useState, useEffect } from "react";
import type { QuizDetails } from "../types";
import { fetchQuizById } from "../api";

interface UseQuizStateReturn {
  quiz: QuizDetails | null;
  loading: boolean;
  error: string | null;
  isLoggedIn: boolean;
  loadQuizDetails: () => Promise<void>;
  updateQuizReactionCounts: (likes: number, dislikes: number) => void;
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

  // Sets both counts from a single response so a Like<->Dislike switch (which
  // changes both counters server-side) can't lose one of the two updates -
  // calling updateQuizLikes then updateQuizDislikes separately would have
  // each closure over the same pre-update `quiz`, silently dropping the first.
  const updateQuizReactionCounts = (likes: number, dislikes: number) => {
    if (quiz) {
      setQuiz({ ...quiz, likes, dislikes });
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
    updateQuizReactionCounts,
  };
};
