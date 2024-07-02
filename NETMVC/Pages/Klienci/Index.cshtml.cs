using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETMVC.Data;
using NETMVC.Models;
using CsvHelper;

namespace NETMVC.Pages_Klienci
{
    public class IndexModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;

        public IndexModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Klient> Klient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Klient = await _context.Klienci.ToListAsync();
        }

        public void OnPostAsync()
        {
            if( Klient != null )
            {
                Console.WriteLine(Klient);
            }
        }
    }
}
