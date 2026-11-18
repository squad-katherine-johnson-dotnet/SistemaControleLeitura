using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Domain.Entities;
namespace SistemaControleLeitura.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        
    }

    public DbSet<Usuario> Usuarios { get; set;} = null!;
    public DbSet<Livro> Livros { get; set;} = null!;
    public DbSet<Leitura> Leituras { get; set;} = null!;
    public DbSet<MetaAnual> MetasAnuais { get; set;} = null!;
    public DbSet<ItemEstante> ItensEstante { get; set;} = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Não permite que o email se repita

        modelBuilder.Entity<Usuario>(entity => {
            entity.HasKey(p => p.IdUsuario);
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(120);
            entity.Property(p => p.Email).IsRequired().HasMaxLength(254);
            entity.Property(p => p.SenhaHash).IsRequired().HasMaxLength(256);
            entity.HasIndex(p => p.Email).IsUnique();
        });

        // Cada livro pertence a uma usuaria
        // UsuarioId guarda o ID da dona do livro

        modelBuilder.Entity<Livro>()
        .HasOne<Usuario>()
        .WithMany()
        .HasForeignKey(livro =>livro.UsuarioId);

        // Cada ciclo de leitura pertence a 1 livro
        // 1 livro pode ter varios ciclos (releituras)
        // LivroId identifica o livro relacionado
        // restrict impede excluir um livro que tenha ciclos registrados

        modelBuilder.Entity<Leitura>(e =>
        {
            e.Property(l => l.Status).HasConversion<string>();   // grava "Lido" no banco, não 2
            e.HasOne<ItemEstante>()
            .WithMany()
            .HasForeignKey(l => l.ItemEstanteId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // Nao deixa uma usuaria com duas metas para o mesmo ano

        modelBuilder.Entity<MetaAnual>()
        .HasIndex(meta => new { meta.UsuarioId, meta.Ano }) 
        .IsUnique();

        // cada mta pertence a uma usuaria
        // uma usuaria pode ter metas de anos diferetnes
        // usuarioId guarda o id da usuaria

        modelBuilder.Entity<MetaAnual>()
        .HasOne<Usuario>()
        .WithMany()
        .HasForeignKey(meta => meta.UsuarioId);

        // um livro so pode teer um item correspondente na estante
        // livro id guarda o id do libro

        modelBuilder.Entity<ItemEstante>()
        .HasOne<Livro>()
        .WithOne()
        .HasForeignKey<ItemEstante>(item => item.LivroId)
        .OnDelete(DeleteBehavior.Restrict);

    

    }
}