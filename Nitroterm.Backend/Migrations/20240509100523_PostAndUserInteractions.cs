using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nitroterm.Backend.Migrations
{
    /// <inheritdoc />
    public partial class PostAndUserInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserToUserInteractions_Users_UserId",
                table: "UserToUserInteractions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserToUserInteractions",
                newName: "TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserToUserInteractions_UserId",
                table: "UserToUserInteractions",
                newName: "IX_UserToUserInteractions_TargetUserId");

            migrationBuilder.AddColumn<int>(
                name: "SourceUserId",
                table: "UserToUserInteractions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserToPostInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceUserId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToPostInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToPostInteractions_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToPostInteractions_Users_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserToUserInteractions_SourceUserId",
                table: "UserToUserInteractions",
                column: "SourceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToPostInteractions_PostId",
                table: "UserToPostInteractions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToPostInteractions_SourceUserId",
                table: "UserToPostInteractions",
                column: "SourceUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserToUserInteractions_Users_SourceUserId",
                table: "UserToUserInteractions",
                column: "SourceUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToUserInteractions_Users_TargetUserId",
                table: "UserToUserInteractions",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserToUserInteractions_Users_SourceUserId",
                table: "UserToUserInteractions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToUserInteractions_Users_TargetUserId",
                table: "UserToUserInteractions");

            migrationBuilder.DropTable(
                name: "UserToPostInteractions");

            migrationBuilder.DropIndex(
                name: "IX_UserToUserInteractions_SourceUserId",
                table: "UserToUserInteractions");

            migrationBuilder.DropColumn(
                name: "SourceUserId",
                table: "UserToUserInteractions");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "UserToUserInteractions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserToUserInteractions_TargetUserId",
                table: "UserToUserInteractions",
                newName: "IX_UserToUserInteractions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserToUserInteractions_Users_UserId",
                table: "UserToUserInteractions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
