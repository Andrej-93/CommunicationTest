using CommunicationTest.Data;
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddTransient<AppDbContext>(s => new AppDbContext(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<WeatherForecastService>();
builder.Services.AddSingleton<CommunicationService>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();

var app = builder.Build();

using (var context = new AppDbContext(builder.Configuration.GetConnectionString("Default")))
{
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
