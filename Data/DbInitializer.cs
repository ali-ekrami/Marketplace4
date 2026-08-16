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
                "Home & Kitchen"
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
                            "https://placehold.co/600x400?text=Laptop",
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
                            "https://placehold.co/600x400?text=Smartphone",
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
                            "https://placehold.co/600x400?text=T-Shirt",
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
                            "https://placehold.co/600x400?text=Book",
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
                            "https://placehold.co/600x400?text=Coffee+Maker",
                        CategoryId = homeKitchen.Id,
                        SellerId = seller.Id
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}