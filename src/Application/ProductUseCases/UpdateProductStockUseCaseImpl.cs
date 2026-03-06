using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;
using System; // For ArgumentOutOfRangeException

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para actualizar el stock de un producto.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class UpdateProductStockUseCaseImpl(IProductRepository productRepository) : IUpdateProductStockUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para actualizar el stock de un producto específico de forma asíncrona.
    /// </summary>
    /// <param name="productId">El identificador único del producto cuyo stock se va a actualizar.</param>
    /// <param name="newStock">La nueva cantidad de stock para el producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task ExecuteAsync(int productId, int newStock, CancellationToken cancellationToken = default)
    {
        if (productId <= 0) // Basic validation
        {
            throw new ArgumentOutOfRangeException(nameof(productId), "El ID del producto debe ser mayor que cero.");
        }
        if (newStock < 0) // Basic validation
        {
            throw new ArgumentOutOfRangeException(nameof(newStock), "El nuevo stock no puede ser negativo.");
        }
        await productRepository.UpdateStockAsync(productId, newStock, cancellationToken);
    }
}
