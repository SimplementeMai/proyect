using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;
using System; // For ArgumentNullException

namespace JDK.Application.ProductUseCases;

/// <summary>
/// Implementación concreta del caso de uso para crear un nuevo producto.
/// </summary>
/// <param name="productRepository">El repositorio de productos.</param>
public class CreateProductUseCaseImpl(IProductRepository productRepository) : ICreateProductUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para crear un nuevo producto de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a crear.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene el objeto Product creado (posiblemente con su ID asignado).</returns>
    public async Task<Product> ExecuteAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        return await productRepository.AddAsync(product, cancellationToken);
    }
}
