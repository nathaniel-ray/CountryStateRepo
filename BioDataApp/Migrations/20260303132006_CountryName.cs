using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioDataApp.Migrations
{
    /// <inheritdoc />
    public partial class CountryName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryTab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateTab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryTabId = table.Column<int>(type: "int", nullable: true),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateTab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateTab_CountryTab_CountryTabId",
                        column: x => x.CountryTabId,
                        principalTable: "CountryTab",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LGATab",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LGACode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LGAName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateTabId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LGATab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LGATab_StateTab_StateTabId",
                        column: x => x.StateTabId,
                        principalTable: "StateTab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LGATab_StateTabId",
                table: "LGATab",
                column: "StateTabId");

            migrationBuilder.CreateIndex(
                name: "IX_StateTab_CountryTabId",
                table: "StateTab",
                column: "CountryTabId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LGATab");

            migrationBuilder.DropTable(
                name: "StateTab");

            migrationBuilder.DropTable(
                name: "CountryTab");
        }
    }
}
