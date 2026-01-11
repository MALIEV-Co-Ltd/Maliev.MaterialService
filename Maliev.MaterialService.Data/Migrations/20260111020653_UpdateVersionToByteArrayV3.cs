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
            migrationBuilder.Sql("ALTER TABLE materials ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
            migrationBuilder.Sql("ALTER TABLE colors ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
            migrationBuilder.Sql("ALTER TABLE manufacturing_processes ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
            migrationBuilder.Sql("ALTER TABLE mechanical_properties ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
            migrationBuilder.Sql("ALTER TABLE post_processing_methods ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
            migrationBuilder.Sql("ALTER TABLE suppliers ALTER COLUMN version TYPE bytea USING decode(lpad(to_hex(version), 8, '0'), 'hex');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE materials ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
            migrationBuilder.Sql("ALTER TABLE colors ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
            migrationBuilder.Sql("ALTER TABLE manufacturing_processes ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
            migrationBuilder.Sql("ALTER TABLE mechanical_properties ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
            migrationBuilder.Sql("ALTER TABLE post_processing_methods ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
            migrationBuilder.Sql("ALTER TABLE suppliers ALTER COLUMN version TYPE integer USING ('x' || encode(version, 'hex'))::bit(32)::integer;");
        }
    }
}