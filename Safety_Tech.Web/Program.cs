using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Safety_Tech.Models.Models;
using Safety_Tech.Services;
using Safety_Tech.Services.RoleServices;
using Safety_Tech.Services.UserServices;
using NLog.Extensions.Logging;
using System;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews(); // MVC Controllers and Views

// Add Entity Framework Core DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDataContext>()
    .AddDefaultTokenProviders();

// Configure Identity cookie paths so [Authorize] redirects to existing login route
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/loginView";
    options.AccessDeniedPath = "/Auth/loginView";
});
// Remove session configuration
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout of 30 minutes
//     options.Cookie.HttpOnly = true; // Cookie is HTTP only
//     options.Cookie.IsEssential = true; // Cookie is essential for the application
// });

// Add scoped services for dependency injection
builder.Services.AddApplicationServices();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(Program)); // Note: Change from Startup to Program for consistency

// Add Redis distributed cache for caching roles and lookups
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    options.InstanceName = "MVCWebApp_";
});

// Configure authentication services: use Cookies for MVC by default; keep JWT for APIs
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    }).AddOpenIdConnect(options =>
    {
        options.ClientId = builder.Configuration["Authentication:OpenIdConnectS:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:OpenIdConnectS:ClientSecret"];
        options.CallbackPath = builder.Configuration["Authentication:OpenIdConnectS:CallbackPath"];
        options.Authority = builder.Configuration["Authentication:OpenIdConnectS:Authority"];
        options.SaveTokens = true; // Save authentication tokens
        options.Scope.Add("openid"); // Request openid scope
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name", // Set name claim type
            RoleClaimType = "role"  // Set role claim type
        };
    })
.AddMicrosoftAccount(options =>
{
    options.ClientId = builder.Configuration["Authentication:AzureAd:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:AzureAd:ClientSecret"];
    options.CallbackPath = builder.Configuration["Authentication:AzureAd:CallbackPath"];
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});


// Add NLog for logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Clear default logging providers
    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); // Set logging level
    loggingBuilder.AddNLog("nlog.config"); // Add NLog configuration
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// Register global exception handling middleware
app.UseMiddleware<Safety_Tech.Web.Helper.ExceptionHandlingMiddleware>();

// Remove UseDeveloperExceptionPage and UseExceptionHandler
// if (app.Environment.IsDevelopment())
// {
//     app.UseDeveloperExceptionPage();
// }
// else
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Use HTTP Strict Transport Security in production
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files from wwwroot folder
// Remove session middleware
// app.UseSession(); // Enable session middleware
app.UseRouting(); // Enable routing
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

// Configure default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=loginView}/{id?}");

app.Run();
