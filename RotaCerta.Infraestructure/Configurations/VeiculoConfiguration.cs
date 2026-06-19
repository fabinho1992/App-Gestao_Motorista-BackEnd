using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Models;

namespace RotaCerta.Infrastructure.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("veiculos");

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

        // FK motorista
        builder.Property(v => v.MotoristaId)
            .HasColumnName("motorista_id")
            .HasColumnType("uuid")
            .IsRequired();

        // campos do Veiculo
        builder.Property(v => v.Placa)
            .HasColumnName("placa")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(v => v.Modelo)
            .HasColumnName("modelo")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Ano)
            .HasColumnName("ano")
            .IsRequired();

        builder.Property(v => v.TipoCombustivel)
            .HasColumnName("tipo_combustivel")
            .HasConversion<string>() // salva como string no banco
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(v => v.KmAtual)
            .HasColumnName("km_atual")
            .HasColumnType("double precision")
            .IsRequired();

        // campos troca de óleo
        builder.Property(v => v.KmUltimoOleo)
            .HasColumnName("km_ultimo_oleo")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(v => v.DataUltimoOleo)
            .HasColumnName("data_ultimo_oleo")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(v => v.IntervaloOleo)
            .HasColumnName("intervalo_oleo")
            .HasColumnType("double precision")
            .HasDefaultValue(5000)
            .IsRequired();

        // índice único na placa
        builder.HasIndex(v => v.Placa)
            .IsUnique()
            .HasDatabaseName("ix_veiculos_placa");

        // índice para busca por motorista
        builder.HasIndex(v => v.MotoristaId)
            .HasDatabaseName("ix_veiculos_motorista_id");

        // filtro global — soft delete
        builder.HasQueryFilter(v => !v.Excluido);

        // ignora DomainEvents
        builder.Ignore(v => v.DomainEvents);
    }
}