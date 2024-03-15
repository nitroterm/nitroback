using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nitroterm.Backend.Migrations
{
    /// <inheritdoc />
    public partial class PostsUserNitroIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NitroLevel",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicIdentifier",
                table: "Posts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NitroLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicIdentifier",
                table: "Posts");
        }
    }
}
