using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Data;
using OnlineGameStore.Models;
using Microsoft.AspNetCore.Authorization;

namespace OnlineGameStore.Pages.Games
{
	[Authorize(Roles = "Admin, Users")]
	public class IndexModel : PageModel
    {
        private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

        public IndexModel(OnlineGameStore.Data.OnlineGameStoreContext context)
        {
            _context = context;
        }

        public IList<Game> Game{ get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public SelectList Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string GameGenre { get; set; }

        public async Task OnGetAsync()
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from g in _context.Game
                                            orderby g.Genre
                                            select g.Genre;

            var games = from g in _context.Game
                         select g;

            if (!string.IsNullOrEmpty(SearchString))
            {
                games = games.Where(s => s.Title.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(GameGenre))
            {
                games = games.Where(x => x.Genre == GameGenre);
            }
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Game = await games.ToListAsync();
        }
    }
}
