using JDK.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.UseCases;

/// <summary>
/// Define el contrato para buscar un cliente por su email.
/// </summary>
public interface IFindCustomerByEmailUseCase
{
    Task<Customer?> ExecuteAsync(string email, CancellationToken cancellationToken = default);
}
