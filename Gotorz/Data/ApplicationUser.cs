namespace Gotorz.Data;

    using Microsoft.AspNetCore.Identity;


public class ApplicationUser : IdentityUser
    {
    // Du kan udvide denne med flere felter hvis nødvendigt
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string ApplicationUserId { get; set; } = Guid.NewGuid().ToString();

    public Customer Customer { get; set; } // Kun hvis det er en kunde


}

