import { ArrowLeft, AlertCircle } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card } from "@/shared/ui/card";

interface QuizErrorScreenProps {
  error: string;
  onGoBack: () => void;
}

export const QuizErrorScreen = ({ error, onGoBack }: QuizErrorScreenProps) => {
  return (
    <div className="min-h-screen flex items-center justify-center p-4">
      <Card className="max-w-md w-full p-8 bg-linear-to-br from-destructive/5 to-destructive/10 border-destructive/20">
        <div className="flex flex-col items-center text-center">
          <div className="flex h-20 w-20 items-center justify-center rounded-full bg-destructive/10 mb-4">
            <AlertCircle className="h-10 w-10 text-destructive" />
          </div>
          <h3 className="text-lg font-semibold mb-2 text-destructive">
            Failed to load quiz
          </h3>
          <p className="text-sm text-muted-foreground mb-4">{error}</p>
          <Button onClick={onGoBack} variant="outline">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Go Back
          </Button>
        </div>
      </Card>
    </div>
  );
};
