using MonarchMediaLLC.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

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

app.MapGet("/api/team", () => new List<TeamMember>
{
    new() { Name = "Christa Summerville", Role = "CEO & Executive Director", Bio = "Steers business strategy...", ImagePath = "images/team/Christa.jpg" },
    new() { Name = "Anthony", Role = "Lead Systems Developer", Bio = "Architects ultra-performant modern web ecosystems...", ImagePath = "images/team/Anthony.jpg" }
});

app.MapGet("/api/projects", () => new List<ProjectSummary>
{
    new() { Title = "Southard Homes", Description = "A custom architectural web portal...", TechStack = "Astro • TS • Tailwind", Url = "https://www.southardhomesllc.com/" }
});

app.MapDefaultEndpoints();
app.Run();
