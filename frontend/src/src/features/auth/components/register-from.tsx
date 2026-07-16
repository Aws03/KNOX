"use client";

import { useState } from "react";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/shared/ui/button";
import { CascadingSelects } from "@/shared/components/forms/CascadingSelects";
import { usePasswordValidation } from "../hooks/usePasswordValidation";
import { useAcademicSelection } from "../hooks/useAcademicSelection";
import { useRegistration } from "../hooks/useRegistration";
import { BasicInputField } from "./BasicInputField";
import { PasswordInput } from "./PasswordInput";
import { ConfirmPasswordInput } from "./ConfirmPasswordInput";

const RegisterForm = () => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { handleRegistration } = useRegistration();

  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const { validation, isValid: isPasswordValid } =
    usePasswordValidation(password);

  const {
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    setSelectedUniversity,
    setSelectedFaculty,
    setSelectedMajor,
    fetchUniversities,
    fetchFaculties,
    fetchMajors,
  } = useAcademicSelection();

  const passwordsMatch = password === confirmPassword && confirmPassword !== "";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!fullName.trim()) return toast.error("Full name is required");
    if (!email.trim() || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
      return toast.error("Please enter a valid email address");
    if (!isPasswordValid)
      return toast.error("Password does not meet requirements");
    if (!passwordsMatch) return toast.error("Passwords do not match");
    if (!selectedMajor)
      return toast.error("Please select your university, faculty, and major");

    setIsSubmitting(true);
    try {
      await handleRegistration({
        email: email.trim(),
        password,
        fullName: fullName.trim(),
        majorId: selectedMajor.id,
      });
    } catch {
      // Error handled in hook
    } finally {
      setIsSubmitting(false);
    }
  };

  const canSubmit =
    fullName &&
    email &&
    isPasswordValid &&
    passwordsMatch &&
    selectedMajor &&
    !isSubmitting;

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      <BasicInputField
        id="fullName"
        label="Full Name*"
        type="text"
        placeholder="Enter your full name"
        value={fullName}
        onChange={setFullName}
        disabled={isSubmitting}
      />

      <BasicInputField
        id="userEmail"
        label="Email address*"
        type="email"
        placeholder="Enter your email address"
        value={email}
        onChange={setEmail}
        disabled={isSubmitting}
      />

      <CascadingSelects
        selectedUniversity={selectedUniversity}
        selectedFaculty={selectedFaculty}
        selectedMajor={selectedMajor}
        onUniversityChange={setSelectedUniversity}
        onFacultyChange={setSelectedFaculty}
        onMajorChange={setSelectedMajor}
        fetchUniversities={fetchUniversities}
        fetchFaculties={fetchFaculties}
        fetchMajors={fetchMajors}
        required
      />

      <PasswordInput
        value={password}
        onChange={setPassword}
        validation={validation}
        disabled={isSubmitting}
      />

      <ConfirmPasswordInput
        value={confirmPassword}
        onChange={setConfirmPassword}
        passwordsMatch={passwordsMatch}
        disabled={isSubmitting}
      />

      <Button className="w-full" type="submit" disabled={!canSubmit}>
        {isSubmitting ? (
          <>
            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            Creating account...
          </>
        ) : (
          "Sign Up"
        )}
      </Button>
    </form>
  );
};

export default RegisterForm;
