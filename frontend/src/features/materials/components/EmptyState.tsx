import { Card } from "@/shared/ui/card";

export function EmptyState() {
  return (
    <Card className="p-12 text-center">
      <p className="text-muted-foreground">
        This folder is empty. No materials available yet.
      </p>
    </Card>
  );
}
