using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineGameStore.Models;

namespace OnlineGameStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
		private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

		public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger,OnlineGameStore.Data.OnlineGameStoreContext
context)
        {
            _signInManager = signInManager;
            _logger = logger;
			_context = context;
		}

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				// Login failed attempt
				//-create an audit record
				var auditrecord = new AuditRecord();
				auditrecord.AuditActionType = "User Logged Out";
				auditrecord.DateTimeStamp = DateTime.Now;
				auditrecord.KeyGameFieldID = 999;

				
				// save the email used for the failed login
				_context.AuditRecords.Add(auditrecord);
				await _context.SaveChangesAsync();
				return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
