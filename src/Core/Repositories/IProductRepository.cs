using JDK.Core.Entities;
using JDK.Core.Filters;
using System.Collections.Generic; // Para IAsyncEnumerable
using System.Threading; // Para CancellationToken
using System.Threading.Tasks; // Para Task

namespace JDK.Core.Repositories;

/// <summary>
/// Define el contrato para el repositorio de productos,
/// proporcionando operaciones para gestionar entidades Product.
/// Esta interfaz se adhiere a la Clean Architecture, operando exclusivamente con tipos de dominio.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Recupera todos los productos de forma asíncrona, permitiendo el streaming de datos.
    /// </summary>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product.</returns>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera un producto específico por su identificador único de forma asíncrona.
    /// </summary>
    /// <param name="productId">El identificador único del producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Contiene el Product si se encuentra, de lo contrario, null.</returns>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca productos que coincidan con los criterios de filtro especificados de forma asíncrona.
    /// </summary>
    /// <param name="filter">Objeto que contiene los criterios de filtro para la búsqueda.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Un IAsyncEnumerable de Product que coinciden con el filtro.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto filter es nulo.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    IAsyncEnumerable<Product> FindAsync(ProductFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo producto al repositorio de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a agregar.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Retorna el objeto Product persistido, posiblemente con su ID generado.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto product es nulo.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un producto existente en el repositorio de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a actualizar. El ID debe coincidir con un producto existente.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si el objeto product es nulo.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si el producto no existe en el repositorio.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza de forma asíncrona la cantidad de stock de un producto específico.
    /// Este es un método especializado para una actualización parcial y atómica.
    /// </summary>
    /// <param name="productId">El identificador único del producto.</param>
    /// <param name="newStock">La nueva cantidad de stock para el producto.</param>
    /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el producto no existe en el repositorio.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Se lanza si newStock es negativo.</exception>
    /// <exception cref="OperationCanceledException">Se lanza si la operación es cancelada.</exception>
    Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default);
}
