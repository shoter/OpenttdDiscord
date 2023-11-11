using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenttdDiscord.Database.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class WelcomeMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoReplies",
                columns: table => new
                {
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TriggerMessage = table.Column<string>(type: "text", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ResponseMessage = table.Column<string>(type: "text", nullable: false),
                    AdditionalAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoReplies", x => new { x.ServerId, x.TriggerMessage });
                    table.ForeignKey(
                        name: "FK_AutoReplies_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WelcomeMessages",
                columns: table => new
                {
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomeMessages", x => x.ServerId);
                    table.ForeignKey(
                        name: "FK_WelcomeMessages_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoReplies");

            migrationBuilder.DropTable(
                name: "WelcomeMessages");
        }
    }
}
