using BioDataApp.Data;
using BioDataApp.ServiceInterface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddDbContext<BioDataDbContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("BioDataConnectionString"))); //add the db context as a service

builder.Services.AddScoped<ICountryStateLGAServices, CountryStateLGAServices>(); //add the db context interface as a service

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
