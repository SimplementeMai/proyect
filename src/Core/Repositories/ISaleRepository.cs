using JDK.Core.Entities;
using JDK.Core.Filters;
using System.Collections.Generic; // Para IAsyncEnumerable
using System.Threading; // Para CancellationToken
using System.Threading.Tasks; // Para Task
using System; // Para ArgumentNullException, InvalidOperationException, OperationCanceledException

namespace JDK.Core.Repositories;

/// <summary>
/// Define el contrato para el repositorio de ventas,
/// proporcionando operaciones para gestionar el agregado "Sale" y sus "SaleDetail" asociados.
/// Esta interfaz se adhiere a la Clean Architecture, operando exclusivamente con tipos de dominio.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Recupera todas las ventas de forma asíncrona, permitiendo el streaming de datos.
    /// Incluye la carga de todos los SaleDetails asociados para cada Sale.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Sale.</returns>
    /// <remarks>
    /// Advertencia N+1: La implementación de este método debe ser cuidadosa para evitar
    /// el problema de N+1 consultas al cargar los detalles de cada venta.
    /// Se recomienda el uso de JOINs o Dapper Multi-Mapping en la implementación.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    IAsyncEnumerable<Sale> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera una venta específica por su identificador único de forma asíncrona.
    /// Incluye la carga de todos los SaleDetails asociados para la venta.
    /// </summary>
    /// <param name="saleId">El identificador único de la venta.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene la Sale si se encuentra, de lo contrario, null.</returns>
    /// <remarks>
    /// Advertencia N+1: La implementación de este método debe ser cuidadosa para evitar
    /// el problema de N+1 consultas al cargar los detalles de la venta.
    /// Se recomienda el uso de JOINs o Dapper Multi-Mapping en la implementación.
    /// </remarks>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task<Sale?> GetByIdAsync(int saleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca ventas que coincidan con los criterios de filtro especificados de forma asíncrona.
    /// Incluye la carga de todos los SaleDetails asociados para cada Sale que coincida.
    /// </summary>
    /// <param name="filter">Objeto que contiene los criterios de filtro para la búsqueda. No puede ser nulo.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Sale que coinciden con el filtro.</returns>
    /// <remarks>
    /// Advertencia N+1: La implementación de este método debe ser cuidadosa para evitar
    /// el problema de N+1 consultas al cargar los detalles de cada venta.
    /// Se recomienda el uso de JOINs o Dapper Multi-Mapping en la implementación.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto filter es nulo.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    IAsyncEnumerable<Sale> FindAsync(SaleFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva venta (y sus detalles asociados) al repositorio de forma asíncrona.
    /// </summary>
    /// <param name="sale">El objeto Sale a agregar. No puede ser nulo.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Retorna el objeto Sale persistido, posiblemente con su ID generado.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto sale es nulo.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una venta existente (y sus detalles asociados) en el repositorio de forma asíncrona.
    /// Actualiza el agregado completo.
    /// </summary>
    /// <param name="sale">El objeto Sale a actualizar. El ID debe coincidir con una venta existente. No puede ser nulo.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto sale es nulo.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si la venta no existe en el repositorio y no puede ser actualizada.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
}
