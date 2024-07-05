using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETMVC.Data;
using NETMVC.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using OfficeOpenXml;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Text;

namespace NETMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlienciController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KlienciController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("create")]
        public async Task Create(Klient klient)
        {
            _context.Klienci.Add(klient);
            await _context.SaveChangesAsync();
            return;
        }

        [HttpGet("details")]
        public async Task<Klient?> DetailsGet(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var klient = await _context.Klienci.FirstOrDefaultAsync(m => m.id == id);
            if (klient == null)
            {
                return null;
            }
            return klient;
        }

        [HttpGet("edit")]
        public async Task<IActionResult> Edit(Klient klient)
        {
            _context.Attach(klient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Klienci.Any(e => e.id == klient.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        [HttpGet("delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klienci.FindAsync(id);
            if (klient != null)
            {
                _context.Klienci.Remove(klient);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("index")]
        public async Task<IList<Klient>> IndexGet()
        {
            return await _context.Klienci.ToListAsync();
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportAsync([FromForm] byte[] uploadedFile, String fileExtension)
        {
            List<Klient> klienci;

            if (fileExtension == ".csv")
            {
                klienci = ImportFromCsvAsync(uploadedFile);
            }
            else if (fileExtension == ".xlsx")
            {
                klienci = ImportFromExcelAsync(uploadedFile);
            }
            else
            {
                return BadRequest("Unsupported file format. Please upload a CSV or XLSX file.");
            }

            if (klienci.Any())
            {
                await _context.Klienci.AddRangeAsync(klienci);
                await _context.SaveChangesAsync();
                return Ok("Import successful.");
            }
            else
            {
                return BadRequest("The file is empty or improperly formatted.");
            }
        }

        [HttpPost("import/csv")]
        private List<Klient> ImportFromCsvAsync(byte[] contents)
        {
            var klienci = new List<Klient>();
            using (var memoryStream = new MemoryStream(contents))
            using (var reader = new StreamReader(memoryStream))
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

        [HttpPost("import/xlsx")]
        private List<Klient> ImportFromExcelAsync(byte[] contents)
        {
            var klienci = new List<Klient>();
            using var stream = new MemoryStream(contents);
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
                    birthyear = Int32.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0"),
                    plec = Int32.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0")
                });
            }
            return klienci;
        }

        [HttpGet("export/csv")]
        public async Task<byte[]> ExportCsvAsync()
        {
            var klienci = await _context.Klienci.ToListAsync();

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(klienci);
                await writer.FlushAsync();
                return memoryStream.ToArray();
            }
        }

        [HttpGet("export/xlsx")]
        public async Task<byte[]> ExportExcelAsync()
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
            worksheet.Cells[1, 5].Value = "plec";

            // Add data
            for (int i = 0; i < klienci.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = klienci[i].name;
                worksheet.Cells[i + 2, 2].Value = klienci[i].surname;
                worksheet.Cells[i + 2, 3].Value = klienci[i].pesel;
                worksheet.Cells[i + 2, 4].Value = klienci[i].birthyear;
                worksheet.Cells[i + 2, 5].Value = klienci[i].plec;
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
