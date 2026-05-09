using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

// ============= WEB APLIKACIJA SETUP =============
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<VideoGameTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("VideoGameTrackerDb")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register EF Repositories
builder.Services.AddScoped<DevelopersRepository>();
builder.Services.AddScoped<GenresRepository>();
builder.Services.AddScoped<PlatformsRepository>();
builder.Services.AddScoped<GamesRepository>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<GameEntriesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
