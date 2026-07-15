# AI-Powered Quiz Generation Feature - Implementation Complete

## ? What Has Been Implemented

### 1. Domain Layer
- ? `QuizGenerationJob` entity for tracking quiz generation processes
- ? `QuizGenerationStatus` enum (Pending, Extracting, GeneratingQuizzes, Completed, Failed, Cancelled)
- ? `QuizSource` enum (Manual, AIGenerated)
- ? `SystemSetting` entity for feature flag management
- ? Updated `Quiz` entity with source tracking properties
- ? Updated `CourseMaterial` entity with quiz generation tracking

### 2. Application Layer
- ? Service interfaces:
  - `ITextExtractionService` - Extract text from PDF/DOCX/PPTX
  - `IOpenAIService` - Generate quizzes using ChatGPT API
  - `IFeatureFlagService` - Manage feature flags
- ? Commands:
  - `GenerateQuizFromMaterialCommand` - Initiate quiz generation
  - `ProcessQuizGenerationJobCommand` - Process generation jobs
- ? Queries:
  - `GetQuizGenerationJobStatusQuery` - Check job status
  - `GetQuizGenerationJobsByMaterialQuery` - Get all jobs for a material
- ? DTOs and Mappers for all new entities
- ? Updated `CreateCourseMaterialCommand` to support auto quiz generation

### 3. Infrastructure Layer
- ? `TextExtractionService` - Placeholder implementations for PDF/DOCX/PPTX
- ? `OpenAIService` - Full ChatGPT integration with retry logic
- ? `FeatureFlagService` - Cached feature flag management
- ? Database configurations for all new entities
- ? Service registration in DependencyInjection

### 4. API Layer
- ? `QuizGenerationController` - Quiz generation endpoints
- ? `SystemSettingsController` - Feature flag management
- ? Updated configuration with OpenAI settings

---

## ?? Required Setup Steps

### 1. Install NuGet Packages

```bash
# Navigate to Infrastructure project
cd JadaraITKnowledgeSystem.Infrastructure

# For PDF extraction
dotnet add package itext7

# For DOCX/PPTX extraction
dotnet add package DocumentFormat.OpenXml

# OpenAI SDK (optional, current implementation uses HttpClient)
# dotnet add package OpenAI
# OR
# dotnet add package Azure.AI.OpenAI
```

### 2. Update TextExtractionService

After installing packages, uncomment the implementations in:
`JadaraITKnowledgeSystem.Infrastructure/Services/TextExtraction/TextExtractionService.cs`

**For PDF (itext7):**
```csharp
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

private async Task<string> ExtractFromPdfAsync(Stream stream, CancellationToken cancellationToken)
{
    using var pdfReader = new PdfReader(stream);
    using var pdfDocument = new PdfDocument(pdfReader);
    var text = new StringBuilder();

    for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
    {
        var page = pdfDocument.GetPage(pageNum);
        var strategy = new SimpleTextExtractionStrategy();
        var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
        text.AppendLine(pageText);
    }

    return await Task.FromResult(text.ToString());
}
```

**For DOCX:**
```csharp
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

private async Task<string> ExtractFromDocxAsync(Stream stream, CancellationToken cancellationToken)
{
    using var document = WordprocessingDocument.Open(stream, false);
    var body = document.MainDocumentPart?.Document.Body;
    
    if (body == null)
        return string.Empty;

    var text = new StringBuilder();
    foreach (var paragraph in body.Descendants<Paragraph>())
    {
        text.AppendLine(paragraph.InnerText);
    }

    return await Task.FromResult(text.ToString());
}
```

**For PPTX:**
```csharp
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

private async Task<string> ExtractFromPptxAsync(Stream stream, CancellationToken cancellationToken)
{
    using var presentation = PresentationDocument.Open(stream, false);
    var presentationPart = presentation.PresentationPart;
    var text = new StringBuilder();

    if (presentationPart?.SlideParts != null)
    {
        foreach (var slidePart in presentationPart.SlideParts)
        {
            var slide = slidePart.Slide;
            foreach (var paragraph in slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
            {
                text.AppendLine(paragraph.InnerText);
            }
        }
    }

    return await Task.FromResult(text.ToString());
}
```

### 3. Configure OpenAI API

Update `appsettings.json` with your OpenAI API key:

```json
"OpenAI": {
  "ApiKey": "sk-your-actual-openai-api-key",
  "Model": "gpt-4-turbo-preview",
  "MaxTokens": 4000,
  "Temperature": 0.7,
  "Enabled": true
}
```

**Get your API key from:** https://platform.openai.com/api-keys

### 4. Run Database Migration

```bash
cd JadaraITKnowledgeSystem.Infrastructure

# Generate migration
dotnet ef migrations add AddQuizGenerationFeature --startup-project ../JadaraITKnowledgeSystem.API

# Apply migration
dotnet ef database update --startup-project ../JadaraITKnowledgeSystem.API
```

### 5. Verify Database

After migration, verify tables were created:

```sql
-- Check new tables
SELECT * FROM SystemSettings;
SELECT * FROM QuizGenerationJobs;

-- Check updated columns
SELECT TOP 1 Id, Source, SourceMaterialId FROM Quizzes;
SELECT TOP 1 Id, HasGeneratedQuizzes, AutoGeneratedQuizCount FROM CourseMaterials;
```

---

## ?? How to Use

### 1. Upload Material with Auto Quiz Generation

**API Request:**
```http
POST /api/course-materials
Content-Type: application/json

{
  "title": "Introduction to Algorithms",
  "contemtUrl": "https://storage.example.com/algorithms.pdf",
  "courseId": 1,
  "folderId": null,
  "description": "Course material for algorithms",
  "tags": ["algorithms", "computer-science"],
  "generateQuiz": true,
  "quizOptions": {
    "questionsPerQuiz": 10,
    "difficulty": "Medium",
    "maxQuizzes": 3,
    "autoPublish": false
  }
}
```

### 2. Manually Generate Quiz from Existing Material

```http
POST /api/quiz-generation/materials/123
Content-Type: application/json

{
  "questionsPerQuiz": 8,
  "difficulty": "Hard",
  "maxQuizzes": 5,
  "autoPublish": false
}
```

**Response (202 Accepted):**
```json
{
  "id": 1,
  "materialId": 123,
  "materialTitle": "Introduction to Algorithms",
  "courseId": 1,
  "courseName": "CS101",
  "status": 0,
  "generatedQuizCount": 0,
  "generatedQuizIds": [],
  "errorMessage": null,
  "createdAt": "2024-01-15T10:00:00Z",
  "completedAt": null,
  "options": {
    "questionsPerQuiz": 8,
    "difficulty": "Hard",
    "maxQuizzes": 5,
    "autoPublish": false
  }
}
```

### 3. Check Generation Status

```http
GET /api/quiz-generation/jobs/1
```

**Response:**
```json
{
  "id": 1,
  "status": 3,
  "generatedQuizCount": 3,
  "generatedQuizIds": [45, 46, 47],
  "completedAt": "2024-01-15T10:05:00Z"
}
```

### 4. Toggle Feature Flag (Admin Only)

```http
PUT /api/system/settings/feature-flags/quiz-generation
Content-Type: application/json

{
  "enabled": true
}
```

### 5. Check if Feature is Enabled

```http
GET /api/system/settings/feature-flags/quiz-generation
```

---

## ?? API Endpoints

### Quiz Generation
- `POST /api/quiz-generation/materials/{materialId}` - Generate quiz
- `GET /api/quiz-generation/jobs/{jobId}` - Get job status
- `GET /api/quiz-generation/materials/{materialId}/jobs` - Get all jobs for material

### System Settings (Admin)
- `GET /api/system/settings/feature-flags` - Get all feature flags
- `GET /api/system/settings/feature-flags/quiz-generation` - Check if enabled
- `PUT /api/system/settings/feature-flags/quiz-generation` - Toggle feature

---

## ?? Generated Quiz Structure

When a quiz is generated, it will have:

- **Title:** `{MaterialName} - Part {N}/{Total} - {Topic}`
- **Description:** AI-generated description of quiz content
- **Source:** `AIGenerated` (enum value: 1)
- **SourceMaterialId:** Links back to the course material
- **PartNumber/TotalParts:** If document was split into multiple quizzes
- **Tags:** Combination of material tags + AI-suggested topics + metadata tags
- **Questions:** AI-generated multiple-choice questions with 4 choices each

---

## ?? Configuration Options

### System Settings (Database)

| Setting Key | Default | Description |
|------------|---------|-------------|
| `Features.QuizGeneration.Enabled` | `true` | Enable/disable quiz generation |
| `Features.QuizGeneration.MaxConcurrent` | `3` | Max concurrent jobs |
| `Features.QuizGeneration.DefaultQuestions` | `8` | Default questions per quiz |

### OpenAI Settings (appsettings.json)

| Setting | Recommended | Description |
|---------|------------|-------------|
| `Model` | `gpt-4-turbo-preview` | GPT model to use |
| `MaxTokens` | `4000` | Max tokens per request |
| `Temperature` | `0.7` | Creativity level (0-1) |

### Quiz Generation Options (Per Request)

| Option | Min | Max | Default |
|--------|-----|-----|---------|
| `QuestionsPerQuiz` | 5 | 20 | 8 |
| `MaxQuizzes` | 1 | 10 | 5 |
| `Difficulty` | - | - | "Medium" |

---

## ?? Error Handling

The system handles various error scenarios:

1. **File Type Not Supported:** Returns 400 Bad Request
2. **Text Extraction Failed:** Job marked as Failed with error message
3. **OpenAI API Error:** Retries 3 times, then marks job as Failed
4. **Feature Disabled:** Returns validation error
5. **Material Not Found:** Returns 404 Not Found

---

## ?? Monitoring

### Check System Status

```sql
-- Active jobs
SELECT * FROM QuizGenerationJobs 
WHERE Status IN (0, 1, 2) -- Pending, Extracting, GeneratingQuizzes
ORDER BY CreatedAt DESC;

-- Failed jobs
SELECT * FROM QuizGenerationJobs 
WHERE Status = 4 -- Failed
ORDER BY CreatedAt DESC;

-- Success rate
SELECT 
    Status,
    COUNT(*) as Count,
    AVG(DATEDIFF(SECOND, CreatedAt, CompletedAt)) as AvgDurationSeconds
FROM QuizGenerationJobs
WHERE CompletedAt IS NOT NULL
GROUP BY Status;

-- Materials with generated quizzes
SELECT 
    m.Id,
    m.Title,
    m.HasGeneratedQuizzes,
    m.AutoGeneratedQuizCount
FROM CourseMaterials m
WHERE m.HasGeneratedQuizzes = 1;

-- AI-generated quizzes
SELECT 
    q.Id,
    q.Title,
    q.Source,
    q.PartNumber,
    q.TotalParts,
    q.CreatedAt
FROM Quizzes q
WHERE q.Source = 1 -- AIGenerated
ORDER BY q.CreatedAt DESC;
```

---

## ?? Security Considerations

1. **Authorization:** Quiz generation requires authenticated user
2. **Rate Limiting:** Consider adding rate limits per user
3. **File Size Limits:** Already enforced by file upload (10MB)
4. **API Key Security:** Store OpenAI API key securely (use Azure Key Vault in production)
5. **Content Validation:** AI-generated content is validated before saving

---

## ?? Best Practices

1. **Monitor OpenAI Costs:** Track API usage and set budget limits
2. **Test with Sample Documents:** Start with small PDFs to verify extraction
3. **Review Generated Quizzes:** AI-generated content should be reviewed
4. **Enable Feature Gradually:** Test with limited users first
5. **Set Appropriate Limits:** Adjust `MaxQuizzes` based on document size

---

## ?? Troubleshooting

### PDF Extraction Returns Empty
- Check if PDF is encrypted or image-based
- Verify itext7 package is installed
- Test with a simple text-based PDF first

### OpenAI API Errors
- Verify API key is correct
- Check OpenAI account has credits
- Review rate limits for your plan

### Quizzes Not Generating
- Check `SystemSettings` table - is feature enabled?
- Check `QuizGenerationJobs` table for error messages
- Review application logs for exceptions

### Performance Issues
- Reduce `MaxQuizzes` for large documents
- Adjust `MaxTokens` if hitting limits
- Consider implementing queue-based processing

---

## ?? Next Steps

### Optional Enhancements

1. **Add Hangfire for Background Jobs:**
   ```bash
   dotnet add package Hangfire.AspNetCore
   dotnet add package Hangfire.SqlServer
   ```

2. **Add SignalR for Real-time Updates:**
   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR
   ```

3. **Add Rate Limiting:**
   ```bash
   dotnet add package AspNetCoreRateLimit
   ```

4. **Enhanced Text Extraction:**
   - OCR for image-based PDFs (Tesseract)
   - Better section detection using NLP
   - Multi-language support

---

## ?? License & Credits

This implementation follows Clean Architecture principles and uses:
- **MediatR** for CQRS pattern
- **FluentValidation** for validation
- **Entity Framework Core** for data access
- **OpenAI API** for quiz generation
- **itext7** for PDF extraction
- **DocumentFormat.OpenXml** for Office documents

---

## ? Testing Checklist

Before deploying to production:

- [ ] Install required NuGet packages
- [ ] Uncomment text extraction implementations
- [ ] Configure OpenAI API key
- [ ] Run database migration
- [ ] Verify seed data in SystemSettings
- [ ] Test PDF upload ? extraction ? quiz generation
- [ ] Test DOCX upload ? extraction ? quiz generation
- [ ] Test feature flag toggle
- [ ] Test error scenarios (invalid file, API error, etc.)
- [ ] Review generated quiz quality
- [ ] Monitor OpenAI API costs
- [ ] Set up logging and monitoring
- [ ] Configure rate limiting (optional)
- [ ] Test concurrent job processing

---

## ?? You're All Set!

The AI-powered quiz generation feature is now implemented and ready to use. Follow the setup steps above, test thoroughly, and enjoy automatic quiz creation from course materials!

For questions or issues, refer to the troubleshooting section or review the generated code comments.
