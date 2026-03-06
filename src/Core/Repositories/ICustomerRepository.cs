using JDK.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Core.Repositories;

/// <summary>
/// Contrato para la gestión de persistencia de clientes.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Busca un cliente por su dirección de correo electrónico.
    /// </summary>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un nuevo cliente en el sistema.
    /// </summary>
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
