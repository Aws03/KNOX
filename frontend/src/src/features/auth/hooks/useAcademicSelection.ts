import { useState, useCallback } from "react";
import { toast } from "sonner";
import { getPaginated } from "@/lib/api/pagination";
import {
  type University,
  type Faculty,
  type Major,
} from "@/shared/components/forms/CascadingSelects";

const PAGE_SIZE = 50;

export const useAcademicSelection = () => {
  const [selectedUniversity, setSelectedUniversity] =
    useState<University | null>(null);
  const [selectedFaculty, setSelectedFaculty] = useState<Faculty | null>(null);
  const [selectedMajor, setSelectedMajor] = useState<Major | null>(null);

  const fetchUniversities = useCallback(async (pageNumber: number) => {
    try {
      const res = await getPaginated<University>(
        "/universities",
        pageNumber,
        PAGE_SIZE
      );
      return { items: res.items, hasNextPage: res.hasNextPage };
    } catch (error) {
      console.error("Failed to load universities", error);
      toast.error("Failed to load universities");
      return { items: [], hasNextPage: false };
    }
  }, []);

  const fetchFaculties = useCallback(
    async (universityId: number, pageNumber: number) => {
      try {
        const res = await getPaginated<Faculty>(
          `/faculties/by-university/${universityId}`,
          pageNumber,
          PAGE_SIZE
        );
        return { items: res.items, hasNextPage: res.hasNextPage };
      } catch (error) {
        console.error("Failed to load faculties", error);
        toast.error("Failed to load faculties");
        return { items: [], hasNextPage: false };
      }
    },
    []
  );

  const fetchMajors = useCallback(
    async (facultyId: number, pageNumber: number) => {
      try {
        const res = await getPaginated<Major>(
          `/majors/by-faculty/${facultyId}`,
          pageNumber,
          PAGE_SIZE
        );
        return { items: res.items, hasNextPage: res.hasNextPage };
      } catch (error) {
        console.error("Failed to load majors", error);
        toast.error("Failed to load majors");
        return { items: [], hasNextPage: false };
      }
    },
    []
  );

  return {
    selectedUniversity,
    selectedFaculty,
    selectedMajor,
    setSelectedUniversity,
    setSelectedFaculty,
    setSelectedMajor,
    fetchUniversities,
    fetchFaculties,
    fetchMajors,
  };
};
