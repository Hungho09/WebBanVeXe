using Infrastructure;
using Infrastructure.Persistence;
<<<<<<< HEAD
using Infrastructure.Persistence.SeedData;
=======
>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:AutoMigrate"))
{
    using var migrateScope = app.Services.CreateScope();
    var db = migrateScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

<<<<<<< HEAD
if (app.Configuration.GetValue<bool>("DemoData:SeedOnStartup"))
{
    using var seedScope = app.Services.CreateScope();
    var log = seedScope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoData");
    await DemoDataSeeder.SeedAsync(seedScope.ServiceProvider, log);
}
=======

>>>>>>> 9197da9e81287ec8d327737d1f37f56927fc8b7e

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection(); // Disable for local if causing issues
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
