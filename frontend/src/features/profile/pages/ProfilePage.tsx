import { UserProfile } from "@/features/profile/components/UserProfile";

const ProfilePage = () => {
  return (
    <div className="min-h-[calc(100vh-4rem)] bg-linear-to-b from-slate-50 to-white dark:from-slate-950 dark:to-slate-900">
      <div className="container mx-auto py-8 lg:py-12">
        <UserProfile />
      </div>
    </div>
  );
};

export default ProfilePage;
