using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETMVC.Controllers;
using NETMVC.Data;
using NETMVC.Models;

namespace NETMVC.Pages_Klienci
{
    public class DetailsModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;
        private KlienciController apiClient;

        public DetailsModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
            apiClient = new KlienciController(context);
        }

        public Klient Klient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var klient = await apiClient.DetailsGet(id);
            if (klient == null)
            {
                return NotFound();
            }
            Klient = klient;
            return Page();
        }
    }
}
