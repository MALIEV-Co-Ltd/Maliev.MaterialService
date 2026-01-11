using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.MaterialService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVersionToByteArrayV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "version", table: "materials");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "materials", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "version", table: "colors");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "colors", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "version", table: "manufacturing_processes");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "manufacturing_processes", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "version", table: "mechanical_properties");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "mechanical_properties", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "version", table: "post_processing_methods");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "post_processing_methods", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);

            migrationBuilder.DropColumn(name: "version", table: "suppliers");
            migrationBuilder.AddColumn<byte[]>(name: "version", table: "suppliers", type: "bytea", rowVersion: true, nullable: false, defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "version", table: "materials");
            migrationBuilder.AddColumn<int>(name: "version", table: "materials", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "version", table: "colors");
            migrationBuilder.AddColumn<int>(name: "version", table: "colors", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "version", table: "manufacturing_processes");
            migrationBuilder.AddColumn<int>(name: "version", table: "manufacturing_processes", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "version", table: "mechanical_properties");
            migrationBuilder.AddColumn<int>(name: "version", table: "mechanical_properties", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "version", table: "post_processing_methods");
            migrationBuilder.AddColumn<int>(name: "version", table: "post_processing_methods", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "version", table: "suppliers");
            migrationBuilder.AddColumn<int>(name: "version", table: "suppliers", type: "integer", nullable: false, defaultValue: 0);
        }
    }
}
