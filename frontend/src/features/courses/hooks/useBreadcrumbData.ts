import { useState, useEffect } from "react";
import type { University, Faculty, Major } from "@/features/courses/types";
import {
  fetchUniversityById,
  fetchFacultyById,
  fetchMajorById,
} from "@/features/courses/api";

export const useBreadcrumbData = (
  universityId?: string,
  facultyId?: string,
  majorId?: string
) => {
  const [university, setUniversity] = useState<University | null>(null);
  const [faculty, setFaculty] = useState<Faculty | null>(null);
  const [major, setMajor] = useState<Major | null>(null);

  useEffect(() => {
    const fetchBreadcrumbData = async () => {
      try {
        if (universityId) {
          const uniData = await fetchUniversityById(parseInt(universityId));
          setUniversity(uniData);
        }

        if (facultyId) {
          const facData = await fetchFacultyById(parseInt(facultyId));
          setFaculty(facData);
        }

        if (majorId) {
          const majData = await fetchMajorById(parseInt(majorId));
          setMajor(majData);
        }
      } catch (error) {
        console.error("Failed to fetch breadcrumb data:", error);
      }
    };

    fetchBreadcrumbData();
  }, [universityId, facultyId, majorId]);

  return { university, faculty, major };
};
