using JDK.Core.Entities;
using System.Collections.Generic;
using System.Threading;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de alertas de inventario crítico.
/// </summary>
public interface ILowStockAlertUseCase
{
    /// <summary>
    /// Recupera los productos que están por debajo o igual al umbral especificado.
    /// </summary>
    /// <param name="threshold">El umbral de stock.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product.</returns>
    IAsyncEnumerable<Product> ExecuteAsync(int threshold, CancellationToken cancellationToken = default);
}
