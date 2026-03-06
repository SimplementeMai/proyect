using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para obtener un producto por su ID.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class GetProductByIdUseCaseImpl(IProductRepository productRepository) : IGetProductByIdUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para obtener un producto específico por su ID de forma asíncrona.
    /// </summary>
    /// <param name="productId">El identificador único del producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene el Product si se encuentra, de lo contrario, null.</returns>
    public Task<Product?> ExecuteAsync(int productId, CancellationToken cancellationToken = default)
    {
        return productRepository.GetByIdAsync(productId, cancellationToken);
    }
}
