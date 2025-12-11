using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PreClear.Api.Data;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class AiService : IAiService
    {
        private readonly IAiRepository _repo;
        private readonly ILogger<AiService> _logger;

        public AiService(IAiRepository repo, ILogger<AiService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<AiResultDto> AnalyzeAsync(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return new AiResultDto { HsCode = string.Empty, Confidence = 0.0, ComplianceRisk = false, Restrictions = Array.Empty<string>(), Suggestions = Array.Empty<string>(), Notes = "empty_input" };
            }

            // Simulate work on a background thread to keep API responsive
            var result = await Task.Run(() => RunHeuristics(description));

            try
            {
                // persist a finding for audit (non-blocking integrity)
                var finding = new AiFinding
                {
                    ShipmentId = 0,
                    Message = $"HS:{result.HsCode}; Risk:{result.ComplianceRisk}; Notes:{result.Notes}",
                    SuggestedAction = result.Suggestions != null && result.Suggestions.Length > 0 ? string.Join(";", result.Suggestions) : null,
                    Details = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                    Severity = result.ComplianceRisk ? Severity.warning : Severity.info
                };

                await _repo.SaveFindingAsync(finding);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist AI finding (non-fatal)");
            }

            return result;
        }

        private AiResultDto RunHeuristics(string text)
        {
            var t = text.ToLowerInvariant();

            // Look for explicit HS code
            var m = Regex.Match(text, "\\b(\\d{6,8})\\b");
            if (m.Success)
            {
                return new AiResultDto
                {
                    HsCode = m.Value,
                    Confidence = 0.98,
                    ComplianceRisk = t.Contains("battery") || t.Contains("lithium") || t.Contains("dangerous"),
                    Restrictions = Array.Empty<string>(),
                    Suggestions = new[] { "Verify origin", "Confirm packing" },
                    Notes = "extracted_explicit_hs"
                };
            }

            string hs = "000000";
            double confidence = 0.4;
            bool risk = false;
            var restrictions = new System.Collections.Generic.List<string>();
            var suggestions = new System.Collections.Generic.List<string>();

            if (t.Contains("laptop") || t.Contains("computer") || t.Contains("electronics"))
            {
                hs = "847130";
                confidence = 0.75;
                suggestions.Add("Classify as electronics");
            }
            else if (t.Contains("shirt") || t.Contains("t-shirt") || t.Contains("clothing"))
            {
                hs = "610910";
                confidence = 0.72;
                suggestions.Add("Provide fiber composition");
            }
            else if (t.Contains("chocolate") || t.Contains("cocoa") || t.Contains("food"))
            {
                hs = "1806";
                confidence = 0.65;
                suggestions.Add("Check sanitary certificates");
            }
            else if (t.Contains("battery") || t.Contains("lithium"))
            {
                hs = "850760";
                confidence = 0.9;
                risk = true;
                restrictions.Add("Dangerous goods declaration required");
                suggestions.Add("Use approved battery packaging");
            }
            else
            {
                hs = "000000";
                confidence = 0.35;
                suggestions.Add("Provide more detailed description");
            }

            if (risk) restrictions.Add("May be restricted or require permits");

            return new AiResultDto
            {
                HsCode = hs,
                Confidence = confidence,
                ComplianceRisk = risk,
                Restrictions = restrictions.ToArray(),
                Suggestions = suggestions.ToArray(),
                Notes = "heuristic_analysis"
            };
        }

        public async Task<AiExtractionResult> ExtractTextAsync(IFormFile file)
        {
            if (file == null)
            {
                return new AiExtractionResult { ExtractedText = string.Empty, SourceFileName = string.Empty, Notes = "no_file" };
            }

            // Read the filename and use a simple deterministic mock extraction
            var fileName = file.FileName ?? "unknown";
            string text;

            // If filename contains keywords, return a representative sample
            var lower = fileName.ToLowerInvariant();
            if (lower.Contains("invoice"))
            {
                text = "Invoice Number: INV-12345\nDate: 2025-12-01\nExporter: ACME Exporters\nTotal Value: USD 12,345.67\nItems: 10 x Widgets";
            }
            else if (lower.Contains("packing") || lower.Contains("packinglist") || lower.Contains("packing-list"))
            {
                text = "Packing List\nPackage Count: 4\nGross Weight: 120 kg\nShipper: ACME Exporters\nConsignee: Global Importers";
            }
            else
            {
                // fallback: generate a short mock text based on file bytes checksum
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var hash = ms.Length > 0 ? (ms.Length % 10000).ToString() : "0";
                text = $"DetectedText: sample_extraction_{hash}\nSourceFile: {fileName}";
            }

            return new AiExtractionResult
            {
                ExtractedText = text,
                SourceFileName = fileName,
                Notes = "mock_extraction"
            };
        }

        public async Task<AiValidationResult> ValidateInvoiceAsync(IFormFile file)
        {
            var extraction = await ExtractTextAsync(file);
            var text = extraction.ExtractedText ?? string.Empty;
            var missing = new List<string>();
            var extracted = new Dictionary<string, string>();

            // Invoice number
            var invMatch = Regex.Match(text, "Invoice Number:\\s*(\\S+)", RegexOptions.IgnoreCase);
            if (invMatch.Success)
            {
                extracted["invoice_number"] = invMatch.Groups[1].Value;
            }
            else missing.Add("invoice_number");

            // Date
            var dateMatch = Regex.Match(text, "Date:\\s*([0-9]{4}-[0-9]{2}-[0-9]{2})", RegexOptions.IgnoreCase);
            if (dateMatch.Success)
            {
                extracted["date"] = dateMatch.Groups[1].Value;
            }
            else missing.Add("date");

            // Exporter
            var exporterMatch = Regex.Match(text, "Exporter:\\s*(.+)", RegexOptions.IgnoreCase);
            if (exporterMatch.Success)
            {
                extracted["exporter"] = exporterMatch.Groups[1].Value.Trim();
            }
            else missing.Add("exporter");

            // Total value
            var valueMatch = Regex.Match(text, "Total Value:\\s*([A-Z]{3}\\s*[0-9,.]+)", RegexOptions.IgnoreCase);
            if (valueMatch.Success)
            {
                extracted["total_value"] = valueMatch.Groups[1].Value;
            }
            else missing.Add("total_value");

            return new AiValidationResult
            {
                IsValid = missing.Count == 0,
                MissingFields = missing.ToArray(),
                ExtractedFields = extracted,
                Notes = "mock_invoice_validation"
            };
        }

        public async Task<AiValidationResult> ValidatePackingListAsync(IFormFile file)
        {
            var extraction = await ExtractTextAsync(file);
            var text = extraction.ExtractedText ?? string.Empty;
            var missing = new List<string>();
            var extracted = new Dictionary<string, string>();

            // Check package count
            var pkgMatch = Regex.Match(text, "Package Count:\\s*(\\d+)", RegexOptions.IgnoreCase);
            if (pkgMatch.Success)
            {
                extracted["package_count"] = pkgMatch.Groups[1].Value;
            }
            else missing.Add("package_count");

            // Gross weight
            var wMatch = Regex.Match(text, "Gross Weight:\\s*([0-9,.]+\\s*kg)", RegexOptions.IgnoreCase);
            if (wMatch.Success)
            {
                extracted["gross_weight"] = wMatch.Groups[1].Value;
            }
            else missing.Add("gross_weight");

            // Shipper
            var shipperMatch = Regex.Match(text, "Shipper:\\s*(.+)", RegexOptions.IgnoreCase);
            if (shipperMatch.Success)
            {
                extracted["shipper"] = shipperMatch.Groups[1].Value.Trim();
            }
            else missing.Add("shipper");

            return new AiValidationResult
            {
                IsValid = missing.Count == 0,
                MissingFields = missing.ToArray(),
                ExtractedFields = extracted,
                Notes = "mock_packinglist_validation"
            };
        }
    }
}

