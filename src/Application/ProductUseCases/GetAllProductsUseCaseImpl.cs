using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para obtener todos los productos.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class GetAllProductsUseCaseImpl(IProductRepository productRepository) : IGetAllProductsUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para obtener todos los productos de forma asíncrona.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product.</returns>
    public IAsyncEnumerable<Product> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return productRepository.GetAllAsync(cancellationToken);
    }
}
