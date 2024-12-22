using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class Barks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "bark_pitch",
                table: "profile",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "bark_proto",
                table: "profile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "high_bark_var",
                table: "profile",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "low_bark_var",
                table: "profile",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bark_pitch",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "bark_proto",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "high_bark_var",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "low_bark_var",
                table: "profile");
        }
    }
}
