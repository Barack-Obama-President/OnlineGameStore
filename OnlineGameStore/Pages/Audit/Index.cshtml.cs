using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Data;
using OnlineGameStore.Models;

namespace OnlineGameStore.Pages.Audit
{
    //[Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

        public IndexModel(OnlineGameStore.Data.OnlineGameStoreContext context)
        {
            _context = context;
        }

        public IList<AuditRecord> AuditRecord { get;set; }

        public async Task OnGetAsync()
        {
            AuditRecord = await _context.AuditRecords.ToListAsync();
        }
    }
}
