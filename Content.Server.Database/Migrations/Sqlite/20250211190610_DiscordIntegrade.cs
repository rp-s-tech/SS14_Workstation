using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class DiscordIntegrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rpsx_discord_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    userID = table.Column<Guid>(type: "TEXT", nullable: false),
                    discordID = table.Column<string>(type: "text", nullable: true),
                    verify = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rpsx_discord_data", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rpsx_discord_data_discordID",
                table: "rpsx_discord_data",
                column: "discordID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rpsx_discord_data");
        }
    }
}
