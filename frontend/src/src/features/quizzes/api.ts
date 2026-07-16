import { apiClient } from "@/lib/api/apiClient";
import { getPaginated } from "@/lib/api/pagination";
import type { PaginatedResponse } from "@/lib/api/types";
import type {
  QuizDetails,
  CreateQuizPayload,
  QuizListItem,
  ReactionResult,
} from "./types";
import { getStoredToken } from "@/hooks/authStorage";

/**
 * Upload a temporary file (image) to the server
 * @param file - The file to upload
 * @returns The URL of the uploaded file or null if failed
 */
export const uploadTemporaryFile = async (
  file: File
): Promise<string | null> => {
  try {
    const formData = new FormData();
    formData.append("file", file);

    const response = await apiClient.post("/files/upload/temporary", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });

    return response.data.fileUrl;
  } catch (err) {
    console.error("Failed to upload image:", err);
    throw new Error("Failed to upload image. Please try again.");
  }
};

/**
 * Create a new quiz
 * @param payload - The quiz data to create
 */
export const createQuiz = async (payload: CreateQuizPayload): Promise<void> => {
  try {
    await apiClient.post("/quizzes", payload);
  } catch (err) {
    console.error("Failed to create quiz:", err);
    const error = err as {
      response?: { data?: { message?: string } };
      message?: string;
    };
    throw new Error(
      error.response?.data?.message ||
        error.message ||
        "Failed to create quiz. Please try again."
    );
  }
};

/**
 * Fetch quiz details by ID
 * @param quizId - The ID of the quiz to fetch
 * @returns Quiz details
 */
export const fetchQuizById = async (quizId: string): Promise<QuizDetails> => {
  try {
    const response = await apiClient.get<QuizDetails>(`/quizzes/${quizId}`);
    return response.data;
  } catch (err) {
    console.error("Failed to fetch quiz details:", err);
    const error = err instanceof Error ? err.message : "Failed to load quiz";
    throw new Error(error);
  }
};

/**
 * Fetch paginated quizzes for a course
 * @param courseId - The course ID
 * @param page - Page number
 * @param pageSize - Number of items per page
 * @returns Paginated quiz list
 */
export const fetchQuizzesByCourse = async (
  courseId: string,
  page: number,
  pageSize: number
): Promise<PaginatedResponse<QuizListItem>> => {
  try {
    return await getPaginated<QuizListItem>(
      `/quizzes/by-course/${courseId}`,
      page,
      pageSize
    );
  } catch (err) {
    console.error("Failed to fetch quizzes:", err);
    const error = err instanceof Error ? err.message : "Failed to load quizzes";
    throw new Error(error);
  }
};

// Matches the backend's Domain.Quizzes.Enums.ReactionType (Dislike = 0, Like = 1).
const ReactionType = { Dislike: 0, Like: 1 } as const;

// The backend derives which user is reacting from the authenticated session -
// it never trusts a client-supplied user id - and returns the quiz's updated
// like/dislike counts so the caller can sync both counters at once (switching
// from Like to Dislike changes both, not just the one that was clicked).
const addReaction = async (
  quizId: string,
  reactionType: (typeof ReactionType)[keyof typeof ReactionType]
): Promise<ReactionResult> => {
  try {
    const response = await apiClient.post<ReactionResult>(
      `/quizzes/${quizId}/reactions`,
      { reactionType }
    );
    return response.data;
  } catch (err) {
    const error = err as {
      response?: { data?: { errors?: { description?: string }[] } };
    };
    const message = error.response?.data?.errors?.[0]?.description;
    throw new Error(message || "Failed to update your reaction. Please try again.");
  }
};

/**
 * Like a quiz. Returns the quiz's updated like/dislike counts.
 * @param quizId - The ID of the quiz to like
 */
export const likeQuiz = (quizId: string): Promise<ReactionResult> =>
  addReaction(quizId, ReactionType.Like);

/**
 * Dislike a quiz. Returns the quiz's updated like/dislike counts.
 * @param quizId - The ID of the quiz to dislike
 */
export const dislikeQuiz = (quizId: string): Promise<ReactionResult> =>
  addReaction(quizId, ReactionType.Dislike);

/**
 * Submit quiz score to the server
 * @param quizId - The ID of the quiz
 * @param score - The score out of 100
 */
export const submitQuizScore = async (
  quizId: string,
  score: number
): Promise<void> => {
  try {
    await apiClient.post(`/quizzes/${quizId}/attempts`, {
      score: Math.round(score),
    });
  } catch (err) {
    console.error("Failed to submit quiz score:", err);
    // Don't throw - submission is optional for tracking purposes
  }
};

/**
 * Get user ID from auth token
 * @returns User ID or default value (1)
 */
export const getUserIdFromToken = (): number => {
  try {
    const authData = getStoredToken();
    if (!authData) return 1;

    const tokenPayload = JSON.parse(atob(authData.split(".")[1]));
    return parseInt(
      tokenPayload[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ] || "1"
    );
  } catch (e) {
    console.error("Failed to parse auth token:", e);
    return 1;
  }
};

/**
 * Check if user is logged in
 * @returns true if user is logged in
 */
export const isUserLoggedIn = (): boolean => {
  return getStoredToken() !== null;
};
