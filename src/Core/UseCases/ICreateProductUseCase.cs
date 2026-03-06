using JDK.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de crear un nuevo producto.
/// </summary>
public interface ICreateProductUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para crear un nuevo producto de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a crear.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene el objeto Product creado (posiblemente con su ID asignado).</returns>
    Task<Product> ExecuteAsync(Product product, CancellationToken cancellationToken);
}
