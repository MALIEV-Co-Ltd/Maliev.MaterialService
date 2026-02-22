using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.MaterialService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialPricingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "cost_per_kg",
                table: "materials",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "density",
                table: "materials",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "process_parameters",
                table: "materials",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cost_per_kg",
                table: "materials");

            migrationBuilder.DropColumn(
                name: "density",
                table: "materials");

            migrationBuilder.DropColumn(
                name: "process_parameters",
                table: "materials");
        }
    }
}
