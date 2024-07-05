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
using NETMVC.Controllers;

namespace NETMVC.Pages_Klienci
{
    public class IndexModel : PageModel
    {
        private readonly NETMVC.Data.ApplicationDbContext _context;
        private KlienciController apiClient;

        public IndexModel(NETMVC.Data.ApplicationDbContext context)
        {
            _context = context;
            apiClient = new KlienciController(context);
        }

        public IList<Klient> Klient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Klient = await apiClient.IndexGet();
        }
    }
}
