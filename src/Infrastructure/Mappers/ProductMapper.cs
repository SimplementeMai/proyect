using JDK.Core.Entities; // Alias for the Domain Product
using InfrastructureProductoEntity = JDK.Infrastructure.Models.Data.ProductoEntity; // Alias for the Infrastructure ProductEntity

namespace JDK.Infrastructure.Mappers;

/// <summary>
/// Proporciona métodos de extensión para la conversión bidireccional
/// entre la entidad de dominio Product y la entidad de persistencia ProductoEntity.
/// Diseñado para ser 100% estático y compatible con Native AOT.
/// </summary>
public static partial class ProductMapper
{
    /// <summary>
    /// Convierte una instancia de ProductoEntity de la capa de infraestructura
    /// a una instancia de Product de la capa de dominio.
    /// </summary>
    /// <param name="entity">La entidad de persistencia ProductoEntity a convertir.</param>
    /// <returns>Una nueva instancia de la entidad de dominio Product.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de entrada es nula.</exception>
    public static Product ToDomain(this InfrastructureProductoEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Uso del constructor primario de Product y asignación de propiedades.
        // Las validaciones de Precio y Stock se ejecutan en los setters del dominio.
        return new Product(entity.ProductoID, entity.SKU)
        {
            Name = entity.Nombre,
            Brand = entity.Marca,
            Price = entity.Precio,
            Stock = entity.Stock
        };
    }

    /// <summary>
    /// Convierte una instancia de Product de la capa de dominio
    /// a una instancia de ProductoEntity de la capa de infraestructura.
    /// </summary>
    /// <param name="domain">La entidad de dominio Product a convertir.</param>
    /// <returns>Una nueva instancia de la entidad de persistencia ProductoEntity.</returns>
    /// <exception cref="ArgumentNullException">Se lanza si la entidad de dominio de entrada es nula.</exception>
    public static InfrastructureProductoEntity ToEntity(this Product domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        // Uso del constructor primario de ProductoEntity y asignación de propiedades.
        return new InfrastructureProductoEntity(domain.ProductID, domain.SKU)
        {
            Nombre = domain.Name,
            Marca = domain.Brand,
            Precio = domain.Price,
            Stock = domain.Stock
        };
    }
}
