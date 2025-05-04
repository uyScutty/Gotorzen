using Gotorz.Data;
using Gotorz.Domain;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (!context.Hotels.Any())
        {
            context.Hotels.AddRange(
                new Hotel
                {
                    Name = "Hotel Copenhagen",
                    Address = "Main Street 1",
                    City = "Copenhagen",
                    Country = "Denmark",
                   
                },
                new Hotel
                {
                    Name = "Hotel Paris",
                    Address = "Champs-Elysées",
                    City = "Paris",
                    Country = "France",
               
                }
            );

            context.SaveChanges();
        }
    }
}
