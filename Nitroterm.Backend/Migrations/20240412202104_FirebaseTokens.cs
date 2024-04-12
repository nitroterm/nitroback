using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nitroterm.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Device",
                table: "Tokens",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Device",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tokens");
        }
    }
}
