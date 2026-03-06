using JDK.Core.Entities;
using JDK.Core.Filters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para el caso de uso de buscar productos con filtros.
/// </summary>
public interface IFindProductsUseCase
{
    /// <summary>
    /// Ejecuta el caso de uso para buscar productos que coincidan con los criterios de filtro especificados de forma asíncrona.
    /// </summary>
    /// <param name="filter">Objeto que contiene los criterios de filtro para la búsqueda.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product que coinciden con el filtro.</returns>
    IAsyncEnumerable<Product> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken);
}
