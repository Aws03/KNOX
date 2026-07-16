import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { register } from "@/features/auth/api";
import { useAuth } from "@/app/providers/useAuth";

type RegistrationData = {
  fullName: string;
  email: string;
  password: string;
  majorId: number;
};

export const useRegistration = () => {
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleRegistration = async (data: RegistrationData) => {
    try {
      await register(data);

      try {
        await login(data.email, data.password);
        toast.success("Welcome to Uni-Hub!", {
          description: "Your account has been created successfully.",
          duration: 3000,
        });
        setTimeout(() => navigate("/"), 1500);
      } catch (loginError) {
        console.error("Auto-login failed:", loginError);
        toast.info("Registration successful", {
          description: "Please log in with your credentials.",
          duration: 4000,
        });
        setTimeout(() => navigate("/auth/login"), 1500);
      }
    } catch (error: unknown) {
      console.error("Registration failed:", error);
      let errorMessage = "Registration failed. Please try again.";

      if (error && typeof error === "object" && "response" in error) {
        const axiosError = error as {
          response?: { data?: { message?: string } };
        };
        errorMessage = axiosError.response?.data?.message || errorMessage;
      } else if (error instanceof Error) {
        errorMessage = error.message;
      }

      toast.error("Registration failed", { description: errorMessage });
      throw error;
    }
  };

  return { handleRegistration };
};
