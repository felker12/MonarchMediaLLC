using Microsoft.EntityFrameworkCore;
using MonarchMediaLLC.Shared;

namespace MonarchMediaLLC.ApiService.Data;

public class MonarchDbContext : DbContext
{
    public MonarchDbContext(DbContextOptions<MonarchDbContext> options) : base(options)
    {
    }

    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<ProjectSummary> ProjectSummaries => Set<ProjectSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Optional: Seed initial data if the database is freshly created
        modelBuilder.Entity<TeamMember>().HasData(
            new TeamMember
            {
                Id = 1,
                Name = "Christa Summerville",
                Role = "CEO & Executive Director",
                Bio = "Steers business strategy, client partnerships, and corporate vision to help local businesses establish clear brand dominion.",
                ImagePath = "images/team/Christa.jpg"
            },
            new TeamMember
            {
                Id = 2,
                Name = "Anthony",
                Role = "Lead Systems Developer",
                Bio = "Architects ultra-performant modern web ecosystems, bridging the gap between blazing-fast JAMstack frontends and complex, data-driven full-stack software systems.",
                ImagePath = "images/team/Anthony.jpg"
            }
        );

        modelBuilder.Entity<ProjectSummary>().HasData(
            new ProjectSummary
            {
                Id = 1,
                Title = "Southard Homes",
                Description = "A custom architectural web portal featuring high-performance image lightboxes and 2D/3D floorplan matrices.",
                TechStack = "Astro • TS • Tailwind",
                Url = "https://www.southardhomesllc.com/"
            },
            new ProjectSummary
            {
                Id = 2,
                Title = "Quality Electric of Indiana",
                Description = "Corporate communication platform optimizing structural local SEO and service maps for the Wabash Valley area.",
                TechStack = "Astro • TS • Tailwind",
                Url = "https://qualityelectricofindiana.com/"
            }
        );
    }
}