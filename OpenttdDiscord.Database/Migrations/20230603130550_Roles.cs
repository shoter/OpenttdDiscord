using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenttdDiscord.Database.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildRoles",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRoles", x => new { x.GuildId, x.RoleId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildRoles");
        }
    }
}
