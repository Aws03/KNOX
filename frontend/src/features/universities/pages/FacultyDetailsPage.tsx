import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useUserRole } from "@/hooks/useUserRole";
import { Button } from "@/shared/ui/button";
import { PlusIcon, ArrowLeftIcon } from "lucide-react";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import type { Major } from "../types";
import { useFacultyDetails } from "../hooks/useFacultyDetails";
import { SearchSection } from "../components/SearchSection";
import { MajorTable } from "../components/MajorTable";
import {
  AddMajorDialog,
  EditMajorDialog,
  // DeleteMajorDialog,
} from "../components/MajorDialogs";

const FacultyDetailsPage = () => {
  const { universityId, facultyId } = useParams<{
    universityId: string;
    facultyId: string;
  }>();
  const navigate = useNavigate();
  const { isSuperAdmin } = useUserRole();
  const universityIdNum = parseInt(universityId || "0");
  const facultyIdNum = parseInt(facultyId || "0");

  // Dialog states
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  // const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingMajor, setEditingMajor] = useState<Major | null>(null);
  // const [deletingMajor, setDeletingMajor] = useState<Major | null>(null);

  // Use custom hook for data management
  const {
    faculty,
    majors,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleClearSearch,
    handleAddMajor,
    handleEditMajor,
    // handleDeleteMajor,
  } = useFacultyDetails({ facultyId: facultyIdNum });

  // Navigation handlers
  const handleBack = () => {
    navigate(`/dashboard/universities/${universityIdNum}`);
  };

  const handleMajorClick = (majorId: number) => {
    navigate(
      `/dashboard/universities/${universityId}/faculties/${facultyId}/majors/${majorId}`
    );
  };

  // Dialog handlers
  const handleEditClick = (major: Major) => {
    setEditingMajor(major);
    setIsEditDialogOpen(true);
  };

  // const handleDeleteClick = (major: Major) => {
  //   setDeletingMajor(major);
  //   setIsDeleteDialogOpen(true);
  // };

  return (
    <div className="space-y-6">
      {/* Back Button */}
      <div>
        <Button onClick={handleBack} variant="ghost" size="sm">
          <ArrowLeftIcon className="mr-2 size-4" />
          Back to University
        </Button>
      </div>

      {isSuperAdmin && (
        <div className="flex justify-end">
          <Button onClick={() => setIsAddDialogOpen(true)} size="lg">
            <PlusIcon className="mr-2 size-4" />
            Add Major
          </Button>
        </div>
      )}

      <SearchSection
        searchTerm={searchTerm}
        onSearchTermChange={setSearchTerm}
        onSearch={handleSearch}
        onClearSearch={handleClearSearch}
      />

      <MajorTable
        majors={majors}
        loading={loading}
        searchTerm={searchTerm}
        facultyName={faculty?.name}
        onMajorClick={handleMajorClick}
        onEditClick={handleEditClick}
        onDeleteClick={undefined} // Disable delete functionality
        onAddClick={() => setIsAddDialogOpen(true)}
        showActions={isSuperAdmin}
      />

      {totalPages > 1 && (
        <SmartPagination
          pageNumber={currentPage}
          totalPages={totalPages}
          hasPreviousPage={paginationInfo.hasPreviousPage}
          hasNextPage={paginationInfo.hasNextPage}
          onPageChange={setCurrentPage}
        />
      )}

      <AddMajorDialog
        isOpen={isAddDialogOpen}
        onClose={() => setIsAddDialogOpen(false)}
        onAdd={handleAddMajor}
        facultyName={faculty?.name}
      />

      <EditMajorDialog
        isOpen={isEditDialogOpen}
        onClose={() => {
          setIsEditDialogOpen(false);
          setEditingMajor(null);
        }}
        onEdit={handleEditMajor}
        major={editingMajor}
      />

      {/* Delete functionality disabled for now */}
      {/* <DeleteMajorDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => {
          setIsDeleteDialogOpen(false);
          setDeletingMajor(null);
        }}
        onDelete={handleDeleteMajor}
        major={deletingMajor}
      /> */}
    </div>
  );
};

export default FacultyDetailsPage;
