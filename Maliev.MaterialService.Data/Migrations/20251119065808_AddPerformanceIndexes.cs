using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.MaterialService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_materials_active",
                table: "materials",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_materials_created_at",
                table: "materials",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_materials_price_per_unit",
                table: "materials",
                column: "price_per_unit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_materials_active",
                table: "materials");

            migrationBuilder.DropIndex(
                name: "ix_materials_created_at",
                table: "materials");

            migrationBuilder.DropIndex(
                name: "ix_materials_price_per_unit",
                table: "materials");
        }
    }
}
