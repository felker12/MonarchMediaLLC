using Microsoft.EntityFrameworkCore;
using MonarchMediaLLC.ApiService.Data;
using MonarchMediaLLC.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Configure EF Core with SQLite targeting a local database file
var connectionString = builder.Configuration.GetConnectionString("MonarchDb") ?? "Data Source=monarch.db";
builder.Services.AddDbContext<MonarchDbContext>(options =>
    options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Dynamically query database items
app.MapGet("/api/team", async (MonarchDbContext db) =>
    await db.TeamMembers.ToListAsync());

app.MapGet("/api/projects", async (MonarchDbContext db) =>
    await db.ProjectSummaries.ToListAsync());

// Automatically apply database migrations on startup for seamless maintenance
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonarchDbContext>();
    await db.Database.MigrateAsync();
}

app.MapDefaultEndpoints();
app.Run();
