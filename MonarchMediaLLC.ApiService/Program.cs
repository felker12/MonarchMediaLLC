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

//Admin endpoint to delete an existing project
app.MapDelete("/api/projects/{id:int}", async (int id, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    //Verify incoming administrative authorization token header
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
        return Results.Unauthorized();

    //Query target entity row out of SQLite context
    var project = await db.ProjectSummaries.FindAsync(id);
    if (project == null)
    {
        return Results.NotFound(new { Message = $"Project target index {id} does not exist inside storage." });
    }

    //Stage and commit destruction lifecycle
    db.ProjectSummaries.Remove(project);
    await db.SaveChangesAsync();

    return Results.NoContent(); // Standard REST 204 response code for successful delete actions
});

//Admin endpoint to update an existing project
app.MapPut("/api/projects/{id:int}", async (int id, ProjectSummary updatedProject, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    //Verify incoming administrative authorization token header
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
        return Results.Unauthorized();

    //Locate targeted database record
    var existingProject = await db.ProjectSummaries.FindAsync(id);
    if (existingProject == null)
        return Results.NotFound(new { Message = $"Project target index {id} does not exist inside storage." });

    //Mutate table properties safely
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
    return Results.NoContent(); // 204 Standard success response for updates
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

//Admin endpoint to update an existing team member
app.MapPut("/api/team/{id:int}", async (int id, TeamMember updatedMember, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
        return Results.Unauthorized();

    var existingMember = await db.TeamMembers.FindAsync(id);
    if (existingMember == null)
        return Results.NotFound(new { Message = $"Team member index {id} does not exist." });

    // Mutate state records safely
    existingMember.Name = updatedMember.Name;
    existingMember.Role = updatedMember.Role;
    existingMember.Bio = updatedMember.Bio;
    existingMember.ImagePath = updatedMember.ImagePath;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Admin endpoint to delete an existing team member
app.MapDelete("/api/team/{id:int}", async (int id, HttpRequest request, MonarchDbContext db, IConfiguration config) =>
{
    var token = request.Headers["X-Admin-Token"].ToString();
    if (token != config["AdminSettings:SecretPasskey"])
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
