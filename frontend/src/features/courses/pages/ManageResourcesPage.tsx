import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  Plus,
  Loader2,
  BookOpen,
  Pencil,
  Trash2,
  ExternalLink,
  AlertCircle,
} from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/shared/ui/card";
import { Badge } from "@/shared/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import { Input } from "@/shared/ui/input";
import { Label } from "@/shared/ui/label";
import { Textarea } from "@/shared/ui/textarea";
import { Checkbox } from "@/shared/ui/checkbox";
import { fetchCourseInfo, fetchCourseById } from "@/features/courses/api";
import { apiClient } from "@/lib/api/apiClient";
import type {
  CourseInfo,
  CourseResource,
  ResourceType,
  CourseApiResponse,
  DifficultyLevel,
} from "@/features/courses/types";

// Resource type options
const RESOURCE_TYPE_OPTIONS: { value: ResourceType; label: string }[] = [
  { value: "ECampusCourse", label: "ECampus Course" },
  { value: "YouTubeVideo", label: "YouTube Video" },
  { value: "YouTubePlaylist", label: "YouTube Playlist" },
  { value: "Article", label: "Article" },
  { value: "BlogPost", label: "Blog Post" },
  { value: "UdemyCourse", label: "Udemy Course" },
  { value: "CourseraCourse", label: "Coursera Course" },
  { value: "EdXCourse", label: "edX Course" },
  { value: "LinkedInLearning", label: "LinkedIn Learning" },
  { value: "PluralSight", label: "Pluralsight" },
  { value: "OtherPlatformCourse", label: "Other Platform Course" },
  { value: "Other", label: "Other" },
];

// Difficulty level options
const DIFFICULTY_OPTIONS: { value: DifficultyLevel; label: string }[] = [
  { value: "Easy", label: "Easy" },
  { value: "Moderate", label: "Moderate" },
  { value: "Hard", label: "Hard" },
];

// Form state interface
interface ResourceFormData {
  title: string;
  type: ResourceType;
  url: string;
  description: string;
  hasDemonstrationVideo: boolean;
  demonstrationVideoUrl: string;
}

interface CourseInfoFormData {
  difficultyLevel: DifficultyLevel;
  description: string;
  demonstrationVideoUrl: string;
  demonstrationVideoTitle: string;
}

const ManageResourcesPage = () => {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [courseInfo, setCourseInfo] = useState<CourseInfo | null>(null);
  const [course, setCourse] = useState<CourseApiResponse | null>(null);
  const [resources, setResources] = useState<CourseResource[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [hasCourseInfo, setHasCourseInfo] = useState(false);

  // Dialog states
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isCourseInfoDialogOpen, setIsCourseInfoDialogOpen] = useState(false);
  const [selectedResource, setSelectedResource] =
    useState<CourseResource | null>(null);
  const [formData, setFormData] = useState<ResourceFormData>({
    title: "",
    type: "Other",
    url: "",
    description: "",
    hasDemonstrationVideo: false,
    demonstrationVideoUrl: "",
  });
  const [courseInfoFormData, setCourseInfoFormData] =
    useState<CourseInfoFormData>({
      difficultyLevel: "Moderate",
      description: "",
      demonstrationVideoUrl: "",
      demonstrationVideoTitle: "",
    });
  const [submitting, setSubmitting] = useState(false);

  // Load course info and resources
  useEffect(() => {
    const loadData = async () => {
      if (!courseId) return;

      setLoading(true);
      setError(null);
      try {
        const courseResponse = await fetchCourseById(parseInt(courseId));
        setCourse(courseResponse);

        // Try to fetch course info - if it fails, course info doesn't exist
        try {
          const infoResponse = await fetchCourseInfo(parseInt(courseId));
          setCourseInfo(infoResponse);
          setResources(infoResponse.resources || []);
          setHasCourseInfo(true);
          // Pre-populate course info form for editing
          setCourseInfoFormData({
            difficultyLevel: infoResponse.difficultyLevel,
            description: infoResponse.description,
            demonstrationVideoUrl: infoResponse.demonstrationVideoUrl || "",
            demonstrationVideoTitle: infoResponse.demonstrationVideoTitle || "",
          });
        } catch {
          // Course info doesn't exist yet (404 or other error)
          setHasCourseInfo(false);
          setCourseInfo(null);
          setResources([]);
        }
      } catch (err) {
        console.error("Failed to fetch course data:", err);
        setError("Failed to load course data");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [courseId]);

  // Reset form
  const resetForm = () => {
    setFormData({
      title: "",
      type: "Other",
      url: "",
      description: "",
      hasDemonstrationVideo: false,
      demonstrationVideoUrl: "",
    });
  };

  const resetCourseInfoForm = () => {
    setCourseInfoFormData({
      difficultyLevel: "Moderate",
      description: "",
      demonstrationVideoUrl: "",
      demonstrationVideoTitle: "",
    });
  };

  // Handle create/update course info
  const handleSaveCourseInfo = async () => {
    if (!courseId) return;

    setSubmitting(true);
    try {
      if (hasCourseInfo && courseInfo) {
        // Update existing course info
        await apiClient.put(`/courses/${courseId}/info`, courseInfoFormData);
      } else {
        // Create new course info
        await apiClient.post(`/courses/${courseId}/info`, courseInfoFormData);
      }

      setIsCourseInfoDialogOpen(false);
      // Reload data
      const courseResponse = await fetchCourseById(parseInt(courseId));
      setCourse(courseResponse);

      try {
        const infoResponse = await fetchCourseInfo(parseInt(courseId));
        setCourseInfo(infoResponse);
        setResources(infoResponse.resources || []);
        setHasCourseInfo(true);
      } catch {
        setHasCourseInfo(false);
        setCourseInfo(null);
        setResources([]);
      }
    } catch (err) {
      console.error("Failed to save course info:", err);
      setError("Failed to save course information");
    } finally {
      setSubmitting(false);
    }
  };

  // Handle add resource
  const handleAddResource = async () => {
    if (!courseInfo || !courseId) return;

    setSubmitting(true);
    try {
      await apiClient.post(`/courses/${courseId}/info/resources`, formData);

      setIsAddDialogOpen(false);
      resetForm();
      // Reload data
      const infoResponse = await fetchCourseInfo(parseInt(courseId));
      setCourseInfo(infoResponse);
      setResources(infoResponse.resources || []);
    } catch (err) {
      console.error("Failed to add resource:", err);
      setError("Failed to add resource");
    } finally {
      setSubmitting(false);
    }
  };

  // Handle edit resource
  const handleEditResource = async () => {
    if (!selectedResource || !courseId) return;

    setSubmitting(true);
    try {
      await apiClient.put(
        `/courses/${courseId}/info/resources/${selectedResource.id}`,
        formData
      );

      setIsEditDialogOpen(false);
      setSelectedResource(null);
      resetForm();
      // Reload data
      const infoResponse = await fetchCourseInfo(parseInt(courseId));
      setCourseInfo(infoResponse);
      setResources(infoResponse.resources || []);
    } catch (err) {
      console.error("Failed to update resource:", err);
      setError("Failed to update resource");
    } finally {
      setSubmitting(false);
    }
  };

  // Handle delete resource
  const handleDeleteResource = async () => {
    if (!selectedResource || !courseId) return;

    setSubmitting(true);
    try {
      await apiClient.delete(
        `/courses/${courseId}/info/resources/${selectedResource.id}`
      );

      setIsDeleteDialogOpen(false);
      setSelectedResource(null);
      // Reload data
      const infoResponse = await fetchCourseInfo(parseInt(courseId));
      setCourseInfo(infoResponse);
      setResources(infoResponse.resources || []);
    } catch (err) {
      console.error("Failed to delete resource:", err);
      setError("Failed to delete resource");
    } finally {
      setSubmitting(false);
    }
  };

  // Open edit dialog
  const openEditDialog = (resource: CourseResource) => {
    setSelectedResource(resource);
    setFormData({
      title: resource.title,
      type: resource.type,
      url: resource.url,
      description: resource.description,
      hasDemonstrationVideo: resource.hasDemonstrationVideo,
      demonstrationVideoUrl: resource.demonstrationVideoUrl || "",
    });
    setIsEditDialogOpen(true);
  };

  // Open delete dialog
  const openDeleteDialog = (resource: CourseResource) => {
    setSelectedResource(resource);
    setIsDeleteDialogOpen(true);
  };

  return (
    <div className="container mx-auto space-y-6 p-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <Button variant="ghost" className="mb-4" onClick={() => navigate(-1)}>
            <ArrowLeft className="mr-2 size-4" />
            Back
          </Button>
          <h1 className="text-3xl font-bold">Manage Resources</h1>
          <p className="text-muted-foreground mt-1">
            {course?.courseName} ({course?.courseCode})
          </p>
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            onClick={() => setIsCourseInfoDialogOpen(true)}
            size="lg"
          >
            <Pencil className="mr-2 size-4" />
            {hasCourseInfo ? "Edit" : "Add"} Course Info
          </Button>
          <Button
            onClick={() => setIsAddDialogOpen(true)}
            size="lg"
            disabled={!hasCourseInfo}
          >
            <Plus className="mr-2 size-4" />
            Add Resource
          </Button>
        </div>
      </div>

      {/* Error Display */}
      {error && (
        <Card className="border-destructive/50 bg-destructive/10">
          <CardContent className="flex items-center gap-2 p-4 text-destructive">
            <AlertCircle className="size-5" />
            <p>{error}</p>
          </CardContent>
        </Card>
      )}

      {/* Loading State */}
      {loading && (
        <div className="flex flex-col items-center justify-center py-16 gap-3">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-sm text-muted-foreground">Loading resources...</p>
        </div>
      )}

      {/* No Course Info State */}
      {!loading && !hasCourseInfo && (
        <Card className="p-12 border-2 border-dashed">
          <div className="flex flex-col items-center justify-center text-center">
            <div className="flex h-20 w-20 items-center justify-center rounded-full bg-orange-500/10 mb-4">
              <AlertCircle className="h-10 w-10 text-orange-500" />
            </div>
            <h3 className="text-lg font-semibold mb-2">
              No Course Information
            </h3>
            <p className="text-sm text-muted-foreground max-w-sm mb-4">
              Before adding resources, you must first create course information
              including difficulty level and description.
            </p>
            <Button onClick={() => setIsCourseInfoDialogOpen(true)} size="lg">
              <Plus className="mr-2 size-4" />
              Add Course Information
            </Button>
          </div>
        </Card>
      )}

      {/* Resources List */}
      {!loading && hasCourseInfo && (
        <div className="space-y-4">
          {resources.length === 0 ? (
            <Card className="p-12">
              <div className="flex flex-col items-center justify-center text-center">
                <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
                  <BookOpen className="h-10 w-10 text-primary" />
                </div>
                <h3 className="text-lg font-semibold mb-2">No resources yet</h3>
                <p className="text-sm text-muted-foreground max-w-sm mb-4">
                  Add learning resources to help students with this course.
                </p>
                <Button onClick={() => setIsAddDialogOpen(true)}>
                  <Plus className="mr-2 size-4" />
                  Add First Resource
                </Button>
              </div>
            </Card>
          ) : (
            <div className="grid gap-4">
              {resources.map((resource) => (
                <Card key={resource.id}>
                  <CardHeader>
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-2">
                          <CardTitle className="text-lg">
                            {resource.title}
                          </CardTitle>
                          <Badge variant="secondary">
                            {
                              RESOURCE_TYPE_OPTIONS.find(
                                (opt) => opt.value === resource.type
                              )?.label
                            }
                          </Badge>
                        </div>
                        {resource.description && (
                          <p className="text-sm text-muted-foreground">
                            {resource.description}
                          </p>
                        )}
                      </div>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={() => window.open(resource.url, "_blank")}
                        >
                          <ExternalLink className="size-4" />
                        </Button>
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={() => openEditDialog(resource)}
                        >
                          <Pencil className="size-4" />
                        </Button>
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={() => openDeleteDialog(resource)}
                        >
                          <Trash2 className="size-4" />
                        </Button>
                      </div>
                    </div>
                  </CardHeader>
                  {resource.hasDemonstrationVideo && (
                    <CardContent>
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <AlertCircle className="size-4" />
                        <span>Has demonstration video</span>
                      </div>
                    </CardContent>
                  )}
                </Card>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Add Resource Dialog */}
      <Dialog open={isAddDialogOpen} onOpenChange={setIsAddDialogOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Add Resource</DialogTitle>
            <DialogDescription>
              Add a new learning resource for this course
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="title">Title *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                placeholder="Resource title"
              />
            </div>
            <div>
              <Label htmlFor="type">Type *</Label>
              <Select
                value={formData.type}
                onValueChange={(value: ResourceType) =>
                  setFormData({ ...formData, type: value })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {RESOURCE_TYPE_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="url">URL *</Label>
              <Input
                id="url"
                value={formData.url}
                onChange={(e) =>
                  setFormData({ ...formData, url: e.target.value })
                }
                placeholder="https://..."
              />
            </div>
            <div>
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                value={formData.description}
                onChange={(e) =>
                  setFormData({ ...formData, description: e.target.value })
                }
                placeholder="Optional description"
                rows={3}
              />
            </div>
            <div className="flex items-center space-x-2">
              <Checkbox
                id="hasDemoVideo"
                checked={formData.hasDemonstrationVideo}
                onCheckedChange={(checked) =>
                  setFormData({
                    ...formData,
                    hasDemonstrationVideo: checked as boolean,
                  })
                }
              />
              <Label htmlFor="hasDemoVideo" className="cursor-pointer">
                Has demonstration video
              </Label>
            </div>
            {formData.hasDemonstrationVideo && (
              <div>
                <Label htmlFor="demoUrl">Demonstration Video URL</Label>
                <Input
                  id="demoUrl"
                  value={formData.demonstrationVideoUrl}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      demonstrationVideoUrl: e.target.value,
                    })
                  }
                  placeholder="YouTube video URL"
                />
              </div>
            )}
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setIsAddDialogOpen(false);
                resetForm();
              }}
              disabled={submitting}
            >
              Cancel
            </Button>
            <Button onClick={handleAddResource} disabled={submitting}>
              {submitting ? (
                <>
                  <Loader2 className="mr-2 size-4 animate-spin" />
                  Adding...
                </>
              ) : (
                "Add Resource"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Edit Resource Dialog */}
      <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Edit Resource</DialogTitle>
            <DialogDescription>
              Update the resource information
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="edit-title">Title *</Label>
              <Input
                id="edit-title"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                placeholder="Resource title"
              />
            </div>
            <div>
              <Label htmlFor="edit-type">Type *</Label>
              <Select
                value={formData.type}
                onValueChange={(value: ResourceType) =>
                  setFormData({ ...formData, type: value })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {RESOURCE_TYPE_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="edit-url">URL *</Label>
              <Input
                id="edit-url"
                value={formData.url}
                onChange={(e) =>
                  setFormData({ ...formData, url: e.target.value })
                }
                placeholder="https://..."
              />
            </div>
            <div>
              <Label htmlFor="edit-description">Description</Label>
              <Textarea
                id="edit-description"
                value={formData.description}
                onChange={(e) =>
                  setFormData({ ...formData, description: e.target.value })
                }
                placeholder="Optional description"
                rows={3}
              />
            </div>
            <div className="flex items-center space-x-2">
              <Checkbox
                id="edit-hasDemoVideo"
                checked={formData.hasDemonstrationVideo}
                onCheckedChange={(checked) =>
                  setFormData({
                    ...formData,
                    hasDemonstrationVideo: checked as boolean,
                  })
                }
              />
              <Label htmlFor="edit-hasDemoVideo" className="cursor-pointer">
                Has demonstration video
              </Label>
            </div>
            {formData.hasDemonstrationVideo && (
              <div>
                <Label htmlFor="edit-demoUrl">Demonstration Video URL</Label>
                <Input
                  id="edit-demoUrl"
                  value={formData.demonstrationVideoUrl}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      demonstrationVideoUrl: e.target.value,
                    })
                  }
                  placeholder="YouTube video URL"
                />
              </div>
            )}
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setIsEditDialogOpen(false);
                setSelectedResource(null);
                resetForm();
              }}
              disabled={submitting}
            >
              Cancel
            </Button>
            <Button onClick={handleEditResource} disabled={submitting}>
              {submitting ? (
                <>
                  <Loader2 className="mr-2 size-4 animate-spin" />
                  Updating...
                </>
              ) : (
                "Update Resource"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Resource Dialog */}
      <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Delete Resource</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete "{selectedResource?.title}"? This
              action cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setIsDeleteDialogOpen(false);
                setSelectedResource(null);
              }}
              disabled={submitting}
            >
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleDeleteResource}
              disabled={submitting}
            >
              {submitting ? (
                <>
                  <Loader2 className="mr-2 size-4 animate-spin" />
                  Deleting...
                </>
              ) : (
                "Delete"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Course Info Dialog */}
      <Dialog
        open={isCourseInfoDialogOpen}
        onOpenChange={setIsCourseInfoDialogOpen}
      >
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>
              {hasCourseInfo ? "Edit" : "Add"} Course Information
            </DialogTitle>
            <DialogDescription>
              {hasCourseInfo
                ? "Update the course information"
                : "Create course information before adding resources"}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="difficulty">Difficulty Level *</Label>
              <Select
                value={courseInfoFormData.difficultyLevel}
                onValueChange={(value: DifficultyLevel) =>
                  setCourseInfoFormData({
                    ...courseInfoFormData,
                    difficultyLevel: value,
                  })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {DIFFICULTY_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="course-description">Description *</Label>
              <Textarea
                id="course-description"
                value={courseInfoFormData.description}
                onChange={(e) =>
                  setCourseInfoFormData({
                    ...courseInfoFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Describe the course content and objectives"
                rows={4}
              />
            </div>
            <div>
              <Label htmlFor="demo-video-title">
                Demonstration Video Title
              </Label>
              <Input
                id="demo-video-title"
                value={courseInfoFormData.demonstrationVideoTitle}
                onChange={(e) =>
                  setCourseInfoFormData({
                    ...courseInfoFormData,
                    demonstrationVideoTitle: e.target.value,
                  })
                }
                placeholder="Optional video title"
              />
            </div>
            <div>
              <Label htmlFor="course-demo-url">Demonstration Video URL</Label>
              <Input
                id="course-demo-url"
                value={courseInfoFormData.demonstrationVideoUrl}
                onChange={(e) =>
                  setCourseInfoFormData({
                    ...courseInfoFormData,
                    demonstrationVideoUrl: e.target.value,
                  })
                }
                placeholder="https://youtube.com/watch?v=..."
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setIsCourseInfoDialogOpen(false);
                if (!hasCourseInfo) {
                  resetCourseInfoForm();
                }
              }}
              disabled={submitting}
            >
              Cancel
            </Button>
            <Button onClick={handleSaveCourseInfo} disabled={submitting}>
              {submitting ? (
                <>
                  <Loader2 className="mr-2 size-4 animate-spin" />
                  {hasCourseInfo ? "Updating..." : "Creating..."}
                </>
              ) : hasCourseInfo ? (
                "Update Course Info"
              ) : (
                "Create Course Info"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ManageResourcesPage;
