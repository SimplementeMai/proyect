using JDK.Core.Entities;
using JDK.Core.Filters; // For SaleFilter
using System.Collections.Generic; // For IAsyncEnumerable
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task

namespace JDK.Core.UseCases;

/// <summary>
/// Defines the contract for the use case of fetching sales based on specified filters.
/// Single Responsibility: Filter and retrieve sales records based on specific criteria.
/// </summary>
public interface IFetchSalesByFilterUseCase
{
    /// <summary>
    /// Executes the use case to fetch sales that match the specified filter criteria asynchronously.
    /// </summary>
    /// <param name="filter">Object containing the filter criteria for the search. Cannot be null.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An IAsyncEnumerable of Sale that matches the filter, allowing for data streaming.</returns>
    IAsyncEnumerable<Sale> ExecuteAsync(SaleFilter filter, CancellationToken cancellationToken);
}
