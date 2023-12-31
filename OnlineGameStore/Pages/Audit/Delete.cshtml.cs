﻿using System;
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
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

        public DeleteModel(OnlineGameStore.Data.OnlineGameStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuditRecord AuditRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AuditRecord = await _context.AuditRecords.FirstOrDefaultAsync(m => m.Audit_ID == id);

            if (AuditRecord == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AuditRecord = await _context.AuditRecords.FindAsync(id);

			if (AuditRecord != null)
			{
				_context.AuditRecords.Remove(AuditRecord);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
