using JDK.Core.Entities;
using JDK.Core.Filters;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System; // For ArgumentNullException

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para buscar productos con filtros.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class FindProductsUseCaseImpl(IProductRepository productRepository) : IFindProductsUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para buscar productos que coincidan con los criterios de filtro especificados de forma asíncrona.
    /// </summary>
    /// <param name="filter">Objeto que contiene los criterios de filtro para la búsqueda.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product que coinciden con el filtro.</returns>
    public IAsyncEnumerable<Product> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return productRepository.FindAsync(filter, cancellationToken);
    }
}
