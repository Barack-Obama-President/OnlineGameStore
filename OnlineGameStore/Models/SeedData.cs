using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineGameStore.Data;
using System;
using System.Linq;

namespace OnlineGameStore.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new OnlineGameStoreContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<OnlineGameStoreContext>>()))
            {
                // Look for any movies.
                if (context.Game.Any())
                {
                    return;   // DB has been seeded
                }

                context.Game.AddRange(
                    new Game
                    {
                        Title = "MineCraft",
                        ReleaseDate = DateTime.Parse("200-2-12"),
                        Genre = "Sandbox",
                        Price = 7.99M
                    },

                    new Game
                    {
                        Title = "Doom",
                        ReleaseDate = DateTime.Parse("1984-3-13"),
                        Genre = "FPS",
                        Price = 8.99M
                    },

                    new Game
                    {
                        Title = "Overwatch",
                        ReleaseDate = DateTime.Parse("2010-2-23"),
                        Genre = "Shooter",
                        Price = 9.99M
                    },

                    new Game
                    {
                        Title = "Fortnite",
                        ReleaseDate = DateTime.Parse("2015-4-15"),
                        Genre = "Battle Royale",
                        Price = 3.99M
                    }
                );
                context.SaveChanges();
            }
        }
    }
}