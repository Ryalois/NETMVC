using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETMVC.Controllers;
using NETMVC.Data;
using NETMVC.Models;

namespace NETMVC.Pages_Klienci
{
    public class CreateModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;
        private KlienciController apiClient;

        public CreateModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
            apiClient = new KlienciController(context);
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

            await apiClient.Create(Klient);
            return RedirectToPage("./Index");
        }
    }
}