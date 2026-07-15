import SmartPagination from "@/shared/components/pagination/SmartPagination";
import { PageHeader } from "../components/PageHeader";
import { UsersSearchSection } from "../components/UsersSearchSection";
import { UsersTable } from "../components/UsersTable";
import {
  AssignRoleDialog,
  ToggleStatusDialog,
} from "../components/UserDialogs";
import { useManageUsers } from "../hooks/useManageUsers";
import { useUserActions } from "../hooks/useUserActions";

const ManageUsersPage = () => {
  const {
    users,
    loading,
    searchTerm,
    setSearchTerm,
    searchType,
    setSearchType,
    currentPage,
    setCurrentPage,
    totalPages,
    paginationInfo,
    handleSearch,
    handleApplyFilters,
    handleResetFilters,
    refreshUsers,
  } = useManageUsers();

  const {
    roles,
    isAssignRoleDialogOpen,
    isBlockDialogOpen,
    selectedUser,
    selectedRole,
    setSelectedRole,
    openAssignRoleDialog,
    closeAssignRoleDialog,
    openBlockDialog,
    closeBlockDialog,
    handleAssignRole,
    handleToggleUserStatus,
  } = useUserActions(refreshUsers);

  return (
    <div className="space-y-6">
      <PageHeader
        title="Manage Users"
        description="View and manage platform users"
      />

      <UsersSearchSection
        searchTerm={searchTerm}
        searchType={searchType}
        onSearchTermChange={setSearchTerm}
        onSearchTypeChange={setSearchType}
        onSearch={handleSearch}
        onApplyFilters={handleApplyFilters}
        onResetFilters={handleResetFilters}
      />

      <UsersTable
        users={users}
        loading={loading}
        onAssignRole={openAssignRoleDialog}
        onToggleStatus={openBlockDialog}
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

      <AssignRoleDialog
        isOpen={isAssignRoleDialogOpen}
        onClose={closeAssignRoleDialog}
        onAssign={handleAssignRole}
        user={selectedUser}
        roles={roles}
        selectedRole={selectedRole}
        onRoleChange={setSelectedRole}
      />

      <ToggleStatusDialog
        isOpen={isBlockDialogOpen}
        onClose={closeBlockDialog}
        onConfirm={() => selectedUser && handleToggleUserStatus(selectedUser)}
        user={selectedUser}
      />
    </div>
  );
};

export default ManageUsersPage;
