using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class SponsorAndDiscordUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rpsx_discord_data");

            migrationBuilder.DropColumn(
                name: "date_of_end",
                table: "rpsx_additional_sponsor_data");

            migrationBuilder.DropColumn(
                name: "sponsorTier",
                table: "rpsx_additional_sponsor_data");

            migrationBuilder.CreateTable(
                name: "sponsor_tier_infos",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sponsor_data_id = table.Column<int>(type: "INTEGER", nullable: false),
                    sponsor_tier_id = table.Column<string>(type: "TEXT", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsor_tier_infos", x => x.id);
                    table.ForeignKey(
                        name: "FK_sponsor_tier_infos_rpsx_additional_sponsor_data_sponsor_data_id",
                        column: x => x.sponsor_data_id,
                        principalTable: "rpsx_additional_sponsor_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_tier_infos_sponsor_data_id_sponsor_tier_id",
                table: "sponsor_tier_infos",
                columns: new[] { "sponsor_data_id", "sponsor_tier_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sponsor_tier_infos");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_end",
                table: "rpsx_additional_sponsor_data",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sponsorTier",
                table: "rpsx_additional_sponsor_data",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "rpsx_discord_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date_created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    discordID = table.Column<string>(type: "text", nullable: true),
                    userID = table.Column<Guid>(type: "TEXT", nullable: false),
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
    }
}
