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

// =========================================================
// PRODUCTION STABILIZATION LAYER: CONFIGURATION FALLBACKS
// =========================================================
// Safely query configurations using both nested hierarchy standards and flat Azure Slug variables
var validUser = (builder.Configuration["AdminSettings:AdminUsername"] ?? builder.Configuration["adminsettings-adminusername"])?.Trim();
var validKey = (builder.Configuration["AdminSettings:SecretPasskey"] ?? builder.Configuration["adminsettings-secretpasskey"])?.Trim();

// Automated Container Log diagnostics on startup
Console.WriteLine("====== Monarch Media LLC Security Diagnostic Initialization ======");
if (string.IsNullOrEmpty(validUser))
    Console.WriteLine("⚠️ SYSTEM WARNING: AdminUsername could not be loaded and is currently NULL!");
else
    Console.WriteLine($"✅ AdminUsername loaded successfully: '{validUser}'");

if (string.IsNullOrEmpty(validKey))
    Console.WriteLine("⚠️ SYSTEM WARNING: SecretPasskey could not be loaded and is currently NULL!");
else
    Console.WriteLine($"✅ SecretPasskey verified and loaded successfully. Token Length: {validKey.Length} characters.");
Console.WriteLine("=================================================================");

//Configure the HTTP request pipeline.
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Automatically apply database migrations on startup for seamless maintenance
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonarchDbContext>();
    await db.Database.MigrateAsync();
}

//Dynamically query database items
app.MapGet("/api/team", async (MonarchDbContext db) =>
    await db.TeamMembers.ToListAsync());

app.MapGet("/api/projects", async (MonarchDbContext db) =>
    await db.ProjectSummaries.ToListAsync());

//Admin verification endpoint
app.MapPost("/api/admin/verify", (AdminLoginRequest login) =>
{
    //Sanitize inbound text elements against whitespace bugs
    var inputUser = login.UserName?.Trim();
    var inputKey = login.Passkey?.Trim();

    if (string.IsNullOrEmpty(validKey) || validKey != inputKey || validUser != inputUser)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new { Authenticated = true });
});

//Admin endpoint to add a new project
app.MapPost("/api/projects", async (ProjectSummary project, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    db.ProjectSummaries.Add(project);
    await db.SaveChangesAsync();
    return Results.Created($"/api/projects/{project.Id}", project);
});

//Admin endpoint to delete an existing project
app.MapDelete("/api/projects/{id:int}", async (int id, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    var project = await db.ProjectSummaries.FindAsync(id);
    if (project == null)
    {
        return Results.NotFound(new { Message = $"Project target index {id} does not exist inside storage." });
    }

    db.ProjectSummaries.Remove(project);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Admin endpoint to update an existing project
app.MapPut("/api/projects/{id:int}", async (int id, ProjectSummary updatedProject, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    var existingProject = await db.ProjectSummaries.FindAsync(id);
    if (existingProject == null)
        return Results.NotFound(new { Message = $"Project target index {id} does not exist inside storage." });

    existingProject.Title = updatedProject.Title;
    existingProject.Description = updatedProject.Description;
    existingProject.TechStack = updatedProject.TechStack;
    existingProject.LiveUrl = updatedProject.LiveUrl;
    existingProject.ImagePath = updatedProject.ImagePath;
    existingProject.ImageAlt = updatedProject.ImageAlt;
    existingProject.Package = updatedProject.Package;
    existingProject.Featured = updatedProject.Featured;
    existingProject.DisplayOrder = updatedProject.DisplayOrder;
    existingProject.CompletedOn = updatedProject.CompletedOn;
    existingProject.IsPublic = updatedProject.IsPublic;
    existingProject.Industry = updatedProject.Industry;
    existingProject.ClientName = updatedProject.ClientName;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Admin endpoint to add a new team member
app.MapPost("/api/team", async (TeamMember member, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    db.TeamMembers.Add(member);
    await db.SaveChangesAsync();
    return Results.Created($"/api/team/{member.Id}", member);
});

//Admin endpoint to update an existing team member
app.MapPut("/api/team/{id:int}", async (int id, TeamMember updatedMember, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    var existingMember = await db.TeamMembers.FindAsync(id);
    if (existingMember == null)
        return Results.NotFound(new { Message = $"Team member index {id} does not exist." });

    existingMember.Name = updatedMember.Name;
    existingMember.Role = updatedMember.Role;
    existingMember.Bio = updatedMember.Bio;
    existingMember.ImagePath = updatedMember.ImagePath;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Admin endpoint to delete an existing team member
app.MapDelete("/api/team/{id:int}", async (int id, HttpRequest request, MonarchDbContext db) =>
{
    var token = request.Headers["X-Admin-Token"].ToString().Trim();
    if (string.IsNullOrEmpty(validKey) || token != validKey)
        return Results.Unauthorized();

    var member = await db.TeamMembers.FindAsync(id);
    if (member == null)
        return Results.NotFound(new { Message = $"Team member index {id} does not exist." });

    db.TeamMembers.Remove(member);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDefaultEndpoints();
app.Run();