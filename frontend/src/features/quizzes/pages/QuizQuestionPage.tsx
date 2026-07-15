import { useState, useCallback } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { likeQuiz, dislikeQuiz, isUserLoggedIn } from "../api";
import { QuizResultsCard } from "../components/QuizResultsCard";
import { QuizReviewSection } from "../components/QuizReviewSection";
import { QuizHeader } from "../components/QuizHeader";
import { QuizQuestionCard } from "../components/QuizQuestionCard";
import { QuizSubmitBar } from "../components/QuizSubmitBar";
import { QuizLoadingScreen } from "../components/QuizLoadingScreen";
import { QuizErrorScreen } from "../components/QuizErrorScreen";
import { useQuizState } from "../hooks/useQuizState";
import { useQuizAnswers } from "../hooks/useQuizAnswers";
import { useQuizSubmission } from "../hooks/useQuizSubmission";

const QuizQuestionsPage = () => {
  const { quizId, courseId } = useParams<{
    quizId: string;
    courseId: string;
  }>();
  const navigate = useNavigate();
  const [highlightedQuestionId, setHighlightedQuestionId] = useState<
    number | null
  >(null);

  // Custom hooks for state management
  const {
    quiz,
    loading,
    error,
    isLoggedIn,
    updateQuizLikes,
    updateQuizDislikes,
  } = useQuizState(quizId, isUserLoggedIn);

  const {
    userAnswers,
    handleSingleChoice,
    handleMultipleChoice,
    handleShortAnswer,
    isAnswered,
    getAnsweredCount,
    resetAnswers,
  } = useQuizAnswers();

  const {
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
  } = useQuizSubmission();

  // Get unanswered question IDs
  const getUnansweredQuestionIds = useCallback((): number[] => {
    if (!quiz) return [];
    return quiz.questions.filter((q) => !isAnswered(q.id)).map((q) => q.id);
  }, [quiz, isAnswered]);

  // Scroll to a specific question
  const handleScrollToQuestion = useCallback((questionId: number) => {
    const element = document.getElementById(`question-${questionId}`);
    if (element) {
      // Highlight the question
      setHighlightedQuestionId(questionId);

      // Scroll to the question with offset for sticky header
      const headerOffset = 100;
      const elementPosition = element.getBoundingClientRect().top;
      const offsetPosition =
        elementPosition + window.pageYOffset - headerOffset;

      window.scrollTo({
        top: offsetPosition,
        behavior: "smooth",
      });

      // Remove highlight after animation
      setTimeout(() => {
        setHighlightedQuestionId(null);
      }, 2000);
    }
  }, []);

  const handleLike = async () => {
    if (!isLoggedIn || !quizId || !quiz) return;

    try {
      await likeQuiz(quizId);
      updateQuizLikes(quiz.likes + 1);
    } catch (err) {
      console.error("Failed to like quiz:", err);
    }
  };

  const handleDislike = async () => {
    if (!isLoggedIn || !quizId || !quiz) return;

    try {
      await dislikeQuiz(quizId);
      updateQuizDislikes(quiz.dislikes + 1);
    } catch (err) {
      console.error("Failed to dislike quiz:", err);
    }
  };

  const handleTryAgain = () => {
    resetResults();
    resetAnswers();
  };

  if (loading) {
    return <QuizLoadingScreen />;
  }

  if (error || !quiz) {
    return (
      <QuizErrorScreen
        error={error || "Quiz not found"}
        onGoBack={() => navigate(-1)}
      />
    );
  }

  if (showResults) {
    return (
      <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
        <div className="container mx-auto max-w-3xl px-4 py-8">
          <Button
            variant="ghost"
            className="mb-6 gap-2"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="size-4" />
            Back to Quizzes
          </Button>

          <QuizResultsCard
            score={score}
            totalQuestions={quiz.questions.length}
            percentage={percentage}
            quizTitle={quiz.title}
            likes={quiz.likes}
            dislikes={quiz.dislikes}
            isLoggedIn={isLoggedIn}
            onLike={handleLike}
            onDislike={handleDislike}
            onTryAgain={handleTryAgain}
            onBackToQuizzes={() => navigate(`/courses/${courseId}/quizzes`)}
          />

          <QuizReviewSection
            questions={quiz.questions}
            userAnswers={userAnswers}
            isQuestionCorrect={isQuestionCorrect}
          />
        </div>
      </div>
    );
  }

  const answeredCount = getAnsweredCount();
  const canSubmit = answeredCount === quiz.questions.length;

  return (
    <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
      <div className="container mx-auto px-4 py-6 max-w-4xl">
        <Button
          variant="ghost"
          className="mb-4 gap-2"
          onClick={() => navigate(-1)}
        >
          <ArrowLeft className="h-4 w-4" />
          Back to Quizzes
        </Button>

        <div className="mb-6">
          <QuizHeader
            quiz={quiz}
            answeredCount={answeredCount}
            totalQuestions={quiz.questions.length}
            courseId={courseId!}
            quizId={quizId!}
          />
        </div>

        <div className="space-y-6">
          {quiz.questions.map((question, index) => (
            <QuizQuestionCard
              key={question.id}
              question={question}
              index={index}
              isAnswered={isAnswered(question.id)}
              userAnswer={userAnswers.get(question.id)}
              onSingleChoice={handleSingleChoice}
              onMultipleChoice={handleMultipleChoice}
              onShortAnswer={handleShortAnswer}
              isHighlighted={highlightedQuestionId === question.id}
            />
          ))}
        </div>

        <QuizSubmitBar
          answeredCount={answeredCount}
          totalQuestions={quiz.questions.length}
          canSubmit={canSubmit}
          isSubmitting={isSubmitting}
          showConfirmDialog={showConfirmDialog}
          onSubmitClick={handleSubmitClick}
          onConfirmSubmit={() =>
            handleConfirmSubmit(quiz.questions, userAnswers, quizId)
          }
          onCancelSubmit={handleCancelSubmit}
          unansweredQuestionIds={getUnansweredQuestionIds()}
          onScrollToQuestion={handleScrollToQuestion}
        />
      </div>
    </div>
  );
};

export default QuizQuestionsPage;
