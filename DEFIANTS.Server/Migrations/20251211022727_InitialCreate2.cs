using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEFIANTS.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Torneos_JuegoId",
                table: "Torneos",
                column: "JuegoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Torneos_Juegos_JuegoId",
                table: "Torneos",
                column: "JuegoId",
                principalTable: "Juegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Torneos_Juegos_JuegoId",
                table: "Torneos");

            migrationBuilder.DropIndex(
                name: "IX_Torneos_JuegoId",
                table: "Torneos");
        }
    }
}
