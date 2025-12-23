using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PrayerTrackerWebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// This is equivalent to ConfigureServices in older versions.
builder.Services.AddControllers();

Console.WriteLine("APP STARTING before AddDbContext...");


// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<PrayerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Azure DB CONNECTION: {conn}");

// Start -  Dynamically Add CORS policy to allow React app or any other apps to use .NET Core web APIs (from appsettings.json)
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// End  - Dynamically Add CORS policy to allow React app or any other apps to use .NET Core web APIs (from appsettings.json)

// Add Swagger (for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Add session support:
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    options.Cookie.SameSite = SameSiteMode.None;  // Allow cross-origin requests
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Ensure it is sent over HTTPS

});

var app = builder.Build();

Console.WriteLine("APP STARTING after build app...");

// after build, add migration at the code level to perform ef db migration to target i.e. azure or any other target
// this is manually done from powershell

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PrayerDbContext>();
    if (db.Database.IsRelational()) {
        db.Database.Migrate();
    }    
}


// This is equivalent to Configure in older versions
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS first (so it applies to all subsequent middleware)
app.UseCors("AllowReactApp");

// HTTPS redirection should be early in the pipeline, typically after CORS
app.UseHttpsRedirection();

// Routing should be placed before authorization and session to ensure proper routing behavior
app.UseRouting();

// Session middleware needs to come before authorization to ensure the session is available
app.UseSession();

// Authorization comes after session to ensure the session info is available for authorization
app.UseAuthorization();

// Finally, map controllers to handle the requests
app.MapControllers();


app.Run();

