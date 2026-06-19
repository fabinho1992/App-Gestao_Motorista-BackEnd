using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Models;

namespace RotaCerta.Infrastructure.Configurations;

public class EntregaConfiguration : IEntityTypeConfiguration<Entrega>
{
    public void Configure(EntityTypeBuilder<Entrega> builder)
    {
        builder.ToTable("entregas");

        // chave primária
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        // campos base
        builder.Property(e => e.CriadoEm)
            .HasColumnName("criado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(e => e.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(e => e.DeletadoEm)
            .HasColumnName("deletado_em")
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.Excluido)
            .HasColumnName("excluido")
            .HasDefaultValue(false)
            .IsRequired();

        // FK viagem
        builder.Property(e => e.ViagemId)
            .HasColumnName("viagem_id")
            .HasColumnType("uuid")
            .IsRequired();

        // campos da Entrega
        builder.Property(e => e.Cliente)
            .HasColumnName("cliente")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.EnderecoDestino)
            .HasColumnName("endereco_destino")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Observacao)
            .HasColumnName("observacao")
            .HasMaxLength(500);

        builder.Property(e => e.DataHoraEntrega)
            .HasColumnName("data_hora_entrega")
            .HasColumnType("timestamp with time zone");

        // fotos — lista de strings salva como array no PostgreSQL
        builder.Property(e => e.Fotos)
            .HasColumnName("fotos")
            .HasColumnType("text[]")
            .HasDefaultValueSql("'{}'");

        // índices
        builder.HasIndex(e => e.ViagemId)
            .HasDatabaseName("ix_entregas_viagem_id");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_entregas_status");

        // filtro global — soft delete
        builder.HasQueryFilter(e => !e.Excluido);

        // ignora DomainEvents
        builder.Ignore(e => e.DomainEvents);
    }
}