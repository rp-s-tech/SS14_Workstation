using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ReworkSponsors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userID",
                table: "rpsx_additional_sponsor_data",
                newName: "player_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_rpsx_additional_sponsor_data_userID",
                table: "rpsx_additional_sponsor_data",
                newName: "IX_rpsx_additional_sponsor_data_player_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "player_user_id",
                table: "rpsx_additional_sponsor_data",
                newName: "userID");

            migrationBuilder.RenameIndex(
                name: "IX_rpsx_additional_sponsor_data_player_user_id",
                table: "rpsx_additional_sponsor_data",
                newName: "IX_rpsx_additional_sponsor_data_userID");
        }
    }
}
