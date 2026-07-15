import { useState } from "react";
import { EyeIcon, EyeOffIcon, CheckCircle2, XCircle } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { type PasswordValidation } from "../hooks/usePasswordValidation";

type PasswordInputProps = {
  value: string;
  onChange: (value: string) => void;
  validation: PasswordValidation;
  disabled?: boolean;
};

export const PasswordInput = ({
  value,
  onChange,
  validation,
  disabled,
}: PasswordInputProps) => {
  const [isVisible, setIsVisible] = useState(false);

  return (
    <div className="w-full space-y-1">
      <Label className="leading-5" htmlFor="password">
        Password*
      </Label>
      <div className="relative">
        <Input
          id="password"
          type={isVisible ? "text" : "password"}
          placeholder="••••••••••••••••"
          className="pr-9"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          disabled={disabled}
        />
        <Button
          type="button"
          variant="ghost"
          size="icon"
          onClick={() => setIsVisible((prev) => !prev)}
          className="text-muted-foreground focus-visible:ring-ring/50 absolute inset-y-0 right-0 rounded-l-none hover:bg-transparent"
          disabled={disabled}
        >
          {isVisible ? <EyeOffIcon /> : <EyeIcon />}
          <span className="sr-only">
            {isVisible ? "Hide password" : "Show password"}
          </span>
        </Button>
      </div>

      {value && (
        <div className="mt-2 space-y-1 rounded-md border p-3 bg-muted/30">
          <p className="text-xs font-semibold mb-2">Password must contain:</p>
          <div className="space-y-1.5">
            <ValidationItem
              isValid={validation.minLength}
              text="At least 8 characters"
            />
            <ValidationItem
              isValid={validation.hasUppercase}
              text="One uppercase letter (A-Z)"
            />
            <ValidationItem
              isValid={validation.hasLowercase}
              text="One lowercase letter (a-z)"
            />
            <ValidationItem
              isValid={validation.hasSpecialChar}
              text="One special character (!@#$%^&*...)"
            />
          </div>
        </div>
      )}
    </div>
  );
};

const ValidationItem = ({
  isValid,
  text,
}: {
  isValid: boolean;
  text: string;
}) => (
  <div className="flex items-center gap-2 text-xs">
    {isValid ? (
      <CheckCircle2 className="h-3.5 w-3.5 text-green-600" />
    ) : (
      <XCircle className="h-3.5 w-3.5 text-muted-foreground" />
    )}
    <span className={isValid ? "text-green-600" : "text-muted-foreground"}>
      {text}
    </span>
  </div>
);
