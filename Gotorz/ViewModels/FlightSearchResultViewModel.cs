using Gotorz.DTOs;

namespace Gotorz.ViewModels
{
    public class FlightSearchResultViewModel
    {
        public List<FlightPackageViewModel> AvailableFlights { get; set; } = new();
    }
}
