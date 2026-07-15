# Quiz Question Page Architecture

## Component Hierarchy

```
QuizQuestionPage (191 lines)
├── Loading State
│   └── QuizLoadingScreen (13 lines)
│
├── Error State
│   └── QuizErrorScreen (29 lines)
│
├── Results State
│   ├── QuizResultsCard (existing)
│   └── QuizReviewSection (existing)
│
└── Active Quiz State
    ├── QuizHeader (89 lines)
    │   ├── Quiz Title & Description
    │   ├── Quiz Metadata (writer, date, likes, dislikes)
    │   ├── Progress Bar
    │   └── Share Button
    │
    ├── QuizQuestionCard[] (147 lines each)
    │   ├── Question Number Badge
    │   ├── Question Type Badge
    │   ├── Question Text & Image
    │   └── Answer Input (based on type)
    │       ├── SingleChoiceQuestion
    │       ├── MultipleChoiceQuestion
    │       ├── TrueFalse (Radio Group)
    │       └── ShortAnswerQuestion
    │
    └── QuizSubmitBar (89 lines)
        ├── Progress Summary
        ├── Submit Button
        └── Confirmation Dialog
```

## Custom Hooks

```
useQuizState (57 lines)
├── quiz: QuizDetails | null
├── loading: boolean
├── error: string | null
├── isLoggedIn: boolean
├── loadQuizDetails()
├── updateQuizLikes()
└── updateQuizDislikes()

useQuizAnswers (85 lines)
├── userAnswers: Map<number, UserAnswer>
├── handleSingleChoice()
├── handleMultipleChoice()
├── handleShortAnswer()
├── isAnswered()
├── getAnsweredCount()
└── resetAnswers()

useQuizSubmission (66 lines)
├── showResults: boolean
├── score: number
├── percentage: number
├── isSubmitting: boolean
├── showConfirmDialog: boolean
├── handleSubmitClick()
├── handleConfirmSubmit()
├── handleCancelSubmit()
├── isQuestionCorrect()
└── resetResults()
```

## Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    QuizQuestionPage                          │
│                                                               │
│  ┌────────────────┐  ┌──────────────┐  ┌─────────────────┐ │
│  │  useQuizState  │  │useQuizAnswers│  │useQuizSubmission│ │
│  │                │  │              │  │                 │ │
│  │ • Loads quiz   │  │ • Tracks     │  │ • Handles       │ │
│  │ • Manages      │  │   answers    │  │   submission    │ │
│  │   loading/error│  │ • Validates  │  │ • Calculates    │ │
│  │ • Updates      │  │   completion │  │   score         │ │
│  │   likes/       │  │              │  │                 │ │
│  │   dislikes     │  │              │  │                 │ │
│  └────────┬───────┘  └──────┬───────┘  └────────┬────────┘ │
│           │                 │                    │          │
│           ▼                 ▼                    ▼          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Component Props                           │  │
│  └──────────────────────────────────────────────────────┘  │
│           │                                                  │
│           ▼                                                  │
│  ┌────────────────────────────────────────────────┐        │
│  │           UI Components                         │        │
│  │  • QuizHeader                                   │        │
│  │  • QuizQuestionCard (for each question)        │        │
│  │  • QuizSubmitBar                                │        │
│  └────────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

## Benefits of This Architecture

### 1. **Modularity**

- Each component is self-contained
- Easy to modify without affecting other parts
- Can be tested independently

### 2. **Scalability**

- New question types can be added easily
- New features can be added without refactoring
- Components can be reused in other pages

### 3. **Maintainability**

- Clear separation of concerns
- Easier to locate and fix bugs
- Better code readability

### 4. **Performance**

- Smaller components re-render more efficiently
- State updates are more granular
- Easier to optimize specific components

### 5. **Developer Experience**

- Smaller files are easier to navigate
- Clear component responsibilities
- Better IntelliSense support
- Easier code reviews
