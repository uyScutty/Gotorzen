// ViewModels/HotelFormViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Gotorz.ViewModels
{
    public class HotelFormViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string ApiHotelId { get; set; } = string.Empty;
    }
}
