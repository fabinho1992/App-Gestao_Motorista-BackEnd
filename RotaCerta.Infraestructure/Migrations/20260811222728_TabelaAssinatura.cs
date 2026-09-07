using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RotaCerta.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class TabelaAssinatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assinaturas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    motorista_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    trial_fim_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gateway_cliente_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gateway_assinatura_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    lembrete_enviado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deletado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assinaturas_motorista_id",
                table: "assinaturas",
                column: "motorista_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_assinaturas_status_trial_fim_em",
                table: "assinaturas",
                columns: new[] { "status", "trial_fim_em" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assinaturas");
        }
    }
}
