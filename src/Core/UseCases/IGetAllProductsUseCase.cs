using JDK.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de obtener todos los productos.
/// </summary>
public interface IGetAllProductsUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para obtener todos los productos de forma asíncrona.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product.</returns>
    IAsyncEnumerable<Product> ExecuteAsync(CancellationToken cancellationToken);
}
