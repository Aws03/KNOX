export interface AnalyticsData {
  totalUniversities: number;
  totalFaculties: number;
  totalMajors: number;
  totalCourses: number;
  totalQuizzes: number;
  totalMaterials: number;
  totalUsers: number;
  quizzesGrowth: { month: string; count: number }[];
  materialsGrowth: { month: string; count: number }[];
  usersGrowth: { month: string; count: number }[];
  activeQuizAttemptsLast30Days: number;
  averageMaterialsPerCourse: number;
  averageQuizzesPerCourse: number;
}
