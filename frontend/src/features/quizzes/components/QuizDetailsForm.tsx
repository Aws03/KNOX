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

interface QuizDetailsFormProps {
  title: string;
  description: string;
  onTitleChange: (value: string) => void;
  onDescriptionChange: (value: string) => void;
}

export const QuizDetailsForm = ({
  title,
  description,
  onTitleChange,
  onDescriptionChange,
}: QuizDetailsFormProps) => {
  return (
    <Card className="mb-6">
      <CardHeader>
        <CardTitle>Quiz Details</CardTitle>
        <CardDescription>Basic information about your quiz</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="title">
            Title <span className="text-destructive">*</span>
          </Label>
          <Input
            id="title"
            placeholder="Enter quiz title..."
            value={title}
            onChange={(e) => onTitleChange(e.target.value)}
            maxLength={200}
          />
        </div>

        <div className="space-y-2">
          <Label htmlFor="description">Description (Optional)</Label>
          <Textarea
            id="description"
            placeholder="Enter quiz description..."
            value={description}
            onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
              onDescriptionChange(e.target.value)
            }
            rows={3}
            maxLength={1000}
          />
        </div>
      </CardContent>
    </Card>
  );
};
