using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using JDK.Core.Filters;
using System.Collections.Generic;
using System.Threading;

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación del caso de uso para alertas de inventario crítico.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class LowStockAlertUseCaseImpl(IProductRepository productRepository) : ILowStockAlertUseCase
{
    public IAsyncEnumerable<Product> ExecuteAsync(int threshold, CancellationToken cancellationToken = default)
    {
        // Utilizamos FindAsync con un filtro de stock máximo
        var filter = new ProductFilter { MaxStock = threshold };
        return productRepository.FindAsync(filter, cancellationToken);
    }
}
