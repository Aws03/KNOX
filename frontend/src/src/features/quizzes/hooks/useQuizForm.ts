import { useState } from "react";
import type { Question, Choice, QuestionTypeValue } from "../types";
import { QuestionType } from "../types";

export const useQuizForm = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [questions, setQuestions] = useState<Question[]>([]);
  const [error, setError] = useState<string | null>(null);

  const addQuestion = () => {
    if (questions.length >= 100) {
      setError("Maximum 100 questions per quiz");
      return;
    }

    const newQuestion: Question = {
      id: Date.now().toString(),
      text: "",
      imageUrl: null,
      type: QuestionType.SingleChoice,
      choices: [
        {
          id: Date.now().toString() + "-1",
          text: "",
          imageUrl: null,
          isCorrect: false,
          uploadingImage: false,
        },
        {
          id: Date.now().toString() + "-2",
          text: "",
          imageUrl: null,
          isCorrect: false,
          uploadingImage: false,
        },
      ],
      uploadingImage: false,
    };

    setQuestions([...questions, newQuestion]);
    setError(null);
  };

  const removeQuestion = (questionId: string) => {
    setQuestions(questions.filter((q) => q.id !== questionId));
  };

  const updateQuestion = <K extends keyof Question>(
    questionId: string,
    field: K,
    value: Question[K]
  ) => {
    setQuestions(
      questions.map((q) => (q.id === questionId ? { ...q, [field]: value } : q))
    );
  };

  const addChoice = (questionId: string) => {
    setQuestions(
      questions.map((q) => {
        if (q.id === questionId) {
          if (q.choices.length >= 10) {
            setError("Maximum 10 choices per question");
            return q;
          }
          const newChoice: Choice = {
            id: Date.now().toString(),
            text: "",
            imageUrl: null,
            isCorrect: false,
            uploadingImage: false,
          };
          return { ...q, choices: [...q.choices, newChoice] };
        }
        return q;
      })
    );
  };

  const removeChoice = (questionId: string, choiceId: string) => {
    setQuestions(
      questions.map((q) => {
        if (q.id === questionId) {
          return {
            ...q,
            choices: q.choices.filter((c) => c.id !== choiceId),
          };
        }
        return q;
      })
    );
  };

  const updateChoice = <K extends keyof Choice>(
    questionId: string,
    choiceId: string,
    field: K,
    value: Choice[K]
  ) => {
    setQuestions(
      questions.map((q) => {
        if (q.id === questionId) {
          return {
            ...q,
            choices: q.choices.map((c) =>
              c.id === choiceId ? { ...c, [field]: value } : c
            ),
          };
        }
        return q;
      })
    );
  };

  const handleQuestionTypeChange = (
    questionId: string,
    newType: QuestionTypeValue
  ) => {
    updateQuestion(questionId, "type", newType);

    // Adjust choices based on type
    if (newType === QuestionType.TrueFalse) {
      updateQuestion(questionId, "choices", [
        {
          id: Date.now().toString() + "-true",
          text: "True",
          imageUrl: null,
          isCorrect: false,
          uploadingImage: false,
        },
        {
          id: Date.now().toString() + "-false",
          text: "False",
          imageUrl: null,
          isCorrect: false,
          uploadingImage: false,
        },
      ]);
    } else if (newType === QuestionType.ShortAnswer) {
      updateQuestion(questionId, "choices", [
        {
          id: Date.now().toString(),
          text: "",
          imageUrl: null,
          isCorrect: true,
          uploadingImage: false,
        },
      ]);
    }
  };

  return {
    title,
    setTitle,
    description,
    setDescription,
    questions,
    setQuestions,
    error,
    setError,
    addQuestion,
    removeQuestion,
    updateQuestion,
    addChoice,
    removeChoice,
    updateChoice,
    handleQuestionTypeChange,
  };
};
