using JDK.Core.Entities;
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task

namespace JDK.Core.UseCases;

/// <summary>
/// Defines the contract for the use case of creating a new sale.
/// Single Responsibility: Orchestrate the logic to create and persist a new sales record.
/// </summary>
public interface ICreateSaleUseCase
{
    /// <summary>
    /// Executes the use case to create a new sale asynchronously.
    /// </summary>
    /// <param name="sale">The Sale object to create. Cannot be null.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation. Contains the created Sale object (potentially with its generated ID).</returns>
    Task<Sale> ExecuteAsync(Sale sale, CancellationToken cancellationToken);
}
