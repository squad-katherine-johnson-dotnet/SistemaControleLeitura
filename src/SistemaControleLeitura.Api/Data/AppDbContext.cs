using Microsoft.EntityFrameworkCore;
namespace SistemaControleLeitura.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        
    }
}