using JDK.Core.Entities;
using JDK.Core.Filters;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Collections.Generic;
using System.Threading;

namespace JDK.Application.SaleUseCases;

/// <summary>
/// Implementación del caso de uso para buscar ventas por filtro.
/// </summary>
public class FetchSalesByFilterUseCaseImpl(ISaleRepository saleRepository) : IFetchSalesByFilterUseCase
{
    public IAsyncEnumerable<Sale> ExecuteAsync(SaleFilter filter, CancellationToken cancellationToken = default)
    {
        return saleRepository.FindAsync(filter, cancellationToken);
    }
}
