// Course display type (used in UI)
export interface Course {
  id: number;
  code: string;
  name: string;
  major: string;
  badges: string[];
  materials: number;
  quizzes: number;
  hasCourseInfo: boolean;
}

// Backend API response type
export interface CourseApiResponse {
  id: number;
  courseName: string;
  description: string;
  courseCode: string;
  credits: number;
  requirementType: string;
  requirementNature: string;
  numberOfMaterials: number;
  numberOfQuizzes: number;
  hasCourseInfo: boolean;
}

// Resource type enum values
export type ResourceType =
  | "ECampusCourse"
  | "YouTubeVideo"
  | "YouTubePlaylist"
  | "Article"
  | "BlogPost"
  | "UdemyCourse"
  | "CourseraCourse"
  | "EdXCourse"
  | "LinkedInLearning"
  | "PluralSight"
  | "OtherPlatformCourse"
  | "Other";

// Difficulty level enum
export type DifficultyLevel = "Easy" | "Moderate" | "Hard";

// Course resource type
export interface CourseResource {
  id: number;
  courseInfoId: number;
  title: string;
  type: ResourceType;
  url: string;
  description: string;
  demonstrationVideoUrl: string;
  hasDemonstrationVideo: boolean;
}

// Course info API response
export interface CourseInfo {
  id: number;
  courseId: number;
  difficultyLevel: DifficultyLevel;
  description: string;
  demonstrationVideoUrl: string;
  demonstrationVideoTitle: string;
  resources: CourseResource[];
  resourceCount: number;
}

// Filter types
export interface CourseFilterValues {
  universityId?: number;
  collegeId?: number;
  majorId?: number;
  majorName?: string;
  courseType?: string;
  requirement?: string;
}

// Academic hierarchy types
export interface University {
  id: number;
  name: string;
}

export interface Faculty {
  id: number;
  name: string;
  universityId: number;
}

export interface Major {
  id: number;
  name: string;
  facultyId: number;
}

// Paginated state for filters
export interface PaginatedState<T> {
  items: T[];
  pageNumber: number;
  hasNextPage: boolean;
  loading: boolean;
  parentId: number | null;
}

// Requirement mappings
export const REQUIREMENT_TYPE_MAP: Record<string, string> = {
  Specialist: "1",
  "Free Course": "2",
  "Core Course": "3",
  Elective: "4",
  "Lab Course": "5",
};

export const REQUIREMENT_NATURE_MAP: Record<string, string> = {
  Obligatory: "1",
  Optional: "2",
  "Prerequisite Required": "3",
  "No Prerequisites": "4",
  Compulsory: "1",
};

// Available filter options
export const COURSE_TYPES = [
  "Specialist",
  "Free Course",
  "Core Course",
  "Elective",
  "Lab Course",
];

export const REQUIREMENT_TYPES = [
  "Obligatory",
  "Optional",
  "Prerequisite Required",
  "No Prerequisites",
];

// Utility function to create initial paginated state
export const createInitialPaginatedState = <T>(): PaginatedState<T> => ({
  items: [],
  pageNumber: 0,
  hasNextPage: true,
  loading: false,
  parentId: null,
});
