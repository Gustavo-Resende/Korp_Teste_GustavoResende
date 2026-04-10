using Faturamento.Application.Abstractions;
using Faturamento.Infrastructure.Http;
using Faturamento.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faturamento.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();

        var estoqueUrl = configuration["Services:Estoque"];
        if (string.IsNullOrWhiteSpace(estoqueUrl))
            throw new InvalidOperationException("Configuração 'Services:Estoque' é obrigatória (URL base da API de estoque).");

        services.AddHttpClient(EstoqueApiClient.HttpClientName, client =>
        {
            client.BaseAddress = new Uri(estoqueUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<IEstoqueApiClient, EstoqueApiClient>();

        return services;
    }
}
