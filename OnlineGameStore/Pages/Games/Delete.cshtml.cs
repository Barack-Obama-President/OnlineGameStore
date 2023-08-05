using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Data;
using OnlineGameStore.Models;

namespace OnlineGameStore.Pages.Games
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
        public Game Game { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Game = await _context.Game.FirstOrDefaultAsync(m => m.ID == id);

            if (Game == null)
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

            Game = await _context.Game.FindAsync(id);

            if (Game != null)
            {
                _context.Game.Remove(Game);
				// await _context.SaveChangesAsync();
				// Once a record is deleted, create an audit record
				if (await _context.SaveChangesAsync() > 0)
				{
					var auditrecord = new AuditRecord();
					auditrecord.AuditActionType = "Delete Movie Record";
					auditrecord.DateTimeStamp = DateTime.Now;
					auditrecord.KeyGameFieldID = Game.ID;
					var userID = User.Identity.Name.ToString();
					auditrecord.Username = userID;
					_context.AuditRecords.Add(auditrecord);
					await _context.SaveChangesAsync();
				}
			}

            return RedirectToPage("./Index");
        }
    }
}
