using JDK.Core.Entities;
using JDK.Core.Repositories;
using JDK.Core.UseCases;
using System.Threading;
using System.Threading.Tasks;

namespace JDK.Application.SaleUseCases;

/// <summary>
/// Implementación del caso de uso para crear una nueva venta.
/// </summary>
public class CreateSaleUseCaseImpl(ISaleRepository saleRepository) : ICreateSaleUseCase
{
    public Task<Sale> ExecuteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        return saleRepository.AddAsync(sale, cancellationToken);
    }
}
