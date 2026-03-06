using JDK.Core.Entities; // For SaleStatus enum
using System.Threading; // For CancellationToken
using System.Threading.Tasks; // For Task
using System; // For ArgumentOutOfRangeException

namespace JDK.Core.UseCases;

/// <summary>
/// Defines the contract for the use case of updating the status of an existing sale.
/// Single Responsibility: Modify specifically the status property of a sale.
/// </summary>
public interface IUpdateSaleStatusUseCase
{
    /// <summary>
    /// Executes the use case to update the status of a specific sale asynchronously.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale whose status is to be updated.</param>
    /// <param name="newStatus">The new status for the sale.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the newStatus is an invalid SaleStatus value.</exception>
    Task ExecuteAsync(int saleId, SaleStatus newStatus, CancellationToken cancellationToken);
}
