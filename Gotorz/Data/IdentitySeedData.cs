using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gotorz.Data
{
    public static class IdentitySeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Sales", "Customer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Opret en standard admin bruger
            var adminEmail = "admin@gotorz.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }

      

                    // ✅ Opret kunde uanset om admin eksisterer
                    var customerMail = "Kunde@gmail.com";
                    var customerUser = await userManager.FindByEmailAsync(customerMail);

                    if (customerUser == null)
                    {
                        var customer = new ApplicationUser
                        {
                            UserName = customerMail,
                            Email = customerMail,
                            EmailConfirmed = true
                        };
                        var resultCustomer = await userManager.CreateAsync(customer, "Kunde123!");
                        if (resultCustomer.Succeeded)
                        {
                            await userManager.AddToRoleAsync(customer, "Customer");
                        }
                    }


                }
            }
        }
    }

