using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchTicket",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GameCode = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchTicket_User",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchTicket_Status_Format_CreatedDate",
                schema: "dbo",
                table: "MatchTicket",
                columns: new[] { "Status", "Format", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchTicket_UserId",
                schema: "dbo",
                table: "MatchTicket",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchTicket",
                schema: "dbo");
        }
    }
}
