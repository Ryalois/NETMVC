using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETMVC.Controllers;
using NETMVC.Models;

namespace NETMVC.Pages_Klienci
{
    public class DeleteModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;
        private KlienciController apiClient;

        public DeleteModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
            apiClient = new KlienciController(context);
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            await apiClient.Delete(id);
            return RedirectToPage("./Index");
        }
    }
}
