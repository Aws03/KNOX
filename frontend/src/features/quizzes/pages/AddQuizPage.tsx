import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft, Plus } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { createQuiz, getUserIdFromToken } from "../api";
import { validateQuiz } from "../utils";
import { useQuizForm } from "../hooks/useQuizForm";
import { useImageUpload } from "../hooks/useImageUpload";
import { QuizDetailsForm } from "../components/QuizDetailsForm";
import { EmptyQuestionsState } from "../components/EmptyQuestionsState";
import { QuestionCard } from "../components/QuestionCard";
import { ErrorAlert } from "../components/ErrorAlert";
import { QuizSubmitSection } from "../components/QuizSubmitSection";

const AddQuizPage = () => {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
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
  } = useQuizForm();

  const {
    handleQuestionImageUpload,
    handleChoiceImageUpload,
    removeQuestionImage,
    removeChoiceImage,
  } = useImageUpload(setQuestions, setError);

  const handleSubmit = async () => {
    setError(null);

    const validationError = validateQuiz(title, questions);
    if (validationError) {
      setError(validationError);
      window.scrollTo({ top: 0, behavior: "smooth" });
      return;
    }

    setIsSubmitting(true);

    try {
      const writerId = getUserIdFromToken();

      const payload = {
        title: title.trim(),
        writerId,
        courseId: parseInt(courseId!),
        description: description.trim() || null,
        questions: questions.map((q) => ({
          text: q.text.trim(),
          quizId: 0,
          imageUrl: q.imageUrl,
          type: q.type,
          choices: q.choices.map((c) => ({
            text: c.text.trim(),
            imageUrl: c.imageUrl,
            isCorrect: c.isCorrect,
          })),
        })),
      };

      await createQuiz(payload);

      // Navigate back to quizzes page
      navigate(`/courses/${courseId}/quizzes`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create quiz");
      window.scrollTo({ top: 0, behavior: "smooth" });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
      <div className="container mx-auto px-4 py-6 max-w-5xl">
        {/* Header */}
        <div className="mb-6">
          <Button
            variant="ghost"
            className="mb-4 gap-2"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Quizzes
          </Button>

          <h1 className="text-3xl font-bold tracking-tight mb-2">
            Create New Quiz
          </h1>
          <p className="text-muted-foreground">
            Add questions and choices to create an engaging quiz for your course
          </p>
        </div>

        {/* Error Alert */}
        {error && <ErrorAlert error={error} onDismiss={() => setError(null)} />}

        {/* Quiz Details */}
        <QuizDetailsForm
          title={title}
          description={description}
          onTitleChange={setTitle}
          onDescriptionChange={setDescription}
        />

        {/* Questions */}
        <div className="space-y-6 mb-6">
          <div>
            <h2 className="text-2xl font-bold">Questions</h2>
            <p className="text-sm text-muted-foreground">
              {questions.length} / 100 questions
            </p>
          </div>

          {questions.length === 0 && (
            <EmptyQuestionsState onAddQuestion={addQuestion} />
          )}

          {questions.map((question, qIndex) => (
            <QuestionCard
              key={question.id}
              question={question}
              index={qIndex}
              onRemove={() => removeQuestion(question.id)}
              onUpdateQuestion={(field, value) =>
                updateQuestion(question.id, field, value)
              }
              onTypeChange={(newType) =>
                handleQuestionTypeChange(question.id, newType)
              }
              onAddChoice={() => addChoice(question.id)}
              onRemoveChoice={(choiceId) => removeChoice(question.id, choiceId)}
              onUpdateChoice={(choiceId, field, value) =>
                updateChoice(question.id, choiceId, field, value)
              }
              onQuestionImageUpload={(e) =>
                handleQuestionImageUpload(question.id, e)
              }
              onChoiceImageUpload={(choiceId, e) =>
                handleChoiceImageUpload(question.id, choiceId, e)
              }
              onRemoveQuestionImage={() => removeQuestionImage(question.id)}
              onRemoveChoiceImage={(choiceId) =>
                removeChoiceImage(question.id, choiceId)
              }
            />
          ))}

          {/* Add Question Button */}
          {questions.length > 0 && (
            <div className="flex justify-center pt-4">
              <Button
                onClick={addQuestion}
                disabled={questions.length >= 100}
                size="lg"
                variant="outline"
                className="w-full sm:w-auto"
              >
                <Plus className="h-4 w-4 mr-2" />
                Add Another Question
              </Button>
            </div>
          )}
        </div>

        {/* Submit Section */}
        <QuizSubmitSection
          questionsCount={questions.length}
          isSubmitting={isSubmitting}
          onCancel={() => navigate(-1)}
          onSubmit={handleSubmit}
        />
      </div>
    </div>
  );
};

export default AddQuizPage;
