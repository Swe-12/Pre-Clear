namespace PreClear.Api.Models
{
    public class AiExtractionResult
    {
        public string ExtractedText { get; set; } = string.Empty;
        public string SourceFileName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
