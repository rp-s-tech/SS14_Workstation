using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Patrons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patron_profile_item",
                columns: table => new
                {
                    patron_profile_item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_proto_id = table.Column<string>(type: "text", nullable: false),
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patron_profile_item", x => x.patron_profile_item_id);
                    table.ForeignKey(
                        name: "FK_patron_profile_item_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patron_profile_pets",
                columns: table => new
                {
                    patron_profile_pets_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pet_id = table.Column<string>(type: "text", nullable: false),
                    pet_name = table.Column<string>(type: "text", nullable: false),
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patron_profile_pets", x => x.patron_profile_pets_id);
                    table.ForeignKey(
                        name: "FK_patron_profile_pets_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patron_profile_item_profile_id_item_proto_id",
                table: "patron_profile_item",
                columns: new[] { "profile_id", "item_proto_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patron_profile_pets_profile_id",
                table: "patron_profile_pets",
                column: "profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patron_profile_pets_profile_id_pet_id",
                table: "patron_profile_pets",
                columns: new[] { "profile_id", "pet_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patron_profile_item");

            migrationBuilder.DropTable(
                name: "patron_profile_pets");
        }
    }
}
