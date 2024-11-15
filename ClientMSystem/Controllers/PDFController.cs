using IronPdf;
using Microsoft.AspNetCore.Mvc;

namespace ClientMSystem.Controllers
{
    public class PDFController : Controller
    {
        public IActionResult Index()

        {
            var html = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\prajakta.chavan\\Downloads\\ClientMSystem\\ClientMSystem\\Views\\TaskSheet\\Create.cshtml"));
            var ChromePdfRenderer = new ChromePdfRenderer();
            using var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
            pdf.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), "Record.Pdf"));
            
            return View();
        }

        //var html = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\prajakta.chavan\\source\\repos\\IronApp\\IronApp\\Class1.cshtml"));
        //var ChromePdfRenderer = new ChromePdfRenderer();
        //    using var pdf = ChromePdfRenderer.RenderHtmlAsPdf(html);
        //    pdf.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), "ChromePdfRendererExample1.Pdf"));
    }
}
