using EvolvedTax.Common.Utils;
using EvolvedTax.Helpers;
using EvolvedTax.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
});


//configure memory session

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(10);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.AccessDeniedPath = "/Account/AccessDenied?statusCode={0}";
            options.LoginPath = "/Account/Login/"; // auth redirect
            options.ExpireTimeSpan = new TimeSpan(1, 0, 0, 0);
        });
builder.Services.AddSession();
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
var configuration = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build();
var environment = builder.Environment;
ServiceActivator._config = new ConfigurationBuilder().SetBasePath(environment.ContentRootPath).AddJsonFile("appSettings.json").Build();
builder.Services.ConfigureApplicationServices(configuration, environment);
builder.Services.AddSignalR();
var app = builder.Build();
ServiceActivator.Configure(app.Services);


//Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<AnnouncementHub>("/announcementHub"); // Map the SignalR hub

app.UseStatusCodePagesWithRedirects("~/Account/AccessDenied?statusCode={0}");

app.UseSession();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.Run();
