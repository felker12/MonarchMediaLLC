using Microsoft.EntityFrameworkCore;
using MonarchMediaLLC.ApiService.Data;
using MonarchMediaLLC.Shared;
using System.Diagnostics;

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

//Dynamically query database items
app.MapGet("/api/team", async (MonarchDbContext db) =>
    await db.TeamMembers.ToListAsync());

app.MapGet("/api/projects", async (MonarchDbContext db) =>
    await db.ProjectSummaries.ToListAsync());

//Automatically apply database migrations on startup for seamless maintenance
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonarchDbContext>();
    await db.Database.MigrateAsync();
}

//Admin verification endpoint
app.MapPost("/api/admin/verify", (AdminLoginRequest login, IConfiguration config) =>
{
    var validUser = config["AdminSettings:AdminUsername"];
    var validKey = config["AdminSettings:SecretPasskey"];

    if (string.IsNullOrEmpty(validKey) || validKey != login.Passkey || validUser != login.UserName)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new { Authenticated = true });
});

//Admin endpoint to add a new project
app.MapPost("/api/projects", async (ProjectSummary project, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
        return Results.Unauthorized();

    db.ProjectSummaries.Add(project);
    await db.SaveChangesAsync();
    return Results.Created($"/api/projects/{project.Id}", project);
});

//Admin endpoint to add a new team member
app.MapPost("/api/team", async (TeamMember member, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
        return Results.Unauthorized();

    db.TeamMembers.Add(member);
    await db.SaveChangesAsync();
    return Results.Created($"/api/team/{member.Id}", member);
});

app.MapDefaultEndpoints();
app.Run();
