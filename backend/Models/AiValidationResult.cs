using System.Collections.Generic;

namespace PreClear.Api.Models
{
    public class AiValidationResult
    {
        public bool IsValid { get; set; }
        public string[] MissingFields { get; set; } = new string[0];
        public Dictionary<string, string> ExtractedFields { get; set; } = new Dictionary<string, string>();
        public string Notes { get; set; } = string.Empty;
    }
}
