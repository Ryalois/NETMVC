using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETMVC.Models;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Text;


namespace NETMVC.Pages.Klienci
{
    public class ImportModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;

        public ImportModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
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
                ModelState.AddModelError("UploadedFile", "Please select a file to upload.");
                return Page();
            }

            var fileExtension = Path.GetExtension(UploadedFile.FileName).ToLower();
            List<Klient> klienci;

            if (fileExtension == ".csv")
            {
                klienci = await ImportFromCsv();
            }
            else if (fileExtension == ".xlsx")
            {
                klienci = await ImportFromExcel();
            }
            else
            {
                ModelState.AddModelError("UploadedFile", "Unsupported file format. Please upload a CSV or XLSX file.");
                return Page();
            }

            if (klienci.Any())
            {
                await _context.Klienci.AddRangeAsync(klienci);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("UploadedFile", "The file is empty or improperly formatted.");
                return Page();
            }
        }

        private async Task<List<Klient>> ImportFromCsv()
        {
            var klienci = new List<Klient>();

            using (var reader = new StreamReader(UploadedFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                klienci = csv.GetRecords<Klient>().ToList();
                foreach (Klient klient in klienci)
                {
                    klient.id = 0;
                }
            }
            return klienci;
        }

        private async Task<List<Klient>> ImportFromExcel()
        {
            var klienci = new List<Klient>();
            using var stream = new MemoryStream();
            await UploadedFile.CopyToAsync(stream);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Assuming first row is header
            {
                klienci.Add(new Klient
                {
                    name = worksheet.Cells[row, 1].Value?.ToString(),
                    surname = worksheet.Cells[row, 2].Value?.ToString(),
                    pesel = worksheet.Cells[row, 3].Value?.ToString(),
                    birthyear = Int32.Parse(worksheet.Cells[row, 4].Value?.ToString()),
                    p³eæ = Int32.Parse(worksheet.Cells[row, 5].Value?.ToString())
                });
            }
            return klienci;
        }
        /*public async Task<IActionResult> OnPostImportAsync()
        {
            if (csv == null || csv.Length == 0)
            {
                ModelState.AddModelError("CsvFile", "Please select a CSV file to upload.");
                Console.WriteLine("Stupid");
                return Page();
            }

            var klienci = new List<Klient>();

            using (var reader = new StreamReader(csv.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                klienci = csv.GetRecords<Klient>().ToList();
                foreach( Klient klient in klienci )
                {
                    klient.id = 0;
                }
            }

            if (klienci.Any())
            {
                await _context.Klienci.AddRangeAsync(klienci);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("CsvFile", "The CSV file is empty or improperly formatted.");
                Console.WriteLine("More Stupid");
                return Page();
            }
        }*/

        public async Task<IActionResult> OnPostExportCSVAsync()
        {
            var klienci = await _context.Klienci.ToListAsync();

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(klienci);
                await writer.FlushAsync();

                var content = memoryStream.ToArray();
                return File(content, "text/csv", "klienci_export.csv");
            }
        }

        public async Task<IActionResult> OnPostExportXLSXAsync()
        {
            var klienci = await _context.Klienci.ToListAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Klienci");

            // Add headers
            worksheet.Cells[1, 1].Value = "name";
            worksheet.Cells[1, 2].Value = "surname";
            worksheet.Cells[1, 3].Value = "pesel";
            worksheet.Cells[1, 4].Value = "birthyear";
            worksheet.Cells[1, 5].Value = "p³eæ";

            // Add data
            for (int i = 0; i < klienci.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = klienci[i].name;
                worksheet.Cells[i + 2, 2].Value = klienci[i].surname;
                worksheet.Cells[i + 2, 3].Value = klienci[i].pesel;
                worksheet.Cells[i + 2, 4].Value = klienci[i].birthyear;
                worksheet.Cells[i + 2, 5].Value = klienci[i].p³eæ;
            }

            var content = await package.GetAsByteArrayAsync();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "klienci_export.xlsx");
        }
    }
}
