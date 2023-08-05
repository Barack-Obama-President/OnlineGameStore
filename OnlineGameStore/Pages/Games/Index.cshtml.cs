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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace OnlineGameStore.Pages.Games
{
	//[Authorize(Roles = "Admin, Users")]
	public class IndexModel : PageModel
    {
        private readonly OnlineGameStore.Data.OnlineGameStoreContext _context;

        public IndexModel(OnlineGameStore.Data.OnlineGameStoreContext context)
        {
            _context = context;
            ShoppingCart = new Dictionary<int, ShoppingCartItem>(); // Initialize the shopping cart
        }

        public Dictionary<int, ShoppingCartItem> ShoppingCart { get; set; }
        public IList<Game> Game{ get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public SelectList Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string GameGenre { get; set; }
        public decimal TotalPrice { get; set; }


        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (var item in ShoppingCart.Values)
            {
                totalPrice += item.Price * item.Quantity;
            }

            return totalPrice;
        }
        public async Task<IActionResult> OnPostRemoveFromCart(int id)
        {
            var game = await _context.Game.FindAsync(id);

            // Retrieve the existing shopping cart from the session
            var shoppingCartString = HttpContext.Session.GetString("ShoppingCart");

            if (!string.IsNullOrEmpty(shoppingCartString))
                ShoppingCart = JsonConvert.
                    DeserializeObject<Dictionary<int, ShoppingCartItem>>(shoppingCartString);


            // If the game is already in the cart, decrease the quantity
            if (ShoppingCart[game.ID].Quantity > 1)
            {
                ShoppingCart[game.ID].Quantity--;
            }
            else if(ShoppingCart[game.ID].Quantity == 1)
            {
                ShoppingCart.Remove(game.ID);
            }

            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(ShoppingCart));

            // Redirect back to the game page
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostAddToCart(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var cartItem = new ShoppingCartItem
            {
                GameId = game.ID,
                Title = game.Title,
                Price = game.Price,
                Quantity = 1
            };

            // Retrieve the existing shopping cart from the session
            var shoppingCartString = HttpContext.Session.GetString("ShoppingCart");

            if (!string.IsNullOrEmpty(shoppingCartString))
                ShoppingCart = JsonConvert.
                    DeserializeObject<Dictionary<int, ShoppingCartItem>>(shoppingCartString);


            // If the game is already in the cart, increase the quantity
            if (ShoppingCart.ContainsKey(game.ID))
            {
                ShoppingCart[game.ID].Quantity++;
            }
            else
            {
                ShoppingCart.Add(game.ID, cartItem);
            }

            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(ShoppingCart));

            // Redirect back to the game page
            return RedirectToPage("./Index");
        }

        public async Task OnGetAsync()
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from g in _context.Game
                                            orderby g.Genre
                                            select g.Genre;

            var games = from g in _context.Game
                         select g;

            var shoppingCartString = HttpContext.Session.GetString("ShoppingCart");
            ShoppingCart = string.IsNullOrEmpty(shoppingCartString)
                ? new Dictionary<int, ShoppingCartItem>()
                : JsonConvert.DeserializeObject<Dictionary<int, ShoppingCartItem>>(shoppingCartString);

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
            TotalPrice = CalculateTotalPrice();
        }
        public IActionResult OnPostClearCart()
        {
            ShoppingCart.Clear();

            // Update the shopping cart in the session by setting an empty dictionary
            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(new Dictionary<int, ShoppingCartItem>()));

            // Redirect back to the game page
            return RedirectToPage("./Index");
        }

        
    }
}
