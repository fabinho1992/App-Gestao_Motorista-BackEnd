using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RotaCerta.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class ColunaAssinaturas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "proxima_cobranca_em",
                table: "assinaturas",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "proxima_cobranca_em",
                table: "assinaturas");
        }
    }
}
