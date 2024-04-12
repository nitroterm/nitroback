﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nitroterm.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseTokens2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Device",
                table: "Tokens",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Device",
                keyValue: null,
                column: "Device",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Device",
                table: "Tokens",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
