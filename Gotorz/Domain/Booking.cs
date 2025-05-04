namespace Gotorz.Domain
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int TravelPackageId { get; set; }
        public TravelPackage TravelPackage { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;

    }
}