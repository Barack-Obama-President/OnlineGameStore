﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineGameStore.Data;
using OnlineGameStore.Models;
using Microsoft.AspNetCore.Authorization;
//testing commit
namespace OnlineGameStore.Pages.Games
{
	[Authorize(Roles = "Admin")]
	public class CreateModel : PageModel
    {
        private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

        public CreateModel(OnlineGameStore.Data.OnlineGameStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Game Game { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Game.Add(Game);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
