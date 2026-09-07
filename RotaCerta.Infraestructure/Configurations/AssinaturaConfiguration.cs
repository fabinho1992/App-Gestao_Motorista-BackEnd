using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Infraestructure.Configurations
{
    public class AssinaturaConfiguration : IEntityTypeConfiguration<Assinatura>
    {
        public void Configure(EntityTypeBuilder<Assinatura> builder)
        {
            builder.ToTable("assinaturas");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            builder.Property(a => a.CriadoEm)
                .HasColumnName("criado_em")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(a => a.AtualizadoEm)
                .HasColumnName("atualizado_em")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(a => a.DeletadoEm)
                .HasColumnName("deletado_em")
                .HasColumnType("timestamp with time zone");

            builder.Property(a => a.Excluido)
                .HasColumnName("excluido")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(a => a.MotoristaId)
                .HasColumnName("motorista_id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(a => a.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.TrialFimEm)
                .HasColumnName("trial_fim_em")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(a => a.ProximaCobrancaEm)
                .HasColumnName("proxima_cobranca_em")
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            builder.Property(a => a.GatewayClienteId)
                .HasColumnName("gateway_cliente_id")
                .HasMaxLength(100);

            builder.Property(a => a.GatewayAssinaturaId)
                .HasColumnName("gateway_assinatura_id")
                .HasMaxLength(100);

            builder.Property(a => a.LembreteEnviado)
                .HasColumnName("lembrete_enviado")
                .HasDefaultValue(false)
                .IsRequired();

            // um motorista só pode ter uma assinatura
            builder.HasIndex(a => a.MotoristaId)
                .IsUnique()
                .HasDatabaseName("ix_assinaturas_motorista_id");

            // acelera a query do job de lembrete
            builder.HasIndex(a => new { a.Status, a.TrialFimEm })
                .HasDatabaseName("ix_assinaturas_status_trial_fim_em");

            builder.HasQueryFilter(a => !a.Excluido);

            builder.Ignore(a => a.DomainEvents);
        }
    }
}
