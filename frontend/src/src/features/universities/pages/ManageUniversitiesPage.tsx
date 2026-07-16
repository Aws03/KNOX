import { useState } from "react";
import { useNavigate } from "react-router-dom";
import SmartPagination from "@/shared/components/pagination/SmartPagination";
import { CRUDPageLayout } from "@/shared/components/crud";
import type { University } from "../types";
import { useUniversities } from "../hooks/useUniversities";
import { UniversityTable } from "../components/UniversityTable";
import {
  AddUniversityDialog,
  EditUniversityDialog,
  // DeleteUniversityDialog,
} from "../components/UniversityDialogs";

const ManageUniversitiesPage = () => {
  const navigate = useNavigate();

  // Dialog states
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  // const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingUniversity, setEditingUniversity] = useState<University | null>(
    null
  );
  // const [deletingUniversity, setDeletingUniversity] =
  //   useState<University | null>(null);

  // Use custom hook for data management
  const {
    universities,
    loading,
    searchTerm,
    setSearchTerm,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleAddUniversity,
    handleEditUniversity,
    // handleDeleteUniversity,
  } = useUniversities();

  // Navigation handlers
  const handleManageClick = (universityId: number) => {
    navigate(`/dashboard/universities/${universityId}`);
  };

  // Dialog handlers
  const handleEditClick = (university: University) => {
    setEditingUniversity(university);
    setIsEditDialogOpen(true);
  };

  // const handleDeleteClick = (university: University) => {
  //   setDeletingUniversity(university);
  //   setIsDeleteDialogOpen(true);
  // };

  return (
    <>
      <CRUDPageLayout
        title="Universities"
        description="Manage platform universities"
        onAdd={() => setIsAddDialogOpen(true)}
        addButtonLabel="Add University"
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        searchPlaceholder="Search by name or ID..."
      >
        <UniversityTable
          universities={universities}
          loading={loading}
          searchTerm={searchTerm}
          onManageClick={handleManageClick}
          onEditClick={handleEditClick}
          // onDeleteClick={handleDeleteClick}
        />

        {totalPages > 1 && (
          <div className="mt-4">
            <SmartPagination
              pageNumber={currentPage}
              totalPages={totalPages}
              hasPreviousPage={paginationInfo.hasPreviousPage}
              hasNextPage={paginationInfo.hasNextPage}
              onPageChange={setCurrentPage}
            />
          </div>
        )}
      </CRUDPageLayout>

      <AddUniversityDialog
        isOpen={isAddDialogOpen}
        onClose={() => setIsAddDialogOpen(false)}
        onAdd={handleAddUniversity}
      />

      <EditUniversityDialog
        isOpen={isEditDialogOpen}
        onClose={() => {
          setIsEditDialogOpen(false);
          setEditingUniversity(null);
        }}
        onEdit={handleEditUniversity}
        university={editingUniversity}
      />

      {/* <DeleteUniversityDialog
        isOpen={isDeleteDialogOpen}
        onClose={() => {
          setIsDeleteDialogOpen(false);
          setDeletingUniversity(null);
        }}
        onDelete={handleDeleteUniversity}
        university={deletingUniversity}
      /> */}
    </>
  );
};

export default ManageUniversitiesPage;
