using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETMVC.Data;
using NETMVC.Models;

namespace NETMVC.Pages_Klienci
{
    public class CreateModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;

        public CreateModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Klient Klient { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Klienci.Add(Klient);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}