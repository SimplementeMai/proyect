using JDK.Core.Entities; // Alias para las entidades de dominio
using InfrastructureEntities = JDK.Infrastructure.Models.Data; // Alias para las entidades de infraestructura
using System; // Para ArgumentNullException
using System.Collections.Generic; // Para List e IReadOnlyList
using System.Linq; // Para Sum

namespace JDK.Infrastructure.Mappers;

/// <summary>
/// Proporciona métodos de extensión para la conversión bidireccional y profunda
/// entre las entidades de dominio Sale/SaleDetail y las entidades de persistencia VentaEntity/DetalleVentaEntity.
/// Diseñado para ser 100% estático, inmutable y compatible con Native AOT.
/// </summary>
public static partial class SaleMapper
{
    /// <summary>
    /// Convierte una instancia de DetalleVentaEntity a una instancia de SaleDetail.
    /// Requiere una función para resolver la entidad Product de dominio a partir de su ID.
    /// </summary>
    /// <param name="entity">La entidad de persistencia DetalleVentaEntity a convertir.</param>
    /// <param name="getProductById">Función para obtener un Product de dominio dado su ProductID.</param>
    /// <returns>Una nueva instancia de la entidad de dominio SaleDetail.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de entrada o getProductById es nula.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si no se puede encontrar el producto asociado.</exception>
    public static SaleDetail ToDomain(this InfrastructureEntities.DetalleVentaEntity entity, Func<int, Product> getProductById)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(getProductById);

        var product = getProductById(entity.ProductoID) 
            ?? throw new InvalidOperationException($"Product with ID {entity.ProductoID} not found for SaleDetail mapping.");

        return new SaleDetail(entity.DetalleID, product, entity.Cantidad)
        {
            // UnitPrice ya se inicializa desde product.Price en el constructor de SaleDetail
            // TotalDetail es una propiedad calculada en el dominio.
            // Establecer explicitamente propiedades individuales si no se manejan en el constructor.
            // Para el mapeo de BD a Dominio, se asume que DetalleID, Producto, Cantidad son suficientes.
            // PrecioUnitario de la entidad es el precio histórico, que coincide con el constructor de dominio.
            // Si TotalDetalle se necesitara para validación, se calcularía en el dominio.
        };
    }

    /// <summary>
    /// Convierte una instancia de VentaEntity y su lista de DetalleVentaEntity asociadas
    /// a una instancia de Sale de la capa de dominio.
    /// </summary>
    /// <param name="entity">La entidad de persistencia VentaEntity a convertir.</param>
    /// <param name="detalleEntities">La lista de entidades de persistencia DetalleVentaEntity asociadas.</param>
    /// <param name="getProductById">Función para obtener un Product de dominio dado su ProductID.</param>
    /// <returns>Una nueva instancia de la entidad de dominio Sale.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de entrada, la lista de detalles o getProductById es nula.</exception>
    public static Sale ToDomain(this InfrastructureEntities.VentaEntity entity, 
                                List<InfrastructureEntities.DetalleVentaEntity> detalleEntities,
                                Func<int, Product> getProductById)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(detalleEntities);
        ArgumentNullException.ThrowIfNull(getProductById);

        var sale = new Sale(entity.VentaID, entity.Folio)
        {
            SaleDate = entity.FechaVenta,
            CustomerId = entity.ClienteID,
            Status = (SaleStatus)entity.Estatus // Mapeo explícito de byte a enum
        };

        foreach (var detalleEntity in detalleEntities)
        {
            // Asegura que el DetalleVentaEntity pertenezca a esta VentaEntity
            if (detalleEntity.VentaID == entity.VentaID)
            {
                sale.AddSaleDetail(detalleEntity.ToDomain(getProductById));
            }
        }

        return sale;
    }

    /// <summary>
    /// Convierte una instancia de Sale de la capa de dominio a una instancia de VentaEntity.
    /// </summary>
    /// <param name="domain">La entidad de dominio Sale a convertir.</param>
    /// <returns>Una nueva instancia de la entidad de persistencia VentaEntity.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de dominio de entrada es nula.</exception>
    public static InfrastructureEntities.VentaEntity ToEntity(this Sale domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        return new InfrastructureEntities.VentaEntity(domain.SaleID, domain.Folio)
        {
            FechaVenta = domain.SaleDate,
            Estatus = (byte)domain.Status, // Mapeo explícito de enum a byte
            ClienteID = domain.CustomerId,
            TotalArticulos = domain.TotalItems, // Se usa la propiedad calculada del dominio
            TotalVenta = domain.TotalSale // Se usa la propiedad calculada del dominio
        };
    }

    /// <summary>
    /// Convierte una instancia de SaleDetail de la capa de dominio a una instancia de DetalleVentaEntity.
    /// </summary>
    /// <param name="domain">La entidad de dominio SaleDetail a convertir.</param>
    /// <param name="ventaId">El ID de la venta a la que pertenece este detalle.</param>
    /// <returns>Una nueva instancia de la entidad de persistencia DetalleVentaEntity.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de dominio de entrada es nula.</exception>
    public static InfrastructureEntities.DetalleVentaEntity ToEntity(this SaleDetail domain, int ventaId)
    {
        ArgumentNullException.ThrowIfNull(domain);

        return new InfrastructureEntities.DetalleVentaEntity(domain.SaleDetailID, ventaId, domain.Product.ProductID)
        {
            PrecioUnitario = domain.UnitPrice,
            Cantidad = domain.Quantity,
            TotalDetalle = domain.TotalDetail // Se usa la propiedad calculada del dominio
        };
    }
}
