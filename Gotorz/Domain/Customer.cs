using Gotorz.Data;

public class Customer
{
    public string CustomerId { get; set; } = Guid.NewGuid().ToString();

    public string ApplicationUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string CustomerFirstName { get; set; } = string.Empty;
    public string CustomerLastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public ApplicationUser ApplicationUser { get; set; } = null!;
}
