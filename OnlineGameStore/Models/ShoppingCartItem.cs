namespace OnlineGameStore.Models
{
    public class ShoppingCartItem
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } // Add this property to represent the quantity of the game in the cart

    }

}
