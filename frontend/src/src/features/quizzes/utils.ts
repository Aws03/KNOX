import { QuestionType, type Question } from "./types";

/**
 * Validate image file type
 * @param file - The file to validate
 * @returns true if valid, false otherwise
 */
export const isValidImageFile = (file: File): boolean => {
  return file.type.startsWith("image/");
};

/**
 * Validate image file size (max 5MB)
 * @param file - The file to validate
 * @returns true if valid, false otherwise
 */
export const isValidImageSize = (file: File): boolean => {
  const MAX_SIZE = 5 * 1024 * 1024; // 5MB
  return file.size <= MAX_SIZE;
};

/**
 * Validate quiz data before submission
 * @param title - Quiz title
 * @param questions - Array of questions
 * @returns Error message or null if valid
 */
export const validateQuiz = (
  title: string,
  questions: Question[]
): string | null => {
  if (!title.trim()) {
    return "Please enter a quiz title";
  }

  if (questions.length === 0) {
    return "Please add at least one question";
  }

  for (let i = 0; i < questions.length; i++) {
    const question = questions[i];

    if (!question.text.trim()) {
      return `Question ${i + 1}: Please enter question text`;
    }

    if (question.type !== QuestionType.ShortAnswer) {
      if (question.choices.length < 2) {
        return `Question ${i + 1}: Please add at least 2 choices`;
      }

      const hasCorrectAnswer = question.choices.some((c) => c.isCorrect);
      if (!hasCorrectAnswer) {
        return `Question ${i + 1}: Please mark at least one correct answer`;
      }

      for (let j = 0; j < question.choices.length; j++) {
        const choice = question.choices[j];
        if (!choice.text.trim() && !choice.imageUrl) {
          return `Question ${i + 1}, Choice ${
            j + 1
          }: Please enter choice text or add an image`;
        }
      }

      // For Single Choice and True/False, only one answer should be correct
      if (
        question.type === QuestionType.SingleChoice ||
        question.type === QuestionType.TrueFalse
      ) {
        const correctCount = question.choices.filter((c) => c.isCorrect).length;
        if (correctCount > 1) {
          return `Question ${i + 1}: Only one answer can be correct for ${
            question.type === QuestionType.SingleChoice
              ? "Single Choice"
              : "True/False"
          } questions`;
        }
      }
    } else {
      // Short Answer
      if (question.choices.length === 0) {
        return `Question ${i + 1}: Please add the correct answer`;
      }
      if (!question.choices[0].text.trim()) {
        return `Question ${i + 1}: Please enter the correct answer`;
      }
    }
  }

  return null;
};

/**
 * Calculate quiz score based on user answers
 * @param questions - Array of quiz questions
 * @param userAnswers - Map of user answers
 * @returns Object with score and percentage
 */
export const calculateQuizScore = <
  T extends {
    id: number;
    type: string;
    choices: Array<{ id: number; isCorrect: boolean; text: string }>;
  }
>(
  questions: T[],
  userAnswers: Map<
    number,
    { questionId: number; selectedChoiceIds?: number[]; shortAnswer?: string }
  >
): { score: number; percentage: number } => {
  let correctAnswers = 0;

  questions.forEach((question) => {
    const userAnswer = userAnswers.get(question.id);

    if (!userAnswer) return;

    if (question.type === "ShortAnswer") {
      const correctAnswer = question.choices.find((c) => c.isCorrect);
      if (
        correctAnswer &&
        userAnswer.shortAnswer &&
        userAnswer.shortAnswer.trim().toLowerCase() ===
          correctAnswer.text.trim().toLowerCase()
      ) {
        correctAnswers++;
      }
    } else {
      const correctChoiceIds = question.choices
        .filter((choice) => choice.isCorrect)
        .map((choice) => choice.id);

      const selectedIds = userAnswer.selectedChoiceIds || [];

      const isCorrect =
        selectedIds.length === correctChoiceIds.length &&
        selectedIds.every((id) => correctChoiceIds.includes(id)) &&
        correctChoiceIds.every((id) => selectedIds.includes(id));

      if (isCorrect) {
        correctAnswers++;
      }
    }
  });

  const totalQuestions = questions.length;
  const percentage = (correctAnswers / totalQuestions) * 100;

  return {
    score: correctAnswers,
    percentage: Math.round(percentage),
  };
};

/**
 * Check if a specific question was answered correctly
 * @param question - The question to check
 * @param userAnswer - The user's answer
 * @returns true if correct, false otherwise
 */
export const isQuestionCorrect = <
  T extends {
    id: number;
    type: string;
    choices: Array<{ id: number; isCorrect: boolean; text: string }>;
  }
>(
  question: T,
  userAnswer?: {
    questionId: number;
    selectedChoiceIds?: number[];
    shortAnswer?: string;
  }
): boolean => {
  if (!userAnswer) return false;

  if (question.type === "ShortAnswer") {
    const correctAnswer = question.choices.find((c) => c.isCorrect);
    return (
      correctAnswer !== undefined &&
      userAnswer.shortAnswer !== undefined &&
      userAnswer.shortAnswer.trim().toLowerCase() ===
        correctAnswer.text.trim().toLowerCase()
    );
  }

  const correctChoiceIds = question.choices
    .filter((choice) => choice.isCorrect)
    .map((choice) => choice.id);

  const selectedIds = userAnswer.selectedChoiceIds || [];

  return (
    selectedIds.length === correctChoiceIds.length &&
    selectedIds.every((id) => correctChoiceIds.includes(id)) &&
    correctChoiceIds.every((id) => selectedIds.includes(id))
  );
};
