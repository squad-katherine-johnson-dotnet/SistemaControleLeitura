using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SistemaControleLeitura.Application.Interfaces;
using SistemaControleLeitura.Application.Services;
using SistemaControleLeitura.Domain.Interfaces;
using SistemaControleLeitura.Infrastructure.Data;
using SistemaControleLeitura.Infrastructure.Repositories;
using SistemaControleLeitura.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var connectionString = builder.Configuration.GetConnectionString("LeituraDb")
    ?? throw new InvalidOperationException("A connection string 'LeituraDb' não foi encontrada.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

var secretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("A chave secreta do JWT não foi configurada.");

if (Encoding.UTF8.GetByteCount(secretKey) < 32)
    throw new InvalidOperationException("A chave secreta do JWT deve possuir pelo menos 32 bytes.");

var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("O emissor do JWT não foi configurado.");

var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("O público do JWT não foi configurado.");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        ),

        ClockSkew = TimeSpan.Zero
    };
});

// Cadastro & Login
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Metas
// Livros
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<ILivroService, LivroService>();

builder.Services.AddScoped<IMetaAnualRepository, MetaAnualRepository>();
builder.Services.AddScoped<IMetaAnualService, MetaAnualService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
