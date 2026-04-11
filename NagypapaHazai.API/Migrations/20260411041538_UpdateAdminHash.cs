using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NagypapaHazai.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$sQTbILtZbCELlsh/GEqZtOewFM0K0Ijk1VWokeEM0tEF.Ia3dDJjG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$N.vX.K/wB.jYtGzEwT1xYe6Kx9R6lM4Xp.r/5Y.K.K/wB.jYtGzEw");
        }
    }
}
