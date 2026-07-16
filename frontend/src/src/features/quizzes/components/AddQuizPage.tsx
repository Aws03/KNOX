// import { useState } from "react";
// import { useParams, useNavigate } from "react-router-dom";
// import {
//   ArrowLeft,
//   Plus,
//   Trash2,
//   Loader2,
//   Save,
//   Upload,
//   X,
//   ImageIcon,
//   AlertCircle,
// } from "lucide-react";
// import { Button } from "@/shared/ui/button";
// import {
//   Card,
//   CardContent,
//   CardHeader,
//   CardTitle,
//   CardDescription,
// } from "@/shared/ui/card";
// import { Input } from "@/shared/ui/input";
// import { Label } from "@/shared/ui/label";
// import { Textarea } from "@/shared/ui/textarea";
// import {
//   Select,
//   SelectContent,
//   SelectItem,
//   SelectTrigger,
//   SelectValue,
// } from "@/shared/ui/select";
// import { Checkbox } from "@/shared/ui/checkbox";
// import { Badge } from "@/shared/ui/badge";
// import { apiClient } from "@/lib/api/apiClient";

// // Question types matching the backend
// const QuestionType = {
//   SingleChoice: 1,
//   MultipleChoice: 2,
//   TrueFalse: 3,
//   ShortAnswer: 4,
// } as const;

// type QuestionTypeValue = (typeof QuestionType)[keyof typeof QuestionType];

// type Choice = {
//   id: string;
//   text: string;
//   imageUrl: string | null;
//   isCorrect: boolean;
//   uploadingImage: boolean;
// };

// type Question = {
//   id: string;
//   text: string;
//   imageUrl: string | null;
//   type: QuestionTypeValue;
//   choices: Choice[];
//   uploadingImage: boolean;
// };

// const AddQuizPage = () => {
//   const { courseId } = useParams<{ courseId: string }>();
//   const navigate = useNavigate();

//   const [title, setTitle] = useState("");
//   const [description, setDescription] = useState("");
//   const [questions, setQuestions] = useState<Question[]>([]);
//   const [isSubmitting, setIsSubmitting] = useState(false);
//   const [error, setError] = useState<string | null>(null);

//   const addQuestion = () => {
//     if (questions.length >= 100) {
//       setError("Maximum 100 questions per quiz");
//       return;
//     }

//     const newQuestion: Question = {
//       id: Date.now().toString(),
//       text: "",
//       imageUrl: null,
//       type: QuestionType.SingleChoice,
//       choices: [
//         {
//           id: Date.now().toString() + "-1",
//           text: "",
//           imageUrl: null,
//           isCorrect: false,
//           uploadingImage: false,
//         },
//         {
//           id: Date.now().toString() + "-2",
//           text: "",
//           imageUrl: null,
//           isCorrect: false,
//           uploadingImage: false,
//         },
//       ],
//       uploadingImage: false,
//     };

//     setQuestions([...questions, newQuestion]);
//     setError(null);
//   };

//   const removeQuestion = (questionId: string) => {
//     setQuestions(questions.filter((q) => q.id !== questionId));
//   };

//   const updateQuestion = <K extends keyof Question>(
//     questionId: string,
//     field: K,
//     value: Question[K]
//   ) => {
//     setQuestions(
//       questions.map((q) => (q.id === questionId ? { ...q, [field]: value } : q))
//     );
//   };

//   const addChoice = (questionId: string) => {
//     setQuestions(
//       questions.map((q) => {
//         if (q.id === questionId) {
//           if (q.choices.length >= 10) {
//             setError("Maximum 10 choices per question");
//             return q;
//           }
//           const newChoice: Choice = {
//             id: Date.now().toString(),
//             text: "",
//             imageUrl: null,
//             isCorrect: false,
//             uploadingImage: false,
//           };
//           return { ...q, choices: [...q.choices, newChoice] };
//         }
//         return q;
//       })
//     );
//   };

//   const removeChoice = (questionId: string, choiceId: string) => {
//     setQuestions(
//       questions.map((q) => {
//         if (q.id === questionId) {
//           return {
//             ...q,
//             choices: q.choices.filter((c) => c.id !== choiceId),
//           };
//         }
//         return q;
//       })
//     );
//   };

//   const updateChoice = <K extends keyof Choice>(
//     questionId: string,
//     choiceId: string,
//     field: K,
//     value: Choice[K]
//   ) => {
//     setQuestions(
//       questions.map((q) => {
//         if (q.id === questionId) {
//           return {
//             ...q,
//             choices: q.choices.map((c) =>
//               c.id === choiceId ? { ...c, [field]: value } : c
//             ),
//           };
//         }
//         return q;
//       })
//     );
//   };

//   const uploadImage = async (file: File): Promise<string | null> => {
//     try {
//       const formData = new FormData();
//       formData.append("file", file);

//       const response = await apiClient.post(
//         "/files/upload/temporary",
//         formData,
//         {
//           headers: {
//             "Content-Type": "multipart/form-data",
//           },
//         }
//       );

//       return response.data.fileUrl;
//     } catch (err) {
//       console.error("Failed to upload image:", err);
//       setError("Failed to upload image. Please try again.");
//       return null;
//     }
//   };

//   const handleQuestionImageUpload = async (
//     questionId: string,
//     event: React.ChangeEvent<HTMLInputElement>
//   ) => {
//     const file = event.target.files?.[0];
//     if (!file) return;

//     // Validate file type
//     if (!file.type.startsWith("image/")) {
//       setError("Please select an image file");
//       event.target.value = ""; // Reset input
//       return;
//     }

//     // Validate file size (max 5MB)
//     if (file.size > 5 * 1024 * 1024) {
//       setError("Image size must be less than 5MB");
//       event.target.value = ""; // Reset input
//       return;
//     }

//     // Update state to show uploading status
//     setQuestions((prevQuestions) =>
//       prevQuestions.map((q) =>
//         q.id === questionId ? { ...q, uploadingImage: true } : q
//       )
//     );

//     const imageUrl = await uploadImage(file);

//     if (imageUrl) {
//       // Update state with the new image URL
//       setQuestions((prevQuestions) =>
//         prevQuestions.map((q) =>
//           q.id === questionId ? { ...q, imageUrl, uploadingImage: false } : q
//         )
//       );
//     } else {
//       // Failed to upload, reset uploading status
//       setQuestions((prevQuestions) =>
//         prevQuestions.map((q) =>
//           q.id === questionId ? { ...q, uploadingImage: false } : q
//         )
//       );
//     }

//     // Reset the input so the same file can be selected again
//     event.target.value = "";
//   };

//   const handleChoiceImageUpload = async (
//     questionId: string,
//     choiceId: string,
//     event: React.ChangeEvent<HTMLInputElement>
//   ) => {
//     const file = event.target.files?.[0];
//     if (!file) return;

//     // Validate file type
//     if (!file.type.startsWith("image/")) {
//       setError("Please select an image file");
//       event.target.value = ""; // Reset input
//       return;
//     }

//     // Validate file size (max 5MB)
//     if (file.size > 5 * 1024 * 1024) {
//       setError("Image size must be less than 5MB");
//       event.target.value = ""; // Reset input
//       return;
//     }

//     // Update state to show uploading status
//     setQuestions((prevQuestions) =>
//       prevQuestions.map((q) => {
//         if (q.id === questionId) {
//           return {
//             ...q,
//             choices: q.choices.map((c) =>
//               c.id === choiceId ? { ...c, uploadingImage: true } : c
//             ),
//           };
//         }
//         return q;
//       })
//     );

//     const imageUrl = await uploadImage(file);

//     if (imageUrl) {
//       // Update state with the new image URL
//       setQuestions((prevQuestions) =>
//         prevQuestions.map((q) => {
//           if (q.id === questionId) {
//             return {
//               ...q,
//               choices: q.choices.map((c) =>
//                 c.id === choiceId
//                   ? { ...c, imageUrl, uploadingImage: false }
//                   : c
//               ),
//             };
//           }
//           return q;
//         })
//       );
//     } else {
//       // Failed to upload, reset uploading status
//       setQuestions((prevQuestions) =>
//         prevQuestions.map((q) => {
//           if (q.id === questionId) {
//             return {
//               ...q,
//               choices: q.choices.map((c) =>
//                 c.id === choiceId ? { ...c, uploadingImage: false } : c
//               ),
//             };
//           }
//           return q;
//         })
//       );
//     }

//     // Reset the input so the same file can be selected again
//     event.target.value = "";
//   };

//   const removeQuestionImage = (questionId: string) => {
//     setQuestions((prevQuestions) =>
//       prevQuestions.map((q) =>
//         q.id === questionId ? { ...q, imageUrl: null } : q
//       )
//     );
//   };

//   const removeChoiceImage = (questionId: string, choiceId: string) => {
//     setQuestions((prevQuestions) =>
//       prevQuestions.map((q) => {
//         if (q.id === questionId) {
//           return {
//             ...q,
//             choices: q.choices.map((c) =>
//               c.id === choiceId ? { ...c, imageUrl: null } : c
//             ),
//           };
//         }
//         return q;
//       })
//     );
//   };

//   const validateQuiz = (): string | null => {
//     if (!title.trim()) {
//       return "Please enter a quiz title";
//     }

//     if (questions.length === 0) {
//       return "Please add at least one question";
//     }

//     for (let i = 0; i < questions.length; i++) {
//       const question = questions[i];

//       if (!question.text.trim()) {
//         return `Question ${i + 1}: Please enter question text`;
//       }

//       if (question.type !== QuestionType.ShortAnswer) {
//         if (question.choices.length < 2) {
//           return `Question ${i + 1}: Please add at least 2 choices`;
//         }

//         const hasCorrectAnswer = question.choices.some((c) => c.isCorrect);
//         if (!hasCorrectAnswer) {
//           return `Question ${i + 1}: Please mark at least one correct answer`;
//         }

//         for (let j = 0; j < question.choices.length; j++) {
//           const choice = question.choices[j];
//           if (!choice.text.trim() && !choice.imageUrl) {
//             return `Question ${i + 1}, Choice ${
//               j + 1
//             }: Please enter choice text or add an image`;
//           }
//         }

//         // For Single Choice and True/False, only one answer should be correct
//         if (
//           question.type === QuestionType.SingleChoice ||
//           question.type === QuestionType.TrueFalse
//         ) {
//           const correctCount = question.choices.filter(
//             (c) => c.isCorrect
//           ).length;
//           if (correctCount > 1) {
//             return `Question ${i + 1}: Only one answer can be correct for ${
//               question.type === QuestionType.SingleChoice
//                 ? "Single Choice"
//                 : "True/False"
//             } questions`;
//           }
//         }
//       } else {
//         // Short Answer
//         if (question.choices.length === 0) {
//           return `Question ${i + 1}: Please add the correct answer`;
//         }
//         if (!question.choices[0].text.trim()) {
//           return `Question ${i + 1}: Please enter the correct answer`;
//         }
//       }
//     }

//     return null;
//   };

//   const handleSubmit = async () => {
//     setError(null);

//     const validationError = validateQuiz();
//     if (validationError) {
//       setError(validationError);
//       window.scrollTo({ top: 0, behavior: "smooth" });
//       return;
//     }

//     setIsSubmitting(true);

//     try {
//       // Get user ID from auth storage
//       const authData = localStorage.getItem("accessToken");
//       let writerId = 1; // Default value

//       if (authData) {
//         try {
//           const tokenPayload = JSON.parse(atob(authData.split(".")[1]));
//           writerId = parseInt(
//             tokenPayload[
//               "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
//             ] || "1"
//           );
//         } catch (e) {
//           console.error("Failed to parse auth token:", e);
//         }
//       }

//       const payload = {
//         title: title.trim(),
//         writerId,
//         courseId: parseInt(courseId!),
//         description: description.trim() || null,
//         questions: questions.map((q) => ({
//           text: q.text.trim(),
//           quizId: 0,
//           imageUrl: q.imageUrl,
//           type: q.type,
//           choices: q.choices.map((c) => ({
//             text: c.text.trim(),
//             imageUrl: c.imageUrl,
//             isCorrect: c.isCorrect,
//           })),
//         })),
//       };

//       await apiClient.post("/quizzes", payload);

//       // Navigate back to quizzes page
//       navigate(`/courses/${courseId}/quizzes`);
//     } catch (err) {
//       console.error("Failed to create quiz:", err);
//       const error = err as {
//         response?: { data?: { message?: string } };
//         message?: string;
//       };
//       setError(
//         error.response?.data?.message ||
//           error.message ||
//           "Failed to create quiz. Please try again."
//       );
//       window.scrollTo({ top: 0, behavior: "smooth" });
//     } finally {
//       setIsSubmitting(false);
//     }
//   };

//   const getQuestionTypeLabel = (type: QuestionTypeValue): string => {
//     switch (type) {
//       case QuestionType.SingleChoice:
//         return "Single Choice";
//       case QuestionType.MultipleChoice:
//         return "Multiple Choice";
//       case QuestionType.TrueFalse:
//         return "True/False";
//       case QuestionType.ShortAnswer:
//         return "Short Answer";
//       default:
//         return "Unknown";
//     }
//   };

//   return (
//     <div className="min-h-screen bg-linear-to-b from-background to-muted/20">
//       <div className="container mx-auto px-4 py-6 max-w-5xl">
//         {/* Header */}
//         <div className="mb-6">
//           <Button
//             variant="ghost"
//             className="mb-4 gap-2"
//             onClick={() => navigate(-1)}
//           >
//             <ArrowLeft className="h-4 w-4" />
//             Back to Quizzes
//           </Button>

//           <h1 className="text-3xl font-bold tracking-tight mb-2">
//             Create New Quiz
//           </h1>
//           <p className="text-muted-foreground">
//             Add questions and choices to create an engaging quiz for your course
//           </p>
//         </div>

//         {/* Error Alert */}
//         {error && (
//           <Card className="mb-6 border-destructive/50 bg-destructive/10">
//             <CardContent className="pt-6">
//               <div className="flex items-start gap-3">
//                 <AlertCircle className="h-5 w-5 text-destructive mt-0.5" />
//                 <div>
//                   <h3 className="font-semibold text-destructive mb-1">Error</h3>
//                   <p className="text-sm text-destructive/90">{error}</p>
//                 </div>
//                 <Button
//                   variant="ghost"
//                   size="icon"
//                   className="ml-auto"
//                   onClick={() => setError(null)}
//                 >
//                   <X className="h-4 w-4" />
//                 </Button>
//               </div>
//             </CardContent>
//           </Card>
//         )}

//         {/* Quiz Details */}
//         <Card className="mb-6">
//           <CardHeader>
//             <CardTitle>Quiz Details</CardTitle>
//             <CardDescription>Basic information about your quiz</CardDescription>
//           </CardHeader>
//           <CardContent className="space-y-4">
//             <div className="space-y-2">
//               <Label htmlFor="title">
//                 Title <span className="text-destructive">*</span>
//               </Label>
//               <Input
//                 id="title"
//                 placeholder="Enter quiz title..."
//                 value={title}
//                 onChange={(e) => setTitle(e.target.value)}
//                 maxLength={200}
//               />
//             </div>

//             <div className="space-y-2">
//               <Label htmlFor="description">Description (Optional)</Label>
//               <Textarea
//                 id="description"
//                 placeholder="Enter quiz description..."
//                 value={description}
//                 onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
//                   setDescription(e.target.value)
//                 }
//                 rows={3}
//                 maxLength={1000}
//               />
//             </div>
//           </CardContent>
//         </Card>

//         {/* Questions */}
//         <div className="space-y-6 mb-6">
//           <div>
//             <h2 className="text-2xl font-bold">Questions</h2>
//             <p className="text-sm text-muted-foreground">
//               {questions.length} / 100 questions
//             </p>
//           </div>

//           {questions.length === 0 && (
//             <Card className="p-12 bg-linear-to-br from-card to-muted/20">
//               <div className="flex flex-col items-center justify-center text-center">
//                 <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 mb-4">
//                   <Plus className="h-10 w-10 text-primary" />
//                 </div>
//                 <h3 className="text-lg font-semibold mb-2">No questions yet</h3>
//                 <p className="text-sm text-muted-foreground max-w-sm mb-4">
//                   Start building your quiz by adding your first question
//                 </p>
//                 <Button onClick={addQuestion}>
//                   <Plus className="h-4 w-4 mr-2" />
//                   Add First Question
//                 </Button>
//               </div>
//             </Card>
//           )}

//           {questions.map((question, qIndex) => (
//             <Card key={question.id} className="overflow-hidden">
//               <CardHeader className="bg-muted/30">
//                 <div className="flex items-start justify-between gap-4">
//                   <div className="flex items-center gap-3">
//                     <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 font-semibold text-primary">
//                       {qIndex + 1}
//                     </div>
//                     <div>
//                       <CardTitle className="text-lg">
//                         Question {qIndex + 1}
//                       </CardTitle>
//                       <CardDescription>
//                         {getQuestionTypeLabel(question.type)}
//                       </CardDescription>
//                     </div>
//                   </div>
//                   <Button
//                     variant="ghost"
//                     size="icon"
//                     onClick={() => removeQuestion(question.id)}
//                   >
//                     <Trash2 className="h-4 w-4 text-destructive" />
//                   </Button>
//                 </div>
//               </CardHeader>

//               <CardContent className="pt-6 space-y-4">
//                 {/* Question Type */}
//                 <div className="space-y-2">
//                   <Label>
//                     Question Type <span className="text-destructive">*</span>
//                   </Label>
//                   <Select
//                     value={question.type.toString()}
//                     onValueChange={(value) => {
//                       const newType = parseInt(value) as QuestionTypeValue;
//                       updateQuestion(question.id, "type", newType);

//                       // Adjust choices based on type
//                       if (newType === QuestionType.TrueFalse) {
//                         updateQuestion(question.id, "choices", [
//                           {
//                             id: Date.now().toString() + "-true",
//                             text: "True",
//                             imageUrl: null,
//                             isCorrect: false,
//                             uploadingImage: false,
//                           },
//                           {
//                             id: Date.now().toString() + "-false",
//                             text: "False",
//                             imageUrl: null,
//                             isCorrect: false,
//                             uploadingImage: false,
//                           },
//                         ]);
//                       } else if (newType === QuestionType.ShortAnswer) {
//                         updateQuestion(question.id, "choices", [
//                           {
//                             id: Date.now().toString(),
//                             text: "",
//                             imageUrl: null,
//                             isCorrect: true,
//                             uploadingImage: false,
//                           },
//                         ]);
//                       }
//                     }}
//                   >
//                     <SelectTrigger>
//                       <SelectValue />
//                     </SelectTrigger>
//                     <SelectContent>
//                       <SelectItem value={QuestionType.SingleChoice.toString()}>
//                         Single Choice
//                       </SelectItem>
//                       <SelectItem
//                         value={QuestionType.MultipleChoice.toString()}
//                       >
//                         Multiple Choice
//                       </SelectItem>
//                       <SelectItem value={QuestionType.TrueFalse.toString()}>
//                         True/False
//                       </SelectItem>
//                       <SelectItem value={QuestionType.ShortAnswer.toString()}>
//                         Short Answer
//                       </SelectItem>
//                     </SelectContent>
//                   </Select>
//                 </div>

//                 {/* Question Text */}
//                 <div className="space-y-2">
//                   <Label>
//                     Question Text <span className="text-destructive">*</span>
//                   </Label>
//                   <Textarea
//                     placeholder="Enter your question..."
//                     value={question.text}
//                     onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
//                       updateQuestion(question.id, "text", e.target.value)
//                     }
//                     rows={3}
//                     maxLength={1000}
//                   />
//                 </div>

//                 {/* Question Image */}
//                 <div className="space-y-2">
//                   <Label>Question Image (Optional)</Label>
//                   {question.imageUrl ? (
//                     <div className="relative inline-block">
//                       <img
//                         src={question.imageUrl}
//                         alt="Question"
//                         className="max-w-xs rounded-lg border"
//                       />
//                       <Button
//                         variant="destructive"
//                         size="icon"
//                         className="absolute top-2 right-2"
//                         onClick={() => removeQuestionImage(question.id)}
//                       >
//                         <X className="h-4 w-4" />
//                       </Button>
//                     </div>
//                   ) : (
//                     <div>
//                       <Input
//                         type="file"
//                         accept="image/*"
//                         onChange={(e) =>
//                           handleQuestionImageUpload(question.id, e)
//                         }
//                         disabled={question.uploadingImage}
//                         className="hidden"
//                         id={`question-image-${question.id}`}
//                       />
//                       <Label
//                         htmlFor={`question-image-${question.id}`}
//                         className="cursor-pointer"
//                       >
//                         <div className="border-2 border-dashed rounded-lg p-6 hover:bg-muted/50 transition-colors text-center">
//                           {question.uploadingImage ? (
//                             <div className="flex items-center justify-center gap-2">
//                               <Loader2 className="h-5 w-5 animate-spin" />
//                               <span className="text-sm">Uploading...</span>
//                             </div>
//                           ) : (
//                             <div className="flex flex-col items-center gap-2">
//                               <Upload className="h-8 w-8 text-muted-foreground" />
//                               <div>
//                                 <p className="text-sm font-medium">
//                                   Click to upload image
//                                 </p>
//                                 <p className="text-xs text-muted-foreground">
//                                   PNG, JPG, GIF up to 5MB
//                                 </p>
//                               </div>
//                             </div>
//                           )}
//                         </div>
//                       </Label>
//                     </div>
//                   )}
//                 </div>

//                 {/* Choices */}
//                 {question.type !== QuestionType.ShortAnswer && (
//                   <div className="space-y-4 pt-4 border-t">
//                     <div className="flex items-center justify-between">
//                       <Label>
//                         Choices <span className="text-destructive">*</span>
//                         {question.type === QuestionType.MultipleChoice && (
//                           <span className="text-xs text-muted-foreground ml-2">
//                             (Select all correct answers)
//                           </span>
//                         )}
//                       </Label>
//                       {question.type !== QuestionType.TrueFalse && (
//                         <Button
//                           variant="outline"
//                           size="sm"
//                           onClick={() => addChoice(question.id)}
//                           disabled={question.choices.length >= 10}
//                         >
//                           <Plus className="h-3 w-3 mr-1" />
//                           Add Choice
//                         </Button>
//                       )}
//                     </div>

//                     <div className="space-y-3">
//                       {question.choices.map((choice, cIndex) => (
//                         <Card key={choice.id} className="p-4">
//                           <div className="space-y-3">
//                             <div className="flex items-start gap-3">
//                               <div className="flex items-center pt-2">
//                                 <Checkbox
//                                   checked={choice.isCorrect}
//                                   onCheckedChange={(checked) =>
//                                     updateChoice(
//                                       question.id,
//                                       choice.id,
//                                       "isCorrect",
//                                       checked === true
//                                     )
//                                   }
//                                 />
//                               </div>
//                               <div className="flex-1 space-y-3">
//                                 <div className="flex items-center gap-2">
//                                   <Badge variant="outline">
//                                     {String.fromCharCode(65 + cIndex)}
//                                   </Badge>
//                                   <Input
//                                     placeholder={`Choice ${cIndex + 1}...`}
//                                     value={choice.text}
//                                     onChange={(e) =>
//                                       updateChoice(
//                                         question.id,
//                                         choice.id,
//                                         "text",
//                                         e.target.value
//                                       )
//                                     }
//                                     disabled={
//                                       question.type === QuestionType.TrueFalse
//                                     }
//                                     maxLength={500}
//                                   />
//                                 </div>

//                                 {/* Choice Image */}
//                                 {choice.imageUrl ? (
//                                   <div className="relative inline-block">
//                                     <img
//                                       src={choice.imageUrl}
//                                       alt={`Choice ${cIndex + 1}`}
//                                       className="max-w-[200px] rounded-lg border"
//                                     />
//                                     <Button
//                                       variant="destructive"
//                                       size="icon"
//                                       className="absolute top-1 right-1 h-6 w-6"
//                                       onClick={() =>
//                                         removeChoiceImage(
//                                           question.id,
//                                           choice.id
//                                         )
//                                       }
//                                     >
//                                       <X className="h-3 w-3" />
//                                     </Button>
//                                   </div>
//                                 ) : (
//                                   <div>
//                                     <Input
//                                       type="file"
//                                       accept="image/*"
//                                       onChange={(e) =>
//                                         handleChoiceImageUpload(
//                                           question.id,
//                                           choice.id,
//                                           e
//                                         )
//                                       }
//                                       disabled={choice.uploadingImage}
//                                       className="hidden"
//                                       id={`choice-image-${choice.id}`}
//                                     />
//                                     <Label
//                                       htmlFor={`choice-image-${choice.id}`}
//                                       className="cursor-pointer"
//                                     >
//                                       <div className="border border-dashed rounded p-3 hover:bg-muted/50 transition-colors">
//                                         {choice.uploadingImage ? (
//                                           <div className="flex items-center gap-2 text-sm">
//                                             <Loader2 className="h-4 w-4 animate-spin" />
//                                             <span>Uploading...</span>
//                                           </div>
//                                         ) : (
//                                           <div className="flex items-center gap-2 text-sm text-muted-foreground">
//                                             <ImageIcon className="h-4 w-4" />
//                                             <span>Add image (optional)</span>
//                                           </div>
//                                         )}
//                                       </div>
//                                     </Label>
//                                   </div>
//                                 )}
//                               </div>
//                               {question.type !== QuestionType.TrueFalse &&
//                                 question.choices.length > 2 && (
//                                   <Button
//                                     variant="ghost"
//                                     size="icon"
//                                     onClick={() =>
//                                       removeChoice(question.id, choice.id)
//                                     }
//                                   >
//                                     <Trash2 className="h-4 w-4 text-destructive" />
//                                   </Button>
//                                 )}
//                             </div>
//                           </div>
//                         </Card>
//                       ))}
//                     </div>
//                   </div>
//                 )}

//                 {/* Short Answer */}
//                 {question.type === QuestionType.ShortAnswer && (
//                   <div className="space-y-2 pt-4 border-t">
//                     <Label>
//                       Correct Answer <span className="text-destructive">*</span>
//                     </Label>
//                     <Input
//                       placeholder="Enter the correct answer..."
//                       value={question.choices[0]?.text || ""}
//                       onChange={(e) =>
//                         updateChoice(
//                           question.id,
//                           question.choices[0]?.id,
//                           "text",
//                           e.target.value
//                         )
//                       }
//                       maxLength={500}
//                     />
//                     <p className="text-xs text-muted-foreground">
//                       The answer will be checked case-insensitively
//                     </p>
//                   </div>
//                 )}
//               </CardContent>
//             </Card>
//           ))}

//           {/* Add Question Button */}
//           {questions.length > 0 && (
//             <div className="flex justify-center pt-4">
//               <Button
//                 onClick={addQuestion}
//                 disabled={questions.length >= 100}
//                 size="lg"
//                 variant="outline"
//                 className="w-full sm:w-auto"
//               >
//                 <Plus className="h-4 w-4 mr-2" />
//                 Add Another Question
//               </Button>
//             </div>
//           )}
//         </div>

//         {/* Submit Section */}
//         <Card className="sticky bottom-4 shadow-2xl border-2">
//           <CardContent className="pt-6">
//             <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
//               <div>
//                 <p className="font-semibold">Ready to create your quiz?</p>
//                 <p className="text-sm text-muted-foreground">
//                   {questions.length} question{questions.length !== 1 ? "s" : ""}{" "}
//                   added
//                 </p>
//               </div>
//               <div className="flex gap-2 w-full sm:w-auto">
//                 <Button
//                   variant="outline"
//                   onClick={() => navigate(-1)}
//                   disabled={isSubmitting}
//                   className="flex-1 sm:flex-none"
//                 >
//                   Cancel
//                 </Button>
//                 <Button
//                   onClick={handleSubmit}
//                   disabled={isSubmitting || questions.length === 0}
//                   className="flex-1 sm:flex-none min-w-[150px]"
//                 >
//                   {isSubmitting ? (
//                     <>
//                       <Loader2 className="h-4 w-4 mr-2 animate-spin" />
//                       Creating...
//                     </>
//                   ) : (
//                     <>
//                       <Save className="h-4 w-4 mr-2" />
//                       Create Quiz
//                     </>
//                   )}
//                 </Button>
//               </div>
//             </div>
//           </CardContent>
//         </Card>
//       </div>
//     </div>
//   );
// };

// export default AddQuizPage;
