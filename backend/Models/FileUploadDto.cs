using Microsoft.AspNetCore.Http;

namespace PreClear.Api.Models
{
    public class ExtractTextRequest
    {
        public IFormFile File { get; set; }
    }

    public class ValidateInvoiceRequest
    {
        public IFormFile File { get; set; }
    }

    public class ValidatePackingListRequest
    {
        public IFormFile File { get; set; }
    }
}
