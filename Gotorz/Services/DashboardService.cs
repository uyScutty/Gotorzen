using Gotorz.Data;
using Gotorz.Services;
using Gotorz.ViewModels;
using Gotorz.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly bool useDummyData = true; // ← Skift til false for rigtig data

        // Dummydata
        private readonly List<(string Destination, int SalesCount)> _dummySalesData = new()
        {
            ("Barcelona", 14),
            ("Paris", 22),
            ("New York", 18),
            ("London", 9),
            ("Tokyo", 5)
        };

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalTravelPackagesAsync()
        {
            if (useDummyData)
                return _dummySalesData.Sum(p => p.SalesCount);

            return await _context.TravelPackages.CountAsync();
        }

        public async Task<List<(string Destination, int Count)>> GetTopDestinationsAsync(int top = 3)
        {
            if (useDummyData)
            {
                return _dummySalesData
                    .OrderByDescending(p => p.SalesCount)
                    .Take(top)
                    .Select(p => (p.Destination, p.SalesCount))
                    .ToList();
            }

            return await _context.Bookings
     .Include(b => b.TravelPackage)
     .GroupBy(b => b.TravelPackage.Destination)
     .OrderByDescending(group => group.Count())
     .Take(top)
     .Select(group => new ValueTuple<string, int>(group.Key, group.Count()))
     .ToListAsync();

        }
    }
}

