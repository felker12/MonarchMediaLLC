using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonarchMediaLLC.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedProjectMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ProjectSummaries",
                newName: "LiveUrl");

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "ProjectSummaries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CompletedOn",
                table: "ProjectSummaries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ProjectSummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Featured",
                table: "ProjectSummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageAlt",
                table: "ProjectSummaries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Industry",
                table: "ProjectSummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "ProjectSummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Package",
                table: "ProjectSummaries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClientName", "CompletedOn", "DisplayOrder", "Featured", "ImageAlt", "Industry", "IsPublic", "Package" },
                values: new object[] { "", new DateOnly(2026, 7, 1), 0, false, "", 0, true, 0 });

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClientName", "CompletedOn", "DisplayOrder", "Featured", "ImageAlt", "Industry", "IsPublic", "Package" },
                values: new object[] { "", new DateOnly(2026, 7, 1), 0, false, "", 0, true, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "Featured",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "ImageAlt",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "ProjectSummaries");

            migrationBuilder.DropColumn(
                name: "Package",
                table: "ProjectSummaries");

            migrationBuilder.RenameColumn(
                name: "LiveUrl",
                table: "ProjectSummaries",
                newName: "Url");
        }
    }
}
