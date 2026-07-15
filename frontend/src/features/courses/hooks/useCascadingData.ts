import { useState, useCallback } from "react";
import {
  fetchUniversities,
  fetchFacultiesByUniversity,
  fetchMajorsByFaculty,
} from "../api";
import type { University, Faculty, Major, PaginatedState } from "../types";
import { createInitialPaginatedState } from "../types";

const PAGE_SIZE = 50;

export const useCascadingData = () => {
  const [universitiesState, setUniversitiesState] = useState<
    PaginatedState<University>
  >(createInitialPaginatedState<University>());
  const [facultiesState, setFacultiesState] = useState<PaginatedState<Faculty>>(
    createInitialPaginatedState<Faculty>()
  );
  const [majorsState, setMajorsState] = useState<PaginatedState<Major>>(
    createInitialPaginatedState<Major>()
  );

  const fetchUniversitiesData = useCallback(
    async (page = 1, append = false) => {
      setUniversitiesState((prev) => ({ ...prev, loading: true }));
      try {
        const res = await fetchUniversities(page, PAGE_SIZE);
        setUniversitiesState((prev) => {
          if (append) {
            // Filter out duplicates by checking existing IDs
            const existingIds = new Set(prev.items.map((u) => u.id));
            const newItems = res.items.filter((u) => !existingIds.has(u.id));
            return {
              items: [...prev.items, ...newItems],
              pageNumber: res.pageNumber,
              hasNextPage: res.hasNextPage,
              loading: false,
              parentId: null,
            };
          }
          return {
            items: res.items,
            pageNumber: res.pageNumber,
            hasNextPage: res.hasNextPage,
            loading: false,
            parentId: null,
          };
        });
      } catch (error) {
        console.error("Failed to load universities", error);
        setUniversitiesState((prev) => ({ ...prev, loading: false }));
      }
    },
    []
  );

  const fetchFacultiesData = useCallback(
    async (universityId: number, page = 1, append = false) => {
      if (!universityId) return;
      setFacultiesState((prev) => ({
        ...prev,
        loading: true,
        parentId: universityId,
      }));
      try {
        const res = await fetchFacultiesByUniversity(
          universityId,
          page,
          PAGE_SIZE
        );
        setFacultiesState((prev) => {
          if (prev.parentId !== universityId) return prev;
          if (append) {
            // Filter out duplicates by checking existing IDs
            const existingIds = new Set(prev.items.map((f) => f.id));
            const newItems = res.items.filter((f) => !existingIds.has(f.id));
            return {
              items: [...prev.items, ...newItems],
              pageNumber: res.pageNumber,
              hasNextPage: res.hasNextPage,
              loading: false,
              parentId: universityId,
            };
          }
          return {
            items: res.items,
            pageNumber: res.pageNumber,
            hasNextPage: res.hasNextPage,
            loading: false,
            parentId: universityId,
          };
        });
      } catch (error) {
        console.error("Failed to load faculties", error);
        setFacultiesState((prev) => ({ ...prev, loading: false }));
      }
    },
    []
  );

  const fetchMajorsData = useCallback(
    async (facultyId: number, page = 1, append = false) => {
      if (!facultyId) return;
      setMajorsState((prev) => ({
        ...prev,
        loading: true,
        parentId: facultyId,
      }));
      try {
        const res = await fetchMajorsByFaculty(facultyId, page, PAGE_SIZE);
        setMajorsState((prev) => {
          if (prev.parentId !== facultyId) return prev;
          if (append) {
            // Filter out duplicates by checking existing IDs
            const existingIds = new Set(prev.items.map((m) => m.id));
            const newItems = res.items.filter((m) => !existingIds.has(m.id));
            return {
              items: [...prev.items, ...newItems],
              pageNumber: res.pageNumber,
              hasNextPage: res.hasNextPage,
              loading: false,
              parentId: facultyId,
            };
          }
          return {
            items: res.items,
            pageNumber: res.pageNumber,
            hasNextPage: res.hasNextPage,
            loading: false,
            parentId: facultyId,
          };
        });
      } catch (error) {
        console.error("Failed to load majors", error);
        setMajorsState((prev) => ({ ...prev, loading: false }));
      }
    },
    []
  );

  const resetFaculties = () =>
    setFacultiesState(createInitialPaginatedState<Faculty>());
  const resetMajors = () =>
    setMajorsState(createInitialPaginatedState<Major>());

  const addUniversity = (uni: University) => {
    setUniversitiesState((prev) => {
      // Check if university already exists in the list
      const exists = prev.items.some((u) => u.id === uni.id);
      if (exists) return prev;
      return { ...prev, items: [uni, ...prev.items] };
    });
  };

  const ensureUniversityExists = (uni: University) => {
    setUniversitiesState((prev) => {
      const exists = prev.items.some((u) => u.id === uni.id);
      if (exists) return prev;
      return { ...prev, items: [...prev.items, uni] };
    });
  };

  const ensureFacultyExists = (faculty: Faculty) => {
    setFacultiesState((prev) => {
      const exists = prev.items.some((f) => f.id === faculty.id);
      if (exists) return prev;
      return { ...prev, items: [...prev.items, faculty] };
    });
  };

  const ensureMajorExists = (major: Major) => {
    setMajorsState((prev) => {
      const exists = prev.items.some((m) => m.id === major.id);
      if (exists) return prev;
      return { ...prev, items: [...prev.items, major] };
    });
  };

  const addFaculty = (faculty: Faculty, universityId: number) => {
    setFacultiesState((prev) => {
      if (
        prev.parentId !== universityId ||
        prev.items.some((f) => f.id === faculty.id)
      )
        return prev;
      return { ...prev, items: [faculty, ...prev.items] };
    });
  };

  const addMajor = (major: Major, facultyId: number) => {
    setMajorsState((prev) => {
      if (
        prev.parentId !== facultyId ||
        prev.items.some((m) => m.id === major.id)
      )
        return prev;
      return { ...prev, items: [major, ...prev.items] };
    });
  };

  return {
    universitiesState,
    facultiesState,
    majorsState,
    fetchUniversitiesData,
    fetchFacultiesData,
    fetchMajorsData,
    resetFaculties,
    resetMajors,
    addUniversity,
    addFaculty,
    addMajor,
    ensureUniversityExists,
    ensureFacultyExists,
    ensureMajorExists,
  };
};
