using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de actualizar el stock de un producto.
/// </summary>
public interface IUpdateProductStockUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para actualizar el stock de un producto específico de forma asíncrona.
    /// </summary>
    /// <param name="productId">El identificador único del producto cuyo stock se va a actualizar.</param>
    /// <param name="newStock">La nueva cantidad de stock para el producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task ExecuteAsync(int productId, int newStock, CancellationToken cancellationToken);
}
