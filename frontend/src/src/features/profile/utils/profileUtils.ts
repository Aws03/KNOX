export const getInitials = (name: string): string => {
  return name
    .split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase()
    .slice(0, 2);
};

export const getRoleBadgeVariant = (role: string): string => {
  const roleUpper = role.toUpperCase();
  if (roleUpper === "ADMIN" || roleUpper === "SUPERADMIN") {
    return "bg-red-500/10 text-red-700 dark:text-red-400 border-red-200 dark:border-red-800";
  }
  if (roleUpper === "WRITER") {
    return "bg-purple-500/10 text-purple-700 dark:text-purple-400 border-purple-200 dark:border-purple-800";
  }
  return "bg-gray-500/10 text-gray-700 dark:text-gray-400 border-gray-200 dark:border-gray-800";
};

export const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};
