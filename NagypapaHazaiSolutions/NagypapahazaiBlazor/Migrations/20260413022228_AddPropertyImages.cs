using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NagypapaHazaiBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Capacity", "CreatedAt", "Description", "ExtraInfo", "ImageUrl", "Location", "Name", "Status" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "/images/bakonyiHaz.jpg", "Bakony", "Faház a Bakonyban", null },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "/images/balatoniHaz.jpg", "Balatonlelle", "Vízparti Nyaraló", null },
                    { 3, null, new DateTime(2026, 4, 13, 4, 22, 28, 244, DateTimeKind.Local).AddTicks(7853), null, null, "", "", "Villányi Vendégház", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
