using JDK.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de actualizar un producto existente.
/// </summary>
public interface IUpdateProductUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para actualizar un producto existente de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a actualizar.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task ExecuteAsync(Product product, CancellationToken cancellationToken);
}
