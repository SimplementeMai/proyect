using JDK.Core.Entities;
using System.Collections.Generic; // For IAsyncEnumerable
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task

namespace JDK.Core.UseCases;

/// <summary>
/// Defines the contract for the use case of fetching all sales.
/// Single Responsibility: Retrieve a collection of all available sales records.
/// </summary>
public interface IFetchAllSalesUseCase
{
    /// <summary>
    /// Executes the use case to fetch all sales asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An IAsyncEnumerable of Sale, allowing for data streaming.</returns>
    IAsyncEnumerable<Sale> ExecuteAsync(CancellationToken cancellationToken);
}
