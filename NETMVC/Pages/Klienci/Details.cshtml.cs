using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETMVC.Data;
using NETMVC.Models;

namespace NETMVC.Pages_Klienci
{
    public class DetailsModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;

        public DetailsModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Klient Klient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var klient = await _context.Klienci.FirstOrDefaultAsync(m => m.id == id);
            if (klient == null)
            {
                return NotFound();
            }
            else
            {
                Klient = klient;
            }
            return Page();
        }
    }
}
