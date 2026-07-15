using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
// Not "using ...Listener;" - iText's own TextChunk type collides with this
// project's TextChunk DTO used elsewhere in this file.
using SimpleTextExtractionStrategy = iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.TextExtraction;

/// <summary>
/// Service for extracting text from various document formats.
/// Requires the following NuGet packages:
/// - itext7 (for PDF extraction)
/// - DocumentFormat.OpenXml (for DOCX/PPTX extraction)
/// </summary>
public class TextExtractionService : ITextExtractionService
{
    private readonly ILogger<TextExtractionService> _logger;
    private static readonly HashSet<string> SupportedExtensions = new() { ".pdf", ".docx", ".pptx" };

    public TextExtractionService(ILogger<TextExtractionService> logger)
    {
        _logger = logger;
    }

    public bool SupportsFileType(string extension)
    {
        return SupportedExtensions.Contains(extension?.ToLowerInvariant() ?? "");
    }

    public async Task<Result<string>> ExtractTextAsync(
        Stream fileStream,
        string extension,
        CancellationToken cancellationToken = default)
    {
        if (fileStream == null)
            return Error.Validation("File.Invalid", "File stream cannot be null");

        if (!SupportsFileType(extension))
            return Error.Validation("File.UnsupportedType", $"File type '{extension}' is not supported for text extraction");

        try
        {
            var normalizedExtension = extension.ToLowerInvariant();

            string extractedText = normalizedExtension switch
            {
                ".pdf" => await ExtractFromPdfAsync(fileStream, cancellationToken),
                ".docx" => await ExtractFromDocxAsync(fileStream, cancellationToken),
                ".pptx" => await ExtractFromPptxAsync(fileStream, cancellationToken),
                _ => throw new NotSupportedException($"Extension {extension} not supported")
            };

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                return Error.Validation("TextExtraction.NoText", "No text could be extracted from the document");
            }

            // Clean the extracted text
            var cleanedText = CleanExtractedText(extractedText);

            _logger.LogInformation(
                "Text extracted successfully. Extension={Extension}, Length={Length}",
                extension, cleanedText.Length);

            return cleanedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from {Extension} file", extension);
            return Error.Failure("TextExtraction.Failed", $"Failed to extract text: {ex.Message}");
        }
    }

    public async Task<Result<ExtractedTextDto>> ExtractTextWithMetadataAsync(
        Stream fileStream,
        string extension,
        CancellationToken cancellationToken = default)
    {
        var textResult = await ExtractTextAsync(fileStream, extension, cancellationToken);

        if (textResult.IsError)
            return textResult.Errors;

        var text = textResult.Value;

        return new ExtractedTextDto
        {
            Text = text,
            CharacterCount = text.Length,
            DetectedLanguage = DetectLanguage(text),
            DetectedTopics = null // Could be enhanced with AI topic extraction
        };
    }

    public async Task<List<TextChunk>> ChunkTextIntelligentlyAsync(
        string text,
        ChunkingOptions options,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<TextChunk>();

        var chunks = new List<TextChunk>();

        // Try section-based splitting first
        if (options.SplitBySection)
        {
            var sectionChunks = SplitBySections(text);
            if (sectionChunks.Count > 1)
            {
                _logger.LogInformation("Split text by {Count} detected sections", sectionChunks.Count);
                return sectionChunks.Select((chunk, index) => new TextChunk
                {
                    Text = chunk,
                    StartIndex = 0,
                    EndIndex = chunk.Length,
                    DetectedTopic = ExtractTopic(chunk)
                }).ToList();
            }
        }

        // Fallback to character-based splitting with sentence boundaries
        return await Task.Run(() => SplitByCharacterCount(text, options), cancellationToken);
    }

    #region PDF Extraction

    private Task<string> ExtractFromPdfAsync(Stream stream, CancellationToken cancellationToken)
    {
        // PDF parsing is CPU-bound and iText7's API is synchronous, so this runs
        // on a thread-pool thread rather than blocking the caller.
        return Task.Run(() =>
        {
            using var pdfReader = new PdfReader(stream);
            using var pdfDocument = new PdfDocument(pdfReader);
            var text = new StringBuilder();

            for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var page = pdfDocument.GetPage(pageNum);
                var strategy = new SimpleTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                text.AppendLine(pageText);
            }

            return text.ToString();
        }, cancellationToken);
    }

    #endregion

    #region DOCX Extraction

    private Task<string> ExtractFromDocxAsync(Stream stream, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            using var document = WordprocessingDocument.Open(stream, false);
            var body = document.MainDocumentPart?.Document.Body;

            if (body == null)
                return string.Empty;

            var text = new StringBuilder();
            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                text.AppendLine(paragraph.InnerText);
            }

            return text.ToString();
        }, cancellationToken);
    }

    #endregion

    #region PPTX Extraction

    private Task<string> ExtractFromPptxAsync(Stream stream, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            using var presentation = PresentationDocument.Open(stream, false);
            var presentationPart = presentation.PresentationPart;
            var text = new StringBuilder();

            if (presentationPart?.SlideParts != null)
            {
                foreach (var slidePart in presentationPart.SlideParts)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var slide = slidePart.Slide;
                    foreach (var paragraph in slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        text.AppendLine(paragraph.InnerText);
                    }
                }
            }

            return text.ToString();
        }, cancellationToken);
    }

    #endregion

    #region Text Processing Helpers

    private string CleanExtractedText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remove excessive whitespace
        text = Regex.Replace(text, @"\s+", " ");

        // Remove control characters except newlines and tabs
        text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

        // Normalize line endings
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        // Remove multiple consecutive newlines
        text = Regex.Replace(text, @"\n{3,}", "\n\n");

        return text.Trim();
    }

    private List<string> SplitBySections(string text)
    {
        // Look for common section markers: headings, chapters, etc.
        var sectionPattern = @"(?:^|\n)(?:Chapter|Section|Part|Unit)\s+\d+|(?:^|\n)[A-Z][^\n]{10,80}\n";
        var matches = Regex.Matches(text, sectionPattern, RegexOptions.Multiline);

        if (matches.Count < 2)
            return new List<string> { text };

        var sections = new List<string>();
        for (int i = 0; i < matches.Count; i++)
        {
            var startIndex = matches[i].Index;
            var endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : text.Length;
            var section = text.Substring(startIndex, endIndex - startIndex).Trim();

            if (!string.IsNullOrWhiteSpace(section))
                sections.Add(section);
        }

        return sections.Count > 0 ? sections : new List<string> { text };
    }

    private List<TextChunk> SplitByCharacterCount(string text, ChunkingOptions options)
    {
        var chunks = new List<TextChunk>();
        var sentences = SplitIntoSentences(text);
        var currentChunk = new StringBuilder();
        var chunkStartIndex = 0;

        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > options.MaxCharsPerChunk 
                && currentChunk.Length >= options.MinCharsPerChunk)
            {
                var chunkText = currentChunk.ToString().Trim();
                chunks.Add(new TextChunk
                {
                    Text = chunkText,
                    StartIndex = chunkStartIndex,
                    EndIndex = chunkStartIndex + chunkText.Length,
                    DetectedTopic = ExtractTopic(chunkText)
                });

                chunkStartIndex += chunkText.Length;
                currentChunk.Clear();

                // Add overlap if configured
                if (options.OverlapPercentage > 0)
                {
                    var overlapSize = (int)(sentence.Length * options.OverlapPercentage / 100.0);
                    if (overlapSize > 0 && sentence.Length > overlapSize)
                    {
                        currentChunk.Append(sentence.Substring(sentence.Length - overlapSize));
                    }
                }
            }

            currentChunk.Append(sentence).Append(" ");
        }

        // Add remaining text as last chunk
        if (currentChunk.Length > 0)
        {
            var chunkText = currentChunk.ToString().Trim();
            chunks.Add(new TextChunk
            {
                Text = chunkText,
                StartIndex = chunkStartIndex,
                EndIndex = chunkStartIndex + chunkText.Length,
                DetectedTopic = ExtractTopic(chunkText)
            });
        }

        return chunks;
    }

    private List<string> SplitIntoSentences(string text)
    {
        // Simple sentence splitting (can be enhanced with more sophisticated NLP)
        var sentencePattern = @"(?<=[.!?])\s+(?=[A-Z])";
        var sentences = Regex.Split(text, sentencePattern)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return sentences.Count > 0 ? sentences : new List<string> { text };
    }

    private string? ExtractTopic(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length < 50)
            return null;

        // Extract first line or first sentence as potential topic
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var firstLine = lines.FirstOrDefault()?.Trim();

        if (firstLine != null && firstLine.Length < 100)
            return firstLine;

        return null;
    }

    private string? DetectLanguage(string text)
    {
        // Simple language detection (can be enhanced with a proper library)
        // For now, just return "en" as default
        return "en";
    }

    #endregion
}
