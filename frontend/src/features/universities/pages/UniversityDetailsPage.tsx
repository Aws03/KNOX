import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { SearchSection } from "../components/SearchSection";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import type { Faculty } from "../types";
import { useUniversityDetails } from "../hooks/useUniversityDetails";
import { UniversityHeader } from "../components/UniversityHeader";
import { FacultyTable } from "../components/FacultyTable";
import {
  AddFacultyDialog,
  EditFacultyDialog,
  // DeleteFacultyDialog,
} from "../components/FacultyDialogs";

const UniversityDetailsPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const universityId = parseInt(id || "0");

  // Dialog states
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  // const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingFaculty, setEditingFaculty] = useState<Faculty | null>(null);
  // const [deletingFaculty, setDeletingFaculty] = useState<Faculty | null>(null);

  // Use custom hook for data management
  const {
    university,
    faculties,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleClearSearch,
    handleAddFaculty,
    handleEditFaculty,
    // handleDeleteFaculty,
  } = useUniversityDetails({ universityId });

  // Navigation handlers
  const handleBack = () => {
    navigate("/dashboard/universities");
  };

  const handleFacultyClick = (facultyId: number) => {
    navigate(`/dashboard/universities/${universityId}/faculties/${facultyId}`);
  };

  // Dialog handlers
  const handleEditClick = (faculty: Faculty) => {
    setEditingFaculty(faculty);
    setIsEditDialogOpen(true);
  };

  // const handleDeleteClick = (faculty: Faculty) => {
  //   setDeletingFaculty(faculty);
  //   setIsDeleteDialogOpen(true);
  // };

  return (
    <div className="space-y-6">
      <UniversityHeader
        universityName={university?.name}
        onBack={handleBack}
        onAddFaculty={() => setIsAddDialogOpen(true)}
      />

      <SearchSection
        searchTerm={searchTerm}
        onSearchTermChange={setSearchTerm}
        onSearch={handleSearch}
        onClearSearch={handleClearSearch}
      />

      <FacultyTable
        faculties={faculties}
        loading={loading}
        searchTerm={searchTerm}
        universityName={university?.name}
        onFacultyClick={handleFacultyClick}
        onEditClick={handleEditClick}
        // onDeleteClick={handleDeleteClick}
        onAddClick={() => setIsAddDialogOpen(true)}
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

      <AddFacultyDialog
        isOpen={isAddDialogOpen}
        onClose={() => setIsAddDialogOpen(false)}
        onAdd={handleAddFaculty}
        universityName={university?.name}
      />

      <EditFacultyDialog
        isOpen={isEditDialogOpen}
        onClose={() => {
          setIsEditDialogOpen(false);
          setEditingFaculty(null);
        }}
        onEdit={handleEditFaculty}
        faculty={editingFaculty}
      />

      {/* <DeleteFacultyDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => {
          setIsDeleteDialogOpen(false);
          setDeletingFaculty(null);
        }}
        onDelete={handleDeleteFaculty}
        faculty={deletingFaculty}
      /> */}
    </div>
  );
};

export default UniversityDetailsPage;
