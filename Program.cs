using GymTime.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Swagger implemetation 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GymTime API",
        Version = "v1",
        Description = "An ASP.NET Core Web API for managing gym workouts and diets."
    });
});


// ✅ Register IDbConnection for DI
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnection(connectionString);
});

// ✅ Register your repository
builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<UserRepository>();

//Neccesary for User accounting
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ensures wwwroot files are served

app.UseRouting();

app.UseAuthorization();

//Neccesary for User accounting
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
