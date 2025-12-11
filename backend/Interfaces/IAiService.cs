namespace PreClear.Api.Interfaces
{
    public interface IAiService
    {
        System.Threading.Tasks.Task<PreClear.Api.Models.AiResultDto> AnalyzeAsync(string description);

        // Extract plain text from an uploaded document (mock implementation)
        System.Threading.Tasks.Task<PreClear.Api.Models.AiExtractionResult> ExtractTextAsync(Microsoft.AspNetCore.Http.IFormFile file);

        // Validate an invoice document (mock checks)
        System.Threading.Tasks.Task<PreClear.Api.Models.AiValidationResult> ValidateInvoiceAsync(Microsoft.AspNetCore.Http.IFormFile file);

        // Validate a packing list document (mock checks)
        System.Threading.Tasks.Task<PreClear.Api.Models.AiValidationResult> ValidatePackingListAsync(Microsoft.AspNetCore.Http.IFormFile file);
    }
}
