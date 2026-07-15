export type FolderItem = {
  id: number;
  name: string;
  courseId: number;
  description: string | null;
};

export type MaterialItem = {
  id: number;
  title: string;
  contentUrl: string;
  courseId: number;
  description: string | null;
};

export type FolderContentsResponse = {
  courseId: number;
  folders: FolderItem[];
  materials: MaterialItem[];
};

export type CourseMaterialsResponse = {
  courseId: number;
  folders: FolderItem[];
  materials: MaterialItem[];
};
