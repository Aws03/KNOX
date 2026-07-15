import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";

const PAGE_SIZES = [5, 10, 12, 20, 50];

interface PageSizeSelectorProps {
  pageSize?: number;
  onChange: (size: number) => void;
}

export function PageSizeSelector({
  pageSize = 12,
  onChange,
}: PageSizeSelectorProps) {
  return (
    <div className="flex items-center gap-2 text-sm text-muted-foreground">
      <span>Rows per page:</span>
      <Select
        value={pageSize.toString()}
        onValueChange={(value) => onChange(Number(value))}
      >
        <SelectTrigger className="w-20">
          <SelectValue placeholder={pageSize.toString()} />
        </SelectTrigger>
        <SelectContent>
          {PAGE_SIZES.map((size) => (
            <SelectItem key={size} value={size.toString()}>
              {size}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
