using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioDataApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStateIdToLga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LGATab_StateTab_StateTabId",
                table: "LGATab");

            migrationBuilder.AlterColumn<int>(
                name: "StateTabId",
                table: "LGATab",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LGATab_StateTab_StateTabId",
                table: "LGATab",
                column: "StateTabId",
                principalTable: "StateTab",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LGATab_StateTab_StateTabId",
                table: "LGATab");

            migrationBuilder.AlterColumn<int>(
                name: "StateTabId",
                table: "LGATab",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LGATab_StateTab_StateTabId",
                table: "LGATab",
                column: "StateTabId",
                principalTable: "StateTab",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
