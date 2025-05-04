using Gotorz.Data;
using Gotorz.Providers;
using Gotorz.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Database + IdentityDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) IdentityCore + EF Store + SignInManager
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 3) Cookie-baseret authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

// 4) Blazor services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 5) Din egen AuthenticationStateProvider og sessionlagring
builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddAuthorizationCore();




// 6) Egne services (rækkefølge er vigtig!)
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<HotelApiService>(); // 👈 Denne skal først!
builder.Services.AddScoped<FlightApiService>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<TravelpackageService>(); // 👈 Denne afhænger af HotelApiService

// 7) HttpClient opsætning til kald ud af systemet
builder.Services.AddHttpClient("GotorzApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7292");
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("GotorzApi"));

// 8) Middleware pipeline
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeedData.InitializeAsync(services);
    var context = services.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // 👈 nødvendig for API-endpoints som /auth/logout
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
