import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";

type BasicInputFieldProps = {
  id: string;
  label: string;
  type: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
};

export const BasicInputField = ({
  id,
  label,
  type,
  placeholder,
  value,
  onChange,
  disabled,
}: BasicInputFieldProps) => (
  <div className="space-y-1">
    <Label className="leading-5" htmlFor={id}>
      {label}
    </Label>
    <Input
      type={type}
      id={id}
      placeholder={placeholder}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      disabled={disabled}
    />
  </div>
);
