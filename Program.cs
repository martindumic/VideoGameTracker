using VideoGameTracker.Data;
using VideoGameTracker.Models;

// ============= WEB APLIKACIJA SETUP =============
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Mock Repositories
builder.Services.AddSingleton<DevelopersMockRepository>();
builder.Services.AddSingleton<GenresMockRepository>();
builder.Services.AddSingleton<PlatformsMockRepository>();
builder.Services.AddSingleton<GamesMockRepository>();
builder.Services.AddSingleton<UsersMockRepository>();
builder.Services.AddSingleton<ReviewsMockRepository>();
builder.Services.AddSingleton<GameEntriesMockRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
