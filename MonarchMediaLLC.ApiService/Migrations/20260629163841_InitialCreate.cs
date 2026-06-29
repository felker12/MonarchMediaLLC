using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonarchMediaLLC.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TechStack = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProjectSummaries",
                columns: new[] { "Id", "Description", "TechStack", "Title", "Url" },
                values: new object[,]
                {
                    { 1, "A custom architectural web portal featuring high-performance image lightboxes and 2D/3D floorplan matrices.", "Astro • TS • Tailwind", "Southard Homes", "https://www.southardhomesllc.com/" },
                    { 2, "Corporate communication platform optimizing structural local SEO and service maps for the Wabash Valley area.", "Astro • TS • Tailwind", "Quality Electric of Indiana", "https://qualityelectricofindiana.com/" }
                });

            migrationBuilder.InsertData(
                table: "TeamMembers",
                columns: new[] { "Id", "Bio", "ImagePath", "Name", "Role" },
                values: new object[,]
                {
                    { 1, "Steers business strategy, client partnerships, and corporate vision to help local businesses establish clear brand dominion.", "images/team/Christa.jpg", "Christa Summerville", "CEO & Executive Director" },
                    { 2, "Architects ultra-performant modern web ecosystems, bridging the gap between blazing-fast JAMstack frontends and complex, data-driven full-stack software systems.", "images/team/Anthony.jpg", "Anthony", "Lead Systems Developer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectSummaries");

            migrationBuilder.DropTable(
                name: "TeamMembers");
        }
    }
}
