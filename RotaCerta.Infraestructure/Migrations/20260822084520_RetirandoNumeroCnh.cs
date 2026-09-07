using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RotaCerta.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class RetirandoNumeroCnh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_motoristas_cnh",
                table: "motoristas");

            migrationBuilder.DropColumn(
                name: "cnh",
                table: "motoristas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cnh",
                table: "motoristas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_motoristas_cnh",
                table: "motoristas",
                column: "cnh",
                unique: true);
        }
    }
}
