import { Trash2, Upload, X, ImageIcon } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { Textarea } from "@/shared/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Checkbox } from "@/shared/ui/checkbox";
import type { Question, Choice, QuestionTypeValue } from "../types";
import { QuestionType } from "../types";

interface QuestionCardProps {
  question: Question;
  index: number;
  onRemove: () => void;
  onUpdateQuestion: <K extends keyof Question>(
    field: K,
    value: Question[K]
  ) => void;
  onTypeChange: (newType: QuestionTypeValue) => void;
  onAddChoice: () => void;
  onRemoveChoice: (choiceId: string) => void;
  onUpdateChoice: <K extends keyof Choice>(
    choiceId: string,
    field: K,
    value: Choice[K]
  ) => void;
  onQuestionImageUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onChoiceImageUpload: (
    choiceId: string,
    event: React.ChangeEvent<HTMLInputElement>
  ) => void;
  onRemoveQuestionImage: () => void;
  onRemoveChoiceImage: (choiceId: string) => void;
}

export const QuestionCard = ({
  question,
  index,
  onRemove,
  onUpdateQuestion,
  onTypeChange,
  onAddChoice,
  onRemoveChoice,
  onUpdateChoice,
  onQuestionImageUpload,
  onChoiceImageUpload,
  onRemoveQuestionImage,
  onRemoveChoiceImage,
}: QuestionCardProps) => {
  return (
    <Card className="overflow-hidden">
      <CardHeader className="bg-muted/30">
        <div className="flex items-start justify-between gap-4">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 font-semibold text-primary">
              {index + 1}
            </div>
            <div>
              <CardTitle className="text-lg">Question {index + 1}</CardTitle>
              <CardDescription>
                Configure question type and content
              </CardDescription>
            </div>
          </div>
          <Button variant="ghost" size="icon" onClick={onRemove}>
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      </CardHeader>

      <CardContent className="pt-6 space-y-4">
        {/* Question Type */}
        <div className="space-y-2">
          <Label>
            Question Type <span className="text-destructive">*</span>
          </Label>
          <Select
            value={question.type.toString()}
            onValueChange={(value) => {
              const newType = parseInt(value) as QuestionTypeValue;
              onTypeChange(newType);
            }}
          >
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={QuestionType.SingleChoice.toString()}>
                Single Choice
              </SelectItem>
              <SelectItem value={QuestionType.MultipleChoice.toString()}>
                Multiple Choice
              </SelectItem>
              <SelectItem value={QuestionType.TrueFalse.toString()}>
                True/False
              </SelectItem>
              <SelectItem value={QuestionType.ShortAnswer.toString()}>
                Short Answer
              </SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* Question Text */}
        <div className="space-y-2">
          <Label>
            Question Text <span className="text-destructive">*</span>
          </Label>
          <Textarea
            placeholder="Enter your question..."
            value={question.text}
            onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
              onUpdateQuestion("text", e.target.value)
            }
            rows={3}
            maxLength={1000}
          />
        </div>

        {/* Question Image */}
        <div className="space-y-2">
          <Label>Question Image (Optional)</Label>
          {question.imageUrl ? (
            <div className="relative inline-block">
              <img
                src={question.imageUrl}
                alt="Question"
                className="max-w-xs rounded-lg border"
              />
              <Button
                variant="destructive"
                size="icon"
                className="absolute top-2 right-2"
                onClick={onRemoveQuestionImage}
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          ) : (
            <div>
              <Input
                type="file"
                accept="image/*"
                onChange={onQuestionImageUpload}
                disabled={question.uploadingImage}
                className="hidden"
                id={`question-image-${question.id}`}
              />
              <Button
                variant="outline"
                className="w-full"
                onClick={() =>
                  document
                    .getElementById(`question-image-${question.id}`)
                    ?.click()
                }
                disabled={question.uploadingImage}
              >
                {question.uploadingImage ? (
                  <>
                    <Upload className="h-4 w-4 mr-2 animate-pulse" />
                    Uploading...
                  </>
                ) : (
                  <>
                    <ImageIcon className="h-4 w-4 mr-2" />
                    Upload Image
                  </>
                )}
              </Button>
            </div>
          )}
        </div>

        {/* Choices */}
        {question.type !== QuestionType.ShortAnswer && (
          <div className="space-y-4 pt-4 border-t">
            <div className="flex items-center justify-between">
              <Label>
                Choices <span className="text-destructive">*</span>
              </Label>
              {question.type !== QuestionType.TrueFalse && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onAddChoice}
                  disabled={question.choices.length >= 10}
                >
                  <ImageIcon className="h-4 w-4 mr-2" />
                  Add Choice
                </Button>
              )}
            </div>

            <div className="space-y-3">
              {question.choices.map((choice, cIndex) => (
                <div
                  key={choice.id}
                  className="flex flex-col gap-3 p-4 border rounded-lg bg-muted/30"
                >
                  <div className="flex items-start gap-3">
                    <div className="flex items-center gap-3 flex-1">
                      <Checkbox
                        checked={choice.isCorrect}
                        onCheckedChange={(checked) =>
                          onUpdateChoice(choice.id, "isCorrect", !!checked)
                        }
                        id={`choice-${choice.id}`}
                      />
                      <div className="flex-1 space-y-2">
                        <Label htmlFor={`choice-text-${choice.id}`}>
                          Choice {cIndex + 1}
                        </Label>
                        <Input
                          id={`choice-text-${choice.id}`}
                          placeholder="Enter choice text..."
                          value={choice.text}
                          onChange={(e) =>
                            onUpdateChoice(choice.id, "text", e.target.value)
                          }
                          maxLength={500}
                          disabled={question.type === QuestionType.TrueFalse}
                        />
                      </div>
                    </div>
                    {question.type !== QuestionType.TrueFalse &&
                      question.choices.length > 2 && (
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => onRemoveChoice(choice.id)}
                        >
                          <Trash2 className="h-4 w-4 text-destructive" />
                        </Button>
                      )}
                  </div>

                  {/* Choice Image */}
                  <div className="pl-9">
                    {choice.imageUrl ? (
                      <div className="relative inline-block">
                        <img
                          src={choice.imageUrl}
                          alt={`Choice ${cIndex + 1}`}
                          className="max-w-xs rounded-lg border"
                        />
                        <Button
                          variant="destructive"
                          size="icon"
                          className="absolute top-2 right-2"
                          onClick={() => onRemoveChoiceImage(choice.id)}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : (
                      <div>
                        <Input
                          type="file"
                          accept="image/*"
                          onChange={(e) => onChoiceImageUpload(choice.id, e)}
                          disabled={choice.uploadingImage}
                          className="hidden"
                          id={`choice-image-${choice.id}`}
                        />
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() =>
                            document
                              .getElementById(`choice-image-${choice.id}`)
                              ?.click()
                          }
                          disabled={choice.uploadingImage}
                        >
                          {choice.uploadingImage ? (
                            <>
                              <Upload className="h-4 w-4 mr-2 animate-pulse" />
                              Uploading...
                            </>
                          ) : (
                            <>
                              <ImageIcon className="h-4 w-4 mr-2" />
                              Add Image
                            </>
                          )}
                        </Button>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>

            {question.type === QuestionType.MultipleChoice && (
              <p className="text-xs text-muted-foreground">
                Check all correct answers for multiple choice questions
              </p>
            )}
          </div>
        )}

        {/* Short Answer */}
        {question.type === QuestionType.ShortAnswer && (
          <div className="space-y-2 pt-4 border-t">
            <Label>
              Correct Answer <span className="text-destructive">*</span>
            </Label>
            <Input
              placeholder="Enter the correct answer..."
              value={question.choices[0]?.text || ""}
              onChange={(e) =>
                onUpdateChoice(question.choices[0]?.id, "text", e.target.value)
              }
              maxLength={500}
            />
            <p className="text-xs text-muted-foreground">
              The answer will be checked case-insensitively
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
