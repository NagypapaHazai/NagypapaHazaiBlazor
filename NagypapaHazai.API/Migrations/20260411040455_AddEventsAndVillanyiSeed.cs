using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NagypapaHazai.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsAndVillanyiSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Capacity", "CreatedAt", "ExtraInfo", "ImageUrl", "PricePerNight", "Status" },
                values: new object[] { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", 25000, "Aktív" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Capacity", "CreatedAt", "ExtraInfo", "ImageUrl", "PricePerNight", "Status" },
                values: new object[] { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", 35000, "Aktív" });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Capacity", "CreatedAt", "Description", "ExtraInfo", "ImageUrl", "Location", "Name", "PricePerNight", "Status" },
                values: new object[] { 3, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hangulatos borház a dűlők között.", "", "", "Villány", "Villányi Vendégház", 20000, "Aktív" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "UserName" },
                values: new object[] { "$2a$11$N.vX.K/wB.jYtGzEwT1xYe6Kx9R6lM4Xp.r/5Y.K.K/wB.jYtGzEw", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Capacity", "CreatedAt", "ExtraInfo", "ImageUrl", "PricePerNight", "Status" },
                values: new object[] { null, new DateTime(2026, 4, 11, 5, 54, 29, 749, DateTimeKind.Local).AddTicks(7522), null, null, 15000, null });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Capacity", "CreatedAt", "ExtraInfo", "ImageUrl", "PricePerNight", "Status" },
                values: new object[] { null, new DateTime(2026, 4, 11, 5, 54, 29, 752, DateTimeKind.Local).AddTicks(2770), null, null, 25000, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "UserName" },
                values: new object[] { "$2a$11$aU4NQ27DKxXvDH42UxEDFePifZEkqNAZQxsReHLLnpi5hf979lrbS", "admin" });
        }
    }
}
