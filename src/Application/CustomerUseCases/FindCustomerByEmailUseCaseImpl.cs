using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Application.CustomerUseCases;

/// <summary>
/// Implementación del caso de uso para buscar un cliente por su email.
/// </summary>
public class FindCustomerByEmailUseCaseImpl(ICustomerRepository customerRepository) : IFindCustomerByEmailUseCase
{
    public Task<Customer?> ExecuteAsync(string email, CancellationToken cancellationToken = default)
    {
        return customerRepository.GetByEmailAsync(email, cancellationToken);
    }
}
