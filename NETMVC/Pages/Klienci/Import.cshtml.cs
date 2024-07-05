using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETMVC.Controllers;
using NETMVC.Models;
using OfficeOpenXml;
using System.Globalization;
using System.IO;


namespace NETMVC.Pages.Klienci
{
    public class ImportModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;
        private KlienciController apiClient;

        public ImportModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
            apiClient = new KlienciController(context);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        public async Task<IActionResult> OnPostImportAsync()
        {

            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                return BadRequest("Please select a file to upload.");
            }

            byte[] contents;
            using (var memoryStream = new MemoryStream())
            {
                await UploadedFile.CopyToAsync(memoryStream);
                contents = memoryStream.ToArray();
            }

            var fileExtension = Path.GetExtension(UploadedFile.FileName).ToLower();

            var result = await apiClient.ImportAsync(contents, fileExtension);

            if (result is OkResult)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("UploadedFile", result.ToString());
                return Page();
            }
        }
        public async Task<IActionResult> OnPostExportCSVAsync()
        {
            return File(await apiClient.ExportCsvAsync(), "text/csv", "klienci_export.csv");
        }

        public async Task<IActionResult> OnPostExportXLSXAsync()
        {
            return File(await apiClient.ExportExcelAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "klienci_export.xlsx");
        }
    }
}
