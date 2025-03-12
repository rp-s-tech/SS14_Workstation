using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class ProfileEconomics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "economics",
                columns: table => new
                {
                    economics_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    balance = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_economics", x => x.economics_id);
                    table.ForeignKey(
                        name: "FK_economics_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credit",
                columns: table => new
                {
                    credit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_economics_id = table.Column<int>(type: "integer", nullable: false),
                    percent = table.Column<int>(type: "integer", nullable: false),
                    summ = table.Column<int>(type: "integer", nullable: false),
                    credit_start = table.Column<int>(type: "integer", nullable: false),
                    next_payment = table.Column<int>(type: "integer", nullable: false),
                    credit_ending = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit", x => x.credit_id);
                    table.ForeignKey(
                        name: "FK_credit_economics_profile_economics_id",
                        column: x => x.profile_economics_id,
                        principalTable: "economics",
                        principalColumn: "economics_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposit",
                columns: table => new
                {
                    deposit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_economics_id = table.Column<int>(type: "integer", nullable: false),
                    percent = table.Column<int>(type: "integer", nullable: false),
                    summ = table.Column<int>(type: "integer", nullable: false),
                    deposit_start = table.Column<int>(type: "integer", nullable: false),
                    next_payment = table.Column<int>(type: "integer", nullable: false),
                    deposit_ending = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit", x => x.deposit_id);
                    table.ForeignKey(
                        name: "FK_deposit_economics_profile_economics_id",
                        column: x => x.profile_economics_id,
                        principalTable: "economics",
                        principalColumn: "economics_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_credit_profile_economics_id",
                table: "credit",
                column: "profile_economics_id");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_profile_economics_id",
                table: "deposit",
                column: "profile_economics_id");

            migrationBuilder.CreateIndex(
                name: "IX_economics_profile_id",
                table: "economics",
                column: "profile_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit");

            migrationBuilder.DropTable(
                name: "deposit");

            migrationBuilder.DropTable(
                name: "economics");
        }
    }
}
