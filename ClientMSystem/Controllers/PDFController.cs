using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClientMSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        private readonly ChromePdfRenderer _pdfRenderer;

        public PDFController()
        {
            _pdfRenderer = new ChromePdfRenderer();
        }

        /// <summary>
        /// Converts a Razor View into a PDF file and saves it.
        /// </summary>
        [HttpGet("generate-from-view")]
        public async Task<IActionResult> GeneratePdfFromView()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "TaskSheet", "Create.cshtml");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("The specified view file does not exist.");
                }

                string htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
                using var pdf = _pdfRenderer.RenderHtmlAsPdf(htmlContent);
                
                string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles", "TaskSheet.pdf");
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)); // Ensure directory exists
                pdf.SaveAs(outputFilePath);

                return Ok(new { Message = "PDF generated successfully", FilePath = outputFilePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a PDF from a raw HTML string.
        /// </summary>
        [HttpPost("generate-from-html")]
        public IActionResult GeneratePdfFromHtml([FromBody] string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                return BadRequest("HTML content cannot be empty.");
            }

            try
            {
                using var pdf = _pdfRenderer.RenderHtmlAsPdf(htmlContent);
                byte[] pdfBytes = pdf.BinaryData;

                return File(pdfBytes, "application/pdf", "GeneratedDocument.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a PDF from a webpage URL.
        /// </summary>
        [HttpGet("generate-from-url")]
        public IActionResult GeneratePdfFromUrl([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("URL cannot be empty.");
            }

            try
            {
                using var pdf = _pdfRenderer.RenderUrlAsPdf(url);
                byte[] pdfBytes = pdf.BinaryData;

                return File(pdfBytes, "application/pdf", "Webpage.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Downloads a previously generated PDF.
        /// </summary>
        [HttpGet("download")]
        public IActionResult DownloadGeneratedPdf([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("File name cannot be empty.");
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Requested file not found.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
