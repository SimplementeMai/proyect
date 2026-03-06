using Microsoft.Extensions.DependencyInjection;
using JDK.Application.ProductUseCases;
using JDK.Core.UseCases;
using JDK.Core.Repositories; // Necesario para IProductRepository (dependencia de los casos de uso)

namespace JDK.Infrastructure.DependencyInjection;

/// <summary>
/// Proporciona métodos de extensión para IServiceCollection para registrar
/// los servicios de la capa de aplicación (casos de uso).
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Registra los casos de uso del dominio de Producto y sus dependencias en el contenedor de DI.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <returns>La instancia de IServiceCollection para encadenamiento.</returns>
    public static IServiceCollection AddProductUseCases(this IServiceCollection services)
    {
        // Registro de repositorios (si aún no se han registrado, aunque AddDatabaseServices ya los incluye)
        // services.AddScoped<IProductRepository, ProductRepositoryImpl>(); // Ejemplo, ya registrado en DatabaseServiceCollectionExtensions

        // Registro de casos de uso de Productos
        services.AddScoped<IGetAllProductsUseCase, GetAllProductsUseCaseImpl>();
        services.AddScoped<IGetProductByIdUseCase, GetProductByIdUseCaseImpl>();
        services.AddScoped<IFindProductsUseCase, FindProductsUseCaseImpl>();
        services.AddScoped<ICreateProductUseCase, CreateProductUseCaseImpl>();
        services.AddScoped<IUpdateProductUseCase, UpdateProductUseCaseImpl>();
        services.AddScoped<IUpdateProductStockUseCase, UpdateProductStockUseCaseImpl>();
        services.AddScoped<ILowStockAlertUseCase, LowStockAlertUseCaseImpl>();

        return services;
    }

    /// <summary>
    /// Registra los casos de uso del dominio de Venta y sus dependencias en el contenedor de DI.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <returns>La instancia de IServiceCollection para encadenamiento.</returns>
    public static IServiceCollection AddSaleUseCases(this IServiceCollection services)
    {
        services.AddScoped<IFetchSalesByFilterUseCase, JDK.Application.SaleUseCases.FetchSalesByFilterUseCaseImpl>();
        services.AddScoped<ICreateSaleUseCase, JDK.Application.SaleUseCases.CreateSaleUseCaseImpl>();
        // Otros casos de uso de ventas podrían registrarse aquí
        return services;
    }

    /// <summary>
    /// Registra los casos de uso del dominio de Cliente y sus dependencias en el contenedor de DI.
    /// </summary>
    /// <param name="services">La colección de servicios a extender.</param>
    /// <returns>La instancia de IServiceCollection para encadenamiento.</returns>
    public static IServiceCollection AddCustomerUseCases(this IServiceCollection services)
    {
        services.AddScoped<IFindCustomerByEmailUseCase, JDK.Application.CustomerUseCases.FindCustomerByEmailUseCaseImpl>();
        return services;
    }
}
