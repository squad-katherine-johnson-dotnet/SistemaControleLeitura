using Microsoft.EntityFrameworkCore;
using SistemaControleLeitura.Api.Data;
using SistemaControleLeitura.Api.Repositories;
using SistemaControleLeitura.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// busca na configuração a conexão chamada "LeituraDb"

var connectionString = builder.Configuration.GetConnectionString("LeituraDb")
    // se não encontrar, interrompe a inicialização e mostra o motivo
    ?? throw new InvalidOperationException("A connection string 'LeituraDb' não foi encontrada.");

// registra o AppDbContext para usar o SQLite com essa conexão.

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// quando uma classe pedir ILivroRepository, o ASP.NET Core usa LivroRepository

builder.Services.AddScoped<ILivroRepository, LivroRepository>();

// disponibiliza LivroService para injeção de dependencia com uma instancia por requisição
builder.Services.AddScoped<LivroService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
