import { useState } from "react";
import { EyeIcon, EyeOffIcon, CheckCircle2, XCircle } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";

type ConfirmPasswordInputProps = {
  value: string;
  onChange: (value: string) => void;
  passwordsMatch: boolean;
  disabled?: boolean;
};

export const ConfirmPasswordInput = ({
  value,
  onChange,
  passwordsMatch,
  disabled,
}: ConfirmPasswordInputProps) => {
  const [isVisible, setIsVisible] = useState(false);

  return (
    <div className="w-full space-y-1">
      <Label className="leading-5" htmlFor="confirmPassword">
        Confirm Password*
      </Label>
      <div className="relative">
        <Input
          id="confirmPassword"
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
        <div className="flex items-center gap-2 text-xs mt-1.5">
          {passwordsMatch ? (
            <>
              <CheckCircle2 className="h-3.5 w-3.5 text-green-600" />
              <span className="text-green-600">Passwords match</span>
            </>
          ) : (
            <>
              <XCircle className="h-3.5 w-3.5 text-destructive" />
              <span className="text-destructive">Passwords do not match</span>
            </>
          )}
        </div>
      )}
    </div>
  );
};
