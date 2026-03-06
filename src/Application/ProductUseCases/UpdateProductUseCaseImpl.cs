using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;
using System; // For ArgumentNullException

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para actualizar un producto existente.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class UpdateProductUseCaseImpl(IProductRepository productRepository) : IUpdateProductUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para actualizar un producto existente de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a actualizar.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task ExecuteAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        await productRepository.UpdateAsync(product, cancellationToken);
    }
}
