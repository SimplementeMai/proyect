using JDK.Core.Entities;
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task

namespace JDK.Core.UseCases;

/// <summary>
/// Defines the contract for the use case of fetching a specific sale by its ID.
/// Single Responsibility: Retrieve a specific sale record using its unique identifier.
/// </summary>
public interface IFetchSaleByIdUseCase
{
    /// <summary>
    /// Executes the use case to fetch a specific sale by its ID asynchronously.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation. Contains the Sale if found, otherwise null.</returns>
    Task<Sale?> ExecuteAsync(int saleId, CancellationToken cancellationToken);
}
