using GymTime.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.OpenApi;   


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Swagger/Open Api support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    { 
        Version = "v1",
        Title = "GymTime API",
        Description = "An ASP.NET Core Web API for managing gym workouts and diets.",
        Contact = new OpenApiContact
        {
            Name = "Justin Simon",
            Url = new Uri("https://github.com/JustinSimon-hub?tab=repositories")
        }
    });
});

//Adding Cors for api accessing from users
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    policy.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
});



//  Register IDbConnection for DI
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnection(connectionString);
});

// Register your repository
builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<UserRepository>();

//Neccesary for User accounting
builder.Services.AddSession();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // ✅ Enable Swagger in Development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GymTime API v1");
        options.RoutePrefix = "api-docs"; // Access at: https://localhost:7000/api-docs
        options.DocumentTitle = "GymTime API Documentation";
        options.DisplayRequestDuration();
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles(); // ensures wwwroot files are served

app.UseRouting();
//cors
app.UseCors("AllowAll");

app.UseAuthorization();

//Neccesary for User accounting
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
