using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamsAndGameFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                schema: "dbo",
                table: "Room");

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveGuesserGameMemberId",
                schema: "dbo",
                table: "Round",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Format",
                schema: "dbo",
                table: "Room",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamNumber",
                schema: "dbo",
                table: "GameMember",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Format",
                schema: "dbo",
                table: "Game",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Round_ActiveGuesserGameMemberId",
                schema: "dbo",
                table: "Round",
                column: "ActiveGuesserGameMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Round_ActiveGuesserGameMember",
                schema: "dbo",
                table: "Round",
                column: "ActiveGuesserGameMemberId",
                principalSchema: "dbo",
                principalTable: "GameMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Round_ActiveGuesserGameMember",
                schema: "dbo",
                table: "Round");

            migrationBuilder.DropIndex(
                name: "IX_Round_ActiveGuesserGameMemberId",
                schema: "dbo",
                table: "Round");

            migrationBuilder.DropColumn(
                name: "ActiveGuesserGameMemberId",
                schema: "dbo",
                table: "Round");

            migrationBuilder.DropColumn(
                name: "Format",
                schema: "dbo",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "TeamNumber",
                schema: "dbo",
                table: "GameMember");

            migrationBuilder.DropColumn(
                name: "Format",
                schema: "dbo",
                table: "Game");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                schema: "dbo",
                table: "Room",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
