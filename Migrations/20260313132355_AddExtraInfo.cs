using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NagypapaHazaiBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraInfo",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraInfo",
                table: "Properties");
        }
    }
}
