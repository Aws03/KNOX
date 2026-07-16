import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogClose,
} from "@/shared/ui/dialog";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import { apiClient } from "@/lib/api/apiClient";
import { fetchCourseByCode } from "@/features/courses/api";
import { toast } from "sonner";
import { useParams, useNavigate } from "react-router-dom";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import { MajorCoursesHeader } from "@/features/courses/components/MajorCoursesHeader";
import { CourseFilters } from "@/features/courses/components/CourseFilters";
import { CourseStatsCards } from "@/features/courses/components/CourseStatsCards";
import { CoursesTable } from "@/features/courses/components/CoursesTable";
import { useCourseFilters } from "@/features/courses/hooks/useCourseFilters";
import { useBreadcrumbData } from "@/features/courses/hooks/useBreadcrumbData";
import { useCourses } from "@/features/courses/hooks/useCourses";

const MajorCoursesPage = () => {
  const { universityId, facultyId, majorId } = useParams<{
    universityId: string;
    facultyId: string;
    majorId: string;
  }>();
  const navigate = useNavigate();

  const majorIdNum = parseInt(majorId || "0");

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // Add Course Dialog State
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [addLoading, setAddLoading] = useState(false);
  const [addError, setAddError] = useState("");
  const [step, setStep] = useState(1); // 1: enter code, 2: form
  const [codeInput, setCodeInput] = useState("");
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const [existingCourse, setExistingCourse] = useState(null as null | any);
  const [form, setForm] = useState({
    courseName: "",
    courseCode: "",
    description: "",
    requirementType: "1",
    requirementNature: "1",
    credits: 0,
  });

  // Breadcrumb data
  const { university, faculty, major } = useBreadcrumbData(
    universityId,
    facultyId,
    majorId
  );

  // Filter hook
  const {
    showFilters,
    setShowFilters,
    localRequirementType,
    setLocalRequirementType,
    localRequirementNature,
    setLocalRequirementNature,
    requirementType,
    requirementNature,
    hasActiveFilters,
    hasUnappliedChanges,
    handleApplyFilters: applyFilters,
    handleClearFilters: clearAllFilters,
    handleClearRequirementTypeFilter,
    handleClearRequirementNatureFilter,
  } = useCourseFilters();

  // Courses data
  const {
    courses,
    loading,
    totalCount,
    totalPages,
    hasPreviousPage,
    hasNextPage,
    refetch: refetchCourses,
  } = useCourses(
    majorIdNum,
    currentPage,
    pageSize,
    requirementType,
    requirementNature
  );

  // Add Course Handler
  // Step 1: Check course code
  const handleCheckCode = async (e: React.FormEvent) => {
    e.preventDefault();
    setAddLoading(true);
    setAddError("");
    setExistingCourse(null);
    try {
      const course = await fetchCourseByCode(codeInput.trim().toUpperCase());
      setExistingCourse(course);
      setForm({
        courseName: course.courseName,
        courseCode: course.courseCode,
        description: course.description,
        credits: course.credits,
        requirementType: "1",
        requirementNature: "1",
      });
      setStep(2);
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (err: any) {
      if (err?.response?.status === 404) {
        setExistingCourse(null);
        setForm({
          courseName: "",
          courseCode: codeInput.trim().toUpperCase(),
          description: "",
          credits: 0,
          requirementType: "1",
          requirementNature: "1",
        });
        setStep(2);
      } else {
        setAddError("Failed to check course code");
      }
    } finally {
      setAddLoading(false);
    }
  };

  // Step 2: Add course (existing or new)
  const handleAddCourse = async (e: React.FormEvent) => {
    e.preventDefault();
    setAddLoading(true);
    setAddError("");
    try {
      await apiClient.post("/courses", {
        majorId: majorIdNum,
        ...form,
        credits: Number(form.credits),
        requirementType: Number(form.requirementType),
        requirementNature: Number(form.requirementNature),
      });
      setAddDialogOpen(false);
      setForm({
        courseName: "",
        courseCode: "",
        description: "",
        requirementType: "1",
        requirementNature: "1",
        credits: 0,
      });
      setStep(1);
      setCodeInput("");
      setExistingCourse(null);
      toast.success("Course added successfully!");
      setCurrentPage(1);
      // Refetch courses after adding
      refetchCourses();
    } catch (err: unknown) {
      const errorMsg =
        err && typeof err === "object" && "response" in err
          ? (err as unknown as { response?: { data?: { message?: string } } })
              .response?.data?.message
          : "Failed to add course";
      setAddError(
        typeof errorMsg === "string" ? errorMsg : "Failed to add course"
      );
    } finally {
      setAddLoading(false);
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handlePageSizeChange = (newSize: number) => {
    setPageSize(newSize);
    setCurrentPage(1);
  };

  const handleApplyFilters = () => {
    applyFilters();
    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    clearAllFilters();
    setCurrentPage(1);
  };

  const totalMaterials = courses.reduce(
    (acc, c) => acc + c.numberOfMaterials,
    0
  );
  const totalQuizzes = courses.reduce((acc, c) => acc + c.numberOfQuizzes, 0);

  return (
    <div className="container mx-auto space-y-6 p-6">
      <MajorCoursesHeader
        university={university}
        faculty={faculty}
        major={major}
        onBackClick={() =>
          navigate(
            `/dashboard/universities/${universityId}/faculties/${facultyId}`
          )
        }
      />
      <div className="flex justify-end mb-2">
        <Button onClick={() => setAddDialogOpen(true)}>+ Add Course</Button>
      </div>

      {/* Add Course Dialog */}
      <Dialog
        open={addDialogOpen}
        onOpenChange={(open) => {
          setAddDialogOpen(open);
          if (!open) {
            setStep(1);
            setCodeInput("");
            setExistingCourse(null);
            setAddError("");
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Add New Course</DialogTitle>
          </DialogHeader>
          {step === 1 && (
            <form onSubmit={handleCheckCode} className="space-y-4">
              <Input
                required
                placeholder="Enter Course Code"
                value={codeInput}
                onChange={(e) => setCodeInput(e.target.value)}
                autoFocus
              />
              {addError && (
                <div className="text-red-500 text-sm">{addError}</div>
              )}
              <DialogFooter>
                <Button
                  type="submit"
                  disabled={addLoading || !codeInput.trim()}
                >
                  {addLoading ? "Checking..." : "Next"}
                </Button>
                <DialogClose asChild>
                  <Button type="button" variant="outline">
                    Cancel
                  </Button>
                </DialogClose>
              </DialogFooter>
            </form>
          )}
          {step === 2 && (
            <form onSubmit={handleAddCourse} className="space-y-4">
              {existingCourse ? (
                <>
                  <div className="space-y-2">
                    <div>
                      <label className="block text-xs font-medium">
                        Course Name
                      </label>
                      <Input value={form.courseName} readOnly disabled />
                    </div>
                    <div>
                      <label className="block text-xs font-medium">
                        Course Code
                      </label>
                      <Input value={form.courseCode} readOnly disabled />
                    </div>
                    <div>
                      <label className="block text-xs font-medium">
                        Description
                      </label>
                      <Input value={form.description} readOnly disabled />
                    </div>
                    <div>
                      <label className="block text-xs font-medium">
                        Credits
                      </label>
                      <Input value={form.credits} readOnly disabled />
                    </div>
                  </div>
                </>
              ) : (
                <>
                  <Input
                    required
                    placeholder="Course Name"
                    value={form.courseName}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, courseName: e.target.value }))
                    }
                  />
                  <Input
                    required
                    placeholder="Course Code"
                    value={form.courseCode}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, courseCode: e.target.value }))
                    }
                  />
                  <Input
                    placeholder="Description"
                    value={form.description}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, description: e.target.value }))
                    }
                  />
                  <div className="flex-1">
                    <label className="block text-sm mb-1">Credits</label>
                    <Input
                      type="number"
                      min={0}
                      required
                      value={form.credits}
                      onChange={(e) =>
                        setForm((f) => ({
                          ...f,
                          credits: Number(e.target.value),
                        }))
                      }
                    />
                  </div>
                </>
              )}
              <div className="flex gap-2 mt-2">
                <div className="flex-1">
                  <label className="block text-sm mb-1">Requirement Type</label>
                  <select
                    className="w-full border rounded p-2"
                    value={form.requirementType}
                    aria-label="Requirement Type"
                    onChange={(e) =>
                      setForm((f) => ({
                        ...f,
                        requirementType: e.target.value,
                      }))
                    }
                  >
                    <option value="1">University</option>
                    <option value="2">Faculty</option>
                    <option value="3">Major</option>
                    <option value="4">Remedial</option>
                  </select>
                </div>
                <div className="flex-1">
                  <label className="block text-sm mb-1">
                    Requirement Nature
                  </label>
                  <select
                    className="w-full border rounded p-2"
                    value={form.requirementNature}
                    aria-label="Requirement Nature"
                    onChange={(e) =>
                      setForm((f) => ({
                        ...f,
                        requirementNature: e.target.value,
                      }))
                    }
                  >
                    <option value="1">Compulsory</option>
                    <option value="2">Elective</option>
                  </select>
                </div>
              </div>
              {addError && (
                <div className="text-red-500 text-sm">{addError}</div>
              )}
              <DialogFooter>
                <Button type="submit" disabled={addLoading}>
                  {addLoading ? "Adding..." : "Add Course"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setStep(1);
                    setAddError("");
                  }}
                >
                  Back
                </Button>
                <DialogClose asChild>
                  <Button type="button" variant="outline">
                    Cancel
                  </Button>
                </DialogClose>
              </DialogFooter>
            </form>
          )}
        </DialogContent>
      </Dialog>

      <CourseFilters
        showFilters={showFilters}
        onToggleFilters={() => setShowFilters(!showFilters)}
        localRequirementType={localRequirementType}
        localRequirementNature={localRequirementNature}
        onRequirementTypeChange={setLocalRequirementType}
        onRequirementNatureChange={setLocalRequirementNature}
        onClearRequirementType={handleClearRequirementTypeFilter}
        onClearRequirementNature={handleClearRequirementNatureFilter}
        onApplyFilters={handleApplyFilters}
        onClearAllFilters={handleClearFilters}
        hasActiveFilters={hasActiveFilters}
        hasUnappliedChanges={hasUnappliedChanges}
      />

      <CourseStatsCards
        totalCount={totalCount}
        totalMaterials={totalMaterials}
        totalQuizzes={totalQuizzes}
        currentPage={currentPage}
        totalPages={totalPages}
      />

      <CoursesTable
        courses={courses}
        loading={loading}
        totalCount={totalCount}
        currentPage={currentPage}
        pageSize={pageSize}
        onPageSizeChange={handlePageSizeChange}
        hasActiveFilters={hasActiveFilters}
      />

      {/* Pagination */}
      {!loading && totalPages > 1 && (
        <div className="flex justify-center">
          <SmartPagination
            pageNumber={currentPage}
            totalPages={totalPages}
            hasPreviousPage={hasPreviousPage}
            hasNextPage={hasNextPage}
            onPageChange={handlePageChange}
          />
        </div>
      )}
    </div>
  );
};

export default MajorCoursesPage;
