using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonarchMediaLLC.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class FixCompletedOnDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedOn",
                value: new DateOnly(1, 1, 1));

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CompletedOn",
                value: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CompletedOn",
                value: new DateOnly(2026, 7, 1));

            migrationBuilder.UpdateData(
                table: "ProjectSummaries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CompletedOn",
                value: new DateOnly(2026, 7, 1));
        }
    }
}
