using JDK.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de obtener un producto por su ID.
/// </summary>
public interface IGetProductByIdUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para obtener un producto específico por su ID de forma asíncrona.
    /// </summary>
    /// <param name="productId">El identificador único del producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene el Product si se encuentra, de lo contrario, null.</returns>
    Task<Product?> ExecuteAsync(int productId, CancellationToken cancellationToken);
}
