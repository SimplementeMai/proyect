using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using JDK.Core.Configuration;
using JDK.Infrastructure.Data.Connection;
using JDK.Core.Repositories; // Necesario para IProductRepository
using JDK.Infrastructure.Repositories; // Necesario para ProductRepositoryImpl
using System.ComponentModel.DataAnnotations; // For ValidateDataAnnotations

namespace JDK.Infrastructure.DependencyInjection;

/// <summary>
/// Proporciona métodos de extensión para IServiceCollection para registrar
/// los servicios relacionados con la base de datos de forma limpia y configurable.
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>
    /// Registra los servicios necesarios para la conexión a la base de datos y factorías.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <param name="configuration">La configuración de la aplicación.</param>
    /// <returns>La instancia de IServiceCollection para encadenamiento.</returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configurar DatabaseOptions y validar al inicio
        services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.DatabaseSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart(); // Recomendado por la política de seguridad.

        // 2. Registrar la factoría de conexiones
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        // 3. Registrar los repositorios específicos
        services.AddScoped<IProductRepository, ProductRepositoryImpl>();
        services.AddScoped<ICustomerRepository, CustomerRepositoryImpl>();
        services.AddScoped<ISaleRepository, SaleRepositoryImpl>();

        return services;
    }
}
