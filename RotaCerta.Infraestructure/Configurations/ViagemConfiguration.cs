using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Enums;
using RotaCerta.Domain.Models;
using RotaCerta.Domain.Viagens;

namespace RotaCerta.Infrastructure.Configurations;

public class ViagemConfiguration : IEntityTypeConfiguration<Viagem>
{
    public void Configure(EntityTypeBuilder<Viagem> builder)
    {
        builder.ToTable("viagens");

        // chave primária
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // campos base
        builder.Property(v => v.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(v => v.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(v => v.DeletadoEm)
            .HasColumnName("deletado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(v => v.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false)
            .IsRequired();

        // FKs
        builder.Property(v => v.MotoristaId)
            .HasColumnName("motorista_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(v => v.VeiculoId)
            .HasColumnName("veiculo_id")
            .HasColumnType("uuid")
            .IsRequired();

        // --- campos preenchidos na ABERTURA ---
        builder.Property(v => v.DataSaida)
            .HasColumnName("data_saida")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(v => v.Origem)
            .HasColumnName("origem")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.EmpresaContratante)
            .HasColumnName("empresa_contratante")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.KmInicial)
            .HasColumnName("km_inicial")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(v => v.ValorFrete)
            .HasColumnName("valor_frete")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(v => v.FormaPagamento)
            .HasColumnName("forma_pagamento")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(v => v.Pago)
            .HasColumnName("pago")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(v => v.StatusPagamento)
            .HasColumnName("status_pagamento")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(StatusPagamento.Pendente)
            .IsRequired();

        builder.Property(v => v.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // --- campos preenchidos no ENCERRAMENTO ---
        builder.Property(v => v.KmFinal)
            .HasColumnName("km_final")
            .HasColumnType("double precision");

        builder.Property(v => v.DataFim)
            .HasColumnName("data_fim")
            .HasColumnType("date");

        builder.Property(v => v.GastoCombustivel)
            .HasColumnName("gasto_combustivel")
            .HasColumnType("double precision")
            .HasDefaultValue(0);

        builder.Property(v => v.GastoPedagio)
            .HasColumnName("gasto_pedagio")
            .HasColumnType("double precision")
            .HasDefaultValue(0);

        builder.Property(v => v.GastoAlimentacao)
            .HasColumnName("gasto_alimentacao")
            .HasColumnType("double precision")
            .HasDefaultValue(0);

        builder.Property(v => v.GastoOutros)
            .HasColumnName("gasto_outros")
            .HasColumnType("double precision")
            .HasDefaultValue(0);

        builder.Property(v => v.PrecoCombustivelLitro)
            .HasColumnName("preco_combustivel_litro")
            .HasColumnType("double precision")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(v => v.ObsEncerramento)
            .HasColumnName("obs_encerramento")
            .HasMaxLength(500)
            .IsRequired(false);

        // propriedades calculadas — não persistidas
        builder.Ignore(v => v.TotalGastos);
        builder.Ignore(v => v.SaldoLiquido);
        builder.Ignore(v => v.KmRodado);
        builder.Ignore(v => v.TotalEntregasRealizadas);
        builder.Ignore(v => v.DomainEvents);

        // relacionamento com Entrega
        builder.HasMany(v => v.Entregas)
            .WithOne()
            .HasForeignKey(e => e.ViagemId)
            .HasConstraintName("fk_entregas_viagem")
            .OnDelete(DeleteBehavior.Cascade);

        // adiciona FK explícita para Motorista
        builder.HasOne<Motorista>()
            .WithMany()
            .HasForeignKey(v => v.MotoristaId)
            .HasConstraintName("fk_viagens_motorista")
            .OnDelete(DeleteBehavior.Restrict);

        // FK para Veiculo com navegação — necessário para Include(v => v.Veiculo)
        builder.HasOne(v => v.Veiculo)
            .WithMany(v => v.Viagens)
            .HasForeignKey(v => v.VeiculoId)
            .HasConstraintName("fk_viagens_veiculo")
            .OnDelete(DeleteBehavior.Restrict);

        // índices
        builder.HasIndex(v => v.MotoristaId)
            .HasDatabaseName("ix_viagens_motorista_id");

        builder.HasIndex(v => v.VeiculoId)
            .HasDatabaseName("ix_viagens_veiculo_id");

        builder.HasIndex(v => v.DataSaida)
            .HasDatabaseName("ix_viagens_data_saida");

        // filtro global — soft delete
        builder.HasQueryFilter(v => !v.Excluido);
    }
}