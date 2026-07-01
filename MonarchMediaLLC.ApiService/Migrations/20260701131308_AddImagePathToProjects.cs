using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonarchMediaLLC.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ProjectSummaries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagePath",
                value: "");

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagePath",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ProjectSummaries");
        }
    }
}
