using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RotaCerta.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarManutencoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "manutencoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    veiculo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    data_realizacao = table.Column<DateOnly>(type: "date", nullable: false),
                    km_realizacao = table.Column<double>(type: "double precision", nullable: false),
                    custo = table.Column<double>(type: "double precision", nullable: false),
                    observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deletado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manutencoes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_manutencoes_veiculo_id",
                table: "manutencoes",
                column: "veiculo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manutencoes");
        }
    }
}
