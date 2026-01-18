using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maliev.MaterialService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "colors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    hex_code = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_colors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "manufacturing_processes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_manufacturing_processes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mechanical_properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mechanical_properties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_processing_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_processing_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_info = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suppliers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    price_per_unit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    stock_level = table.Column<int>(type: "integer", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_materials", x => x.id);
                    table.ForeignKey(
                        name: "fk_materials__suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "material_colors",
                columns: table => new
                {
                    available_colors_id = table.Column<Guid>(type: "uuid", nullable: false),
                    materials_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_colors", x => new { x.available_colors_id, x.materials_id });
                    table.ForeignKey(
                        name: "fk_material_colors__colors_available_colors_id",
                        column: x => x.available_colors_id,
                        principalTable: "colors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_material_colors__materials_materials_id",
                        column: x => x.materials_id,
                        principalTable: "materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "material_manufacturing_processes",
                columns: table => new
                {
                    manufacturing_processes_id = table.Column<Guid>(type: "uuid", nullable: false),
                    materials_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_manufacturing_processes", x => new { x.manufacturing_processes_id, x.materials_id });
                    table.ForeignKey(
                        name: "fk_material_manufacturing_processes_manufacturing_processes_ma~",
                        column: x => x.manufacturing_processes_id,
                        principalTable: "manufacturing_processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_material_manufacturing_processes_materials_materials_id",
                        column: x => x.materials_id,
                        principalTable: "materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "material_mechanical_properties",
                columns: table => new
                {
                    material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mechanical_property_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_mechanical_properties", x => new { x.material_id, x.mechanical_property_id });
                    table.ForeignKey(
                        name: "fk_material_mechanical_properties__mechanical_properties_mechani~",
                        column: x => x.mechanical_property_id,
                        principalTable: "mechanical_properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_material_mechanical_properties_materials_material_id",
                        column: x => x.material_id,
                        principalTable: "materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "material_post_processing_methods",
                columns: table => new
                {
                    materials_id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_processing_methods_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_post_processing_methods", x => new { x.materials_id, x.post_processing_methods_id });
                    table.ForeignKey(
                        name: "fk_material_post_processing_methods_materials_materials_id",
                        column: x => x.materials_id,
                        principalTable: "materials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_material_post_processing_methods_post_processing_methods_po~",
                        column: x => x.post_processing_methods_id,
                        principalTable: "post_processing_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_colors_hex_code",
                table: "colors",
                column: "hex_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_colors_name",
                table: "colors",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_manufacturing_processes_name",
                table: "manufacturing_processes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_material_colors_materials_id",
                table: "material_colors",
                column: "materials_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_manufacturing_processes_materials_id",
                table: "material_manufacturing_processes",
                column: "materials_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_mechanical_properties_mechanical_property_id",
                table: "material_mechanical_properties",
                column: "mechanical_property_id");

            migrationBuilder.CreateIndex(
                name: "ix_material_post_processing_methods_post_processing_methods_id",
                table: "material_post_processing_methods",
                column: "post_processing_methods_id");

            migrationBuilder.CreateIndex(
                name: "ix_materials_active",
                table: "materials",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_materials_code",
                table: "materials",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_materials_created_at",
                table: "materials",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_materials_name",
                table: "materials",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_materials_price_per_unit",
                table: "materials",
                column: "price_per_unit");

            migrationBuilder.CreateIndex(
                name: "ix_materials_supplier_id",
                table: "materials",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_mechanical_properties_name",
                table: "mechanical_properties",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_processing_methods_name",
                table: "post_processing_methods",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "material_colors");

            migrationBuilder.DropTable(
                name: "material_manufacturing_processes");

            migrationBuilder.DropTable(
                name: "material_mechanical_properties");

            migrationBuilder.DropTable(
                name: "material_post_processing_methods");

            migrationBuilder.DropTable(
                name: "colors");

            migrationBuilder.DropTable(
                name: "manufacturing_processes");

            migrationBuilder.DropTable(
                name: "mechanical_properties");

            migrationBuilder.DropTable(
                name: "materials");

            migrationBuilder.DropTable(
                name: "post_processing_methods");

            migrationBuilder.DropTable(
                name: "suppliers");
        }
    }
}
