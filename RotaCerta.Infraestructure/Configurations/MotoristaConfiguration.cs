using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaCerta.Domain.Models;

namespace RotaCerta.Infrastructure.Configurations;

public class MotoristaConfiguration : IEntityTypeConfiguration<Motorista>
{
    public void Configure(EntityTypeBuilder<Motorista> builder)
    {
        builder.ToTable("motoristas");

        // chave primária
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever(); // gerado no Domain

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

        // campos do Motorista
        builder.Property(m => m.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Cnh)
            .HasColumnName("cnh")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.VencimentoCnh)
            .HasColumnName("vencimento_cnh")
            .HasColumnType("date")
            .IsRequired();

        // índice único no CPF
        builder.HasIndex(m => m.Cpf)
            .IsUnique()
            .HasDatabaseName("ix_motoristas_cpf");

        // índice único no Email
        builder.HasIndex(m => m.Email)
            .IsUnique()
            .HasDatabaseName("ix_motoristas_email");

        // índice único na CNH
        builder.HasIndex(m => m.Cnh)
            .IsUnique()
            .HasDatabaseName("ix_motoristas_cnh");

        // filtro global — soft delete
        builder.HasQueryFilter(m => !m.Excluido);

        // relacionamento com Veiculo
        builder.HasMany(m => m.Veiculos)
            .WithOne()
            .HasForeignKey(v => v.MotoristaId)
            .HasConstraintName("fk_veiculos_motorista")
            .OnDelete(DeleteBehavior.Cascade);

        // ignora DomainEvents — não persiste no banco
        builder.Ignore(m => m.DomainEvents);
    }
}