import { uploadTemporaryFile } from "../api";
import { isValidImageFile, isValidImageSize } from "../utils";
import type { Question } from "../types";

export const useImageUpload = (
  setQuestions: React.Dispatch<React.SetStateAction<Question[]>>,
  setError: (error: string | null) => void
) => {
  const uploadImage = async (file: File): Promise<string | null> => {
    try {
      return await uploadTemporaryFile(file);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to upload image");
      return null;
    }
  };

  const handleQuestionImageUpload = async (
    questionId: string,
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validate file
    if (!isValidImageFile(file)) {
      setError("Please select an image file");
      event.target.value = "";
      return;
    }

    if (!isValidImageSize(file)) {
      setError("Image size must be less than 5MB");
      event.target.value = "";
      return;
    }

    // Update state to show uploading status
    setQuestions((prevQuestions) =>
      prevQuestions.map((q) =>
        q.id === questionId ? { ...q, uploadingImage: true } : q
      )
    );

    const imageUrl = await uploadImage(file);

    if (imageUrl) {
      // Update state with the new image URL
      setQuestions((prevQuestions) =>
        prevQuestions.map((q) =>
          q.id === questionId ? { ...q, imageUrl, uploadingImage: false } : q
        )
      );
    } else {
      // Failed to upload, reset uploading status
      setQuestions((prevQuestions) =>
        prevQuestions.map((q) =>
          q.id === questionId ? { ...q, uploadingImage: false } : q
        )
      );
    }

    // Reset the input so the same file can be selected again
    event.target.value = "";
  };

  const handleChoiceImageUpload = async (
    questionId: string,
    choiceId: string,
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validate file
    if (!isValidImageFile(file)) {
      setError("Please select an image file");
      event.target.value = "";
      return;
    }

    if (!isValidImageSize(file)) {
      setError("Image size must be less than 5MB");
      event.target.value = "";
      return;
    }

    // Update state to show uploading status
    setQuestions((prevQuestions) =>
      prevQuestions.map((q) => {
        if (q.id === questionId) {
          return {
            ...q,
            choices: q.choices.map((c) =>
              c.id === choiceId ? { ...c, uploadingImage: true } : c
            ),
          };
        }
        return q;
      })
    );

    const imageUrl = await uploadImage(file);

    if (imageUrl) {
      // Update state with the new image URL
      setQuestions((prevQuestions) =>
        prevQuestions.map((q) => {
          if (q.id === questionId) {
            return {
              ...q,
              choices: q.choices.map((c) =>
                c.id === choiceId
                  ? { ...c, imageUrl, uploadingImage: false }
                  : c
              ),
            };
          }
          return q;
        })
      );
    } else {
      // Failed to upload, reset uploading status
      setQuestions((prevQuestions) =>
        prevQuestions.map((q) => {
          if (q.id === questionId) {
            return {
              ...q,
              choices: q.choices.map((c) =>
                c.id === choiceId ? { ...c, uploadingImage: false } : c
              ),
            };
          }
          return q;
        })
      );
    }

    // Reset the input so the same file can be selected again
    event.target.value = "";
  };

  const removeQuestionImage = (questionId: string) => {
    setQuestions((prevQuestions) =>
      prevQuestions.map((q) =>
        q.id === questionId ? { ...q, imageUrl: null } : q
      )
    );
  };

  const removeChoiceImage = (questionId: string, choiceId: string) => {
    setQuestions((prevQuestions) =>
      prevQuestions.map((q) => {
        if (q.id === questionId) {
          return {
            ...q,
            choices: q.choices.map((c) =>
              c.id === choiceId ? { ...c, imageUrl: null } : c
            ),
          };
        }
        return q;
      })
    );
  };

  return {
    handleQuestionImageUpload,
    handleChoiceImageUpload,
    removeQuestionImage,
    removeChoiceImage,
  };
};
