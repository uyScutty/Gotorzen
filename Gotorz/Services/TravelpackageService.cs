using Gotorz.Domain;
using System.Linq;
using Gotorz.DTOs;
using Gotorz.ViewModels;
using Gotorz.Data;
using Microsoft.EntityFrameworkCore;

namespace Gotorz.Services
{
    public class TravelpackageService
    {
        private readonly FlightApiService _flightService;
        private readonly ApplicationDbContext _dbContext;
        private readonly HotelApiService _hotelService;

        public TravelpackageService(FlightApiService flightService, HotelApiService hotelService, ApplicationDbContext dbContext)
        {
            _flightService = flightService;
            _hotelService = hotelService;
            _dbContext = dbContext;
        }

        public async Task SaveTravelPackageAsync(TravelPackageFormViewModel model)
        {
            var selectedHotel = await _dbContext.Hotels.FindAsync(model.SelectedHotelId);

            var travelPackage = new TravelPackage
            {
                Destination = model.Destination,
                DepartureDate = model.DepartureDate,
                ReturnDate = model.ReturnDate,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                BasePrice = model.FinalPrice, // 💥 Brug FinalPrice
                ManualPrice = model.ManualPriceOverride,
                PriceMarkupPercentage = (int)model.MarkupPercent,
                HotelName = selectedHotel?.Name,
                HotelAddress = selectedHotel?.Address,
                FlightInfo = model.FlightPackage != null
                    ? $"{model.FlightPackage.Airline} ({model.FlightPackage.FlightNumber})"
                    : null
            };

            _dbContext.TravelPackages.Add(travelPackage);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<List<TravelPackageViewModel>> GetAllPackagesAsync()
        {
            var packages = await _dbContext.TravelPackages.ToListAsync();

            return packages.Select(p => new TravelPackageViewModel
            {
                Destination = p.Destination,
                TotalPrice = p.FinalPrice.ToString("C"),
                TotalNights = (p.ReturnDate - p.DepartureDate).Days,
                ImageUrl = p.ImageUrl ?? "",
                Description = p.Description ?? "",
                HotelPackage = new HotelPackageViewModel
                {
                    HotelName = p.HotelName ?? "",
                    HotelAddress = p.HotelAddress ?? "",
                    price = p.FinalPrice.ToString("C")
                },
                FlightPackage = new FlightPackageViewModel
                {
                    Airline = "Ukendt",
                    DepartureAirport = new AirportInfo(),
                    ArrivalAirport = new AirportInfo(),
                    Flights = new(),
                    Extras = new()
                }
            }).ToList();
        }

        public async Task<TravelPackageViewModel> CreatePackageAsync(
     string from, string to, DateTime departureDate, DateTime returnDate,
     string checkIn, string checkOut, string currency, string country,
     string hotelId, int adults, int kids, int rooms)
        {
            var flightResponse = await _flightService.SearchFlightsAsync(from, to, departureDate, returnDate, currency);
            if (flightResponse == null || flightResponse.OtherFlights == null)
                return null;

            var selectedHotel = await _dbContext.Hotels.FirstOrDefaultAsync(h => h.ApiHotelId == hotelId);
            if (selectedHotel == null)
                return null;

            var firstFlight = flightResponse.OtherFlights.FirstOrDefault();
            if (firstFlight == null || firstFlight.Flights == null || !firstFlight.Flights.Any())
                return null;

            var firstLeg = firstFlight.Flights.First();
            var lastLeg = firstFlight.Flights.Last();

            var flightViewModel = new FlightPackageViewModel
            {
                Airline = firstLeg.Airline,
                AirlineLogo = firstLeg.AirlineLogo,
                FlightNumber = firstLeg.FlightNumber,
                TravelClass = firstLeg.TravelClass,
                Legroom = firstLeg.Legroom,
                Airplane = firstLeg.Airplane,
                PlaneAndCrewBy = firstLeg.PlaneAndCrewBy,
                Overnight = firstLeg.Overnight,
                Extensions = firstLeg.Extensions?.ToList() ?? new(),
                TicketAlsoSoldBy = firstLeg.TicketAlsoSoldBy?.ToList() ?? new(),
                DepartureTime = firstLeg.DepartureAirport.Time,
                ArrivalTime = lastLeg.ArrivalAirport.Time,
                DepartureAirport = new AirportInfo
                {
                    Id = firstLeg.DepartureAirport.Id,
                    Name = firstLeg.DepartureAirport.Name,
                    Time = firstLeg.DepartureAirport.Time
                },
                ArrivalAirport = new AirportInfo
                {
                    Id = lastLeg.ArrivalAirport.Id,
                    Name = lastLeg.ArrivalAirport.Name,
                    Time = lastLeg.ArrivalAirport.Time
                },
                Duration = firstFlight.TotalDuration,
                Flights = firstFlight.Flights
                    .Select(f => $"{f.DepartureAirport.Id} → {f.ArrivalAirport.Id} ({f.Duration} min)")
                    .ToList(),
                Extras = firstFlight.Flights
                    .Where(f => f.Extensions != null)
                    .SelectMany(f => f.Extensions!)
                    .Distinct()
                    .ToList()
            };

            var hotelViewModel = new HotelPackageViewModel
            {
                HotelName = selectedHotel.Name,
                HotelAddress = selectedHotel.Address,
                RoomType = "Ikke valgt endnu",
                price = "Ikke angivet",
                PaymentDetails = new List<string>(),
                Rooms = new List<string>()
            };

            return new TravelPackageViewModel
            {
                FlightPackage = flightViewModel,
                HotelPackage = hotelViewModel,
                Destination = to,
                TotalNights = (returnDate - departureDate).Days,
                TotalPrice = "Ukendt",
                AvailableFlights = flightResponse.OtherFlights
                    .Where(f => f.Flights.Any())
                    .Select(f =>
                    {
                        var leg = f.Flights.First();
                        var last = f.Flights.Last();

                        return new FlightPackageViewModel
                        {
                            Airline = leg.Airline,
                            AirlineLogo = leg.AirlineLogo,
                            FlightNumber = leg.FlightNumber,
                            TravelClass = leg.TravelClass,
                            Legroom = leg.Legroom,
                            Airplane = leg.Airplane,
                            PlaneAndCrewBy = leg.PlaneAndCrewBy,
                            Overnight = leg.Overnight,
                            Extensions = leg.Extensions?.ToList() ?? new(),
                            TicketAlsoSoldBy = leg.TicketAlsoSoldBy?.ToList() ?? new(),
                            DepartureTime = leg.DepartureAirport.Time,
                            ArrivalTime = last.ArrivalAirport.Time,
                            DepartureAirport = new AirportInfo
                            {
                                Id = leg.DepartureAirport.Id,
                                Name = leg.DepartureAirport.Name,
                                Time = leg.DepartureAirport.Time
                            },
                            ArrivalAirport = new AirportInfo
                            {
                                Id = last.ArrivalAirport.Id,
                                Name = last.ArrivalAirport.Name,
                                Time = last.ArrivalAirport.Time
                            },
                            Duration = f.TotalDuration,
                            Flights = f.Flights
                                .Select(x => $"{x.DepartureAirport.Id} → {x.ArrivalAirport.Id} ({x.Duration} min)")
                                .ToList(),
                            Extras = f.Flights
                                .Where(x => x.Extensions != null)
                                .SelectMany(x => x.Extensions!)
                                .Distinct()
                                .ToList(),
                            Price = f.Price.ToString()
                        };
                    }).ToList()
            };
        }


        public async Task<TravelPackageFormViewModel?> GetPackageForEditAsync(int id)
        {
            var package = await _dbContext.TravelPackages.FindAsync(id);
            if (package == null) return null;

            return new TravelPackageFormViewModel
            {
                Destination = package.Destination,
                DepartureDate = package.DepartureDate,
                ReturnDate = package.ReturnDate,
                Description = package.Description,
                ImageUrl = package.ImageUrl,
                ManualPriceOverride = (decimal)package.ManualPrice,
                MarkupPercent = package.PriceMarkupPercentage,
                AutoCalculatedPrice = package.BasePrice,
                FlightPrice = package.BasePrice.ToString(), // Hvis du vil vise base price som string
                OriginAirport = package.OriginAirport,
                Currency = package.Currency,
                Adults = package.Adults,
                Kids = package.Kids,
                Rooms = package.Rooms
            };
        }



        public async Task UpdateTravelPackageAsync(int id, TravelPackageFormViewModel model)
        {
            var package = await _dbContext.TravelPackages.FindAsync(id);
            if (package == null) return;

            package.Destination = model.Destination;
            package.DepartureDate = model.DepartureDate;
            package.ReturnDate = model.ReturnDate;
            package.Description = model.Description;
            package.ImageUrl = model.ImageUrl;
            package.PriceMarkupPercentage = (int)model.MarkupPercent;
            package.ManualPrice = model.ManualPriceOverride;
            package.BasePrice = model.AutoCalculatedPrice;

            await _dbContext.SaveChangesAsync();
        }
        public async Task DeletePackageAsync(int id)
        {
            var package = await _dbContext.TravelPackages.FindAsync(id);
            if (package != null)
            {
                _dbContext.TravelPackages.Remove(package);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<FlightSearchResultViewModel?> CreateFlightOptionsAsync(
         string origin,
         string destination,
         DateTime departureDate,
         DateTime returnDate,
         string currency)
        {
            var responseDto = await _flightService.SearchFlightsAsync(
                origin,
                destination,
                departureDate,
                returnDate,
                currency);

            if (responseDto == null)
                return null;

            return new FlightSearchResultViewModel
            {
                AvailableFlights = responseDto.OtherFlights
    .Where(f => f.Flights.Any())
    .Select(f =>
    {
        var firstFlight = f.Flights.First();
        var lastFlight = f.Flights.Last();

        return new FlightPackageViewModel
        {
            Airline = firstFlight.Airline,
            DepartureAirport = new AirportInfo { Name = firstFlight.DepartureAirport?.Name },
            ArrivalAirport = new AirportInfo { Name = lastFlight.ArrivalAirport?.Name },
            DepartureTime = firstFlight.DepartureTime,
            ArrivalTime = lastFlight.ArrivalTime,
            FlightNumber = firstFlight.FlightNumber,

            // 🛎️ Ny linje: vis hvor mange stop
            Extras = f.Flights.Count > 1
                ? new List<string> { $"{f.Flights.Count - 1} stop(s)" }
                : new List<string>(),

            Price = f.Price.ToString()
        };
    })
    .ToList()

            };
        }



        }
    }


