namespace Gotorz.ViewModels
{
    public class TravelPackageViewModel
    {
        public FlightPackageViewModel FlightPackage { get; set; }
        public HotelPackageViewModel HotelPackage { get; set; }

        // Eventuelt tilføj ekstra info om samlet pris, samlet varighed, mv.
        public string TotalPrice { get; set; } // fx kombineret fly + hotel pris
        public int TotalNights { get; set; }
        public string Destination { get; set; }

        public List<FlightPackageViewModel> AvailableFlights { get; set; } = new();



        public int Id { get; set; } // fx fra database
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public Decimal FinalPrice { get; set; }
            } 
}

