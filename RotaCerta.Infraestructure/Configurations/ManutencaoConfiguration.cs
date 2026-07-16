using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Models;

namespace RotaCerta.Infrastructure.Configurations;

public class ManutencaoConfiguration : IEntityTypeConfiguration<Manutencao>
{
    public void Configure(EntityTypeBuilder<Manutencao> builder)
    {
        builder.ToTable("manutencoes");

        // chave primária
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // campos base
        builder.Property(m => m.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(m => m.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(m => m.DeletadoEm)
            .HasColumnName("deletado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(m => m.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false)
            .IsRequired();

        // FK veículo
        builder.Property(m => m.VeiculoId)
            .HasColumnName("veiculo_id")
            .HasColumnType("uuid")
            .IsRequired();

        // campos da Manutencao
        builder.Property(m => m.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>() // salva como string no banco
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200); 

        builder.Property(m => m.DataRealizacao)
            .HasColumnName("data_realizacao")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(m => m.KmRealizacao)
            .HasColumnName("km_realizacao")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(m => m.Custo)
            .HasColumnName("custo")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(m => m.Observacao)
            .HasColumnName("observacao")
            .HasMaxLength(500);

        // índice para busca por veículo
        builder.HasIndex(m => m.VeiculoId)
            .HasDatabaseName("ix_manutencoes_veiculo_id");

        // filtro global — soft delete
        builder.HasQueryFilter(m => !m.Excluido);

        // ignora DomainEvents
        builder.Ignore(m => m.DomainEvents);
    }
}
