import { apiClient } from "@/lib/api/apiClient";

export interface UpdateProfileData {
  fullName?: string;
  universityId?: number;
  facultyId?: number;
  majorId?: number;
}

/**
 * Updates the current user's profile information
 */
export const updateProfile = async (data: UpdateProfileData): Promise<void> => {
  try {
    await apiClient.put("/Users/me", data);
  } catch (error) {
    console.error("Failed to update profile:", error);
    throw new Error("Failed to update profile. Please try again later.");
  }
};

/**
 * Uploads a profile picture for the current user
 * Returns the URL of the uploaded image
 */
export const uploadProfilePicture = async (file: File): Promise<string> => {
  try {
    const formData = new FormData();
    formData.append("image", file);

    const response = await apiClient.post<{ url: string }>(
      "/Users/me/profile-picture",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );

    return response.data.url;
  } catch (error) {
    console.error("Failed to upload profile picture:", error);
    throw new Error(
      "Failed to upload profile picture. Please try again later."
    );
  }
};

/**
 * Deletes the current user's profile picture
 */
export const deleteProfilePicture = async (): Promise<void> => {
  try {
    await apiClient.delete("/Users/me/profile-picture");
  } catch (error) {
    console.error("Failed to delete profile picture:", error);
    throw new Error(
      "Failed to delete profile picture. Please try again later."
    );
  }
};
