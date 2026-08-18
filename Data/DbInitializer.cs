using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tagr.Models;

namespace tagr.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var context =
                serviceProvider.GetRequiredService<ApplicationDbContext>();

            // =====================================================
            // 1. Create Application Roles
            // =====================================================

            string[] roles = { "Admin", "Seller", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            // =====================================================
            // 2. Create Default Admin
            // =====================================================

            var adminEmail = "admin@marketplace.com";

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(
                        adminUser,
                        "Admin@123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin");
                }
            }

            // =====================================================
            // 3. Create Demo Seller
            // =====================================================

            var sellerEmail = "seller@marketplace.com";

            var seller =
                await userManager.FindByEmailAsync(sellerEmail);

            if (seller == null)
            {
                seller = new ApplicationUser
                {
                    UserName = sellerEmail,
                    Email = sellerEmail,
                    FullName = "Demo Seller",
                    EmailConfirmed = true,
                    IsSellerApproved = true,
                    IsSuspended = false
                };

                var result =
                    await userManager.CreateAsync(
                        seller,
                        "Seller@123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        seller,
                        "Seller");
                }
            }

            // =====================================================
            // 4. Create Categories
            // =====================================================

            var categoryNames = new[]
            {
                "Electronics",
                "Fashion",
                "Books",
                "Home & Kitchen",
                "Sports & Outdoors",
                "Toys & Games"
            };

            foreach (var categoryName in categoryNames)
            {
                var exists = await context.Categories
                    .AnyAsync(c => c.Name == categoryName);

                if (!exists)
                {
                    context.Categories.Add(
                        new Category
                        {
                            Name = categoryName
                        });
                }
            }

            await context.SaveChangesAsync();

            // =====================================================
            // 5. Create Demo Products
            // =====================================================

            var hasProducts =
                await context.Products.AnyAsync();

            if (!hasProducts && seller != null)
            {
                var electronics =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Electronics");

                var fashion =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Fashion");

                var books =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Books");

                var homeKitchen =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Home & Kitchen");

                var sports =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Sports & Outdoors");

                var toys =
                    await context.Categories
                        .FirstAsync(c => c.Name == "Toys & Games");

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Laptop",
                        Description =
                            "High performance laptop suitable for work and study.",
                        Price = 999.99m,
                        StockQuantity = 10,
                        ImageUrl =
                            "https://loremflickr.com/600/400/laptop",
                        CategoryId = electronics.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Smartphone",
                        Description =
                            "Modern smartphone with excellent performance.",
                        Price = 699.99m,
                        StockQuantity = 15,
                        ImageUrl =
                            "https://loremflickr.com/600/400/smartphone",
                        CategoryId = electronics.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Classic T-Shirt",
                        Description =
                            "Comfortable cotton T-shirt for everyday use.",
                        Price = 24.99m,
                        StockQuantity = 30,
                        ImageUrl =
                            "https://loremflickr.com/600/400/tshirt,fashion",
                        CategoryId = fashion.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Programming Book",
                        Description =
                            "A practical book for learning modern programming.",
                        Price = 39.99m,
                        StockQuantity = 20,
                        ImageUrl =
                            "https://loremflickr.com/600/400/book,programming",
                        CategoryId = books.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Coffee Maker",
                        Description =
                            "Easy-to-use coffee maker for your home.",
                        Price = 89.99m,
                        StockQuantity = 8,
                        ImageUrl =
                            "https://loremflickr.com/600/400/coffeemaker",
                        CategoryId = homeKitchen.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Wireless Headphones",
                        Description =
                            "Over-ear wireless headphones with noise cancellation.",
                        Price = 129.99m,
                        StockQuantity = 25,
                        ImageUrl =
                            "https://loremflickr.com/600/400/headphones",
                        CategoryId = electronics.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Smart Watch",
                        Description =
                            "Fitness tracking smart watch with heart rate monitor.",
                        Price = 199.99m,
                        StockQuantity = 12,
                        ImageUrl =
                            "https://loremflickr.com/600/400/smartwatch",
                        CategoryId = electronics.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Denim Jacket",
                        Description =
                            "Classic denim jacket suitable for all seasons.",
                        Price = 59.99m,
                        StockQuantity = 18,
                        ImageUrl =
                            "https://loremflickr.com/600/400/denimjacket",
                        CategoryId = fashion.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Running Shoes",
                        Description =
                            "Lightweight running shoes with breathable mesh.",
                        Price = 74.99m,
                        StockQuantity = 22,
                        ImageUrl =
                            "https://loremflickr.com/600/400/runningshoes",
                        CategoryId = fashion.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Novel Collection",
                        Description =
                            "A collection of bestselling fiction novels.",
                        Price = 29.99m,
                        StockQuantity = 25,
                        ImageUrl =
                            "https://loremflickr.com/600/400/novel,books",
                        CategoryId = books.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Cookbook",
                        Description =
                            "A collection of easy and delicious recipes.",
                        Price = 19.99m,
                        StockQuantity = 16,
                        ImageUrl =
                            "https://loremflickr.com/600/400/cookbook",
                        CategoryId = books.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Blender",
                        Description =
                            "High-speed blender for smoothies and shakes.",
                        Price = 49.99m,
                        StockQuantity = 14,
                        ImageUrl =
                            "https://loremflickr.com/600/400/blender",
                        CategoryId = homeKitchen.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Cookware Set",
                        Description =
                            "Non-stick cookware set for everyday cooking.",
                        Price = 119.99m,
                        StockQuantity = 9,
                        ImageUrl =
                            "https://loremflickr.com/600/400/cookware",
                        CategoryId = homeKitchen.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Yoga Mat",
                        Description =
                            "Non-slip yoga mat suitable for all workouts.",
                        Price = 27.99m,
                        StockQuantity = 40,
                        ImageUrl =
                            "https://loremflickr.com/600/400/yogamat",
                        CategoryId = sports.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Dumbbell Set",
                        Description =
                            "Adjustable dumbbell set for home workouts.",
                        Price = 89.99m,
                        StockQuantity = 11,
                        ImageUrl =
                            "https://loremflickr.com/600/400/dumbbell",
                        CategoryId = sports.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Camping Tent",
                        Description =
                            "Waterproof camping tent for 2-4 people.",
                        Price = 149.99m,
                        StockQuantity = 7,
                        ImageUrl =
                            "https://loremflickr.com/600/400/campingtent",
                        CategoryId = sports.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Building Blocks Set",
                        Description =
                            "Creative building blocks set for kids.",
                        Price = 34.99m,
                        StockQuantity = 26,
                        ImageUrl =
                            "https://loremflickr.com/600/400/buildingblocks,toy",
                        CategoryId = toys.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Remote Control Car",
                        Description =
                            "Fast remote control car with rechargeable battery.",
                        Price = 44.99m,
                        StockQuantity = 13,
                        ImageUrl =
                            "https://loremflickr.com/600/400/rccar,toy",
                        CategoryId = toys.Id,
                        SellerId = seller.Id
                    },

                    new Product
                    {
                        Name = "Board Game",
                        Description =
                            "Fun strategy board game for the whole family.",
                        Price = 24.99m,
                        StockQuantity = 20,
                        ImageUrl =
                            "https://loremflickr.com/600/400/boardgame",
                        CategoryId = toys.Id,
                        SellerId = seller.Id
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}