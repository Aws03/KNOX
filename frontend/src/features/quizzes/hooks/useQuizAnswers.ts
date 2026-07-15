import { useState } from "react";
import type { UserAnswer } from "../types";

interface UseQuizAnswersReturn {
  userAnswers: Map<number, UserAnswer>;
  handleSingleChoice: (questionId: number, choiceId: number) => void;
  handleMultipleChoice: (
    questionId: number,
    choiceId: number,
    checked: boolean
  ) => void;
  handleShortAnswer: (questionId: number, answer: string) => void;
  isAnswered: (questionId: number) => boolean;
  getAnsweredCount: () => number;
  resetAnswers: () => void;
}

export const useQuizAnswers = (): UseQuizAnswersReturn => {
  const [userAnswers, setUserAnswers] = useState<Map<number, UserAnswer>>(
    new Map()
  );

  const handleSingleChoice = (questionId: number, choiceId: number) => {
    setUserAnswers((prev) => {
      const newAnswers = new Map(prev);
      newAnswers.set(questionId, {
        questionId,
        selectedChoiceIds: [choiceId],
      });
      return newAnswers;
    });
  };

  const handleMultipleChoice = (
    questionId: number,
    choiceId: number,
    checked: boolean
  ) => {
    setUserAnswers((prev) => {
      const newAnswers = new Map(prev);
      const currentAnswer = newAnswers.get(questionId);
      const currentSelections = currentAnswer?.selectedChoiceIds || [];

      const updatedSelections = checked
        ? [...currentSelections, choiceId]
        : currentSelections.filter((id) => id !== choiceId);

      newAnswers.set(questionId, {
        questionId,
        selectedChoiceIds: updatedSelections,
      });
      return newAnswers;
    });
  };

  const handleShortAnswer = (questionId: number, answer: string) => {
    setUserAnswers((prev) => {
      const newAnswers = new Map(prev);
      newAnswers.set(questionId, {
        questionId,
        shortAnswer: answer,
      });
      return newAnswers;
    });
  };

  const isAnswered = (questionId: number): boolean => {
    const answer = userAnswers.get(questionId);
    if (!answer) return false;

    if (answer.shortAnswer !== undefined) {
      return answer.shortAnswer.trim() !== "";
    }

    return (
      answer.selectedChoiceIds !== undefined &&
      answer.selectedChoiceIds.length > 0
    );
  };

  const getAnsweredCount = (): number => {
    return userAnswers.size;
  };

  const resetAnswers = () => {
    setUserAnswers(new Map());
  };

  return {
    userAnswers,
    handleSingleChoice,
    handleMultipleChoice,
    handleShortAnswer,
    isAnswered,
    getAnsweredCount,
    resetAnswers,
  };
};
