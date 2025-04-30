using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class SponsorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rpsx_additional_sponsor_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userID = table.Column<Guid>(type: "TEXT", nullable: false),
                    sponsorTier = table.Column<string>(type: "TEXT", nullable: false),
                    date_of_end = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rpsx_additional_sponsor_data", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rpsx_additional_sponsor_data_userID",
                table: "rpsx_additional_sponsor_data",
                column: "userID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rpsx_additional_sponsor_data");
        }
    }
}
