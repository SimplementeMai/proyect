using JDK.Core.Entities; // Necesario para SaleStatus
using System; // Necesario para DateTime

namespace JDK.Core.Filters;

/// <summary>
/// Representa los criterios para filtrar la búsqueda de ventas.
/// Implementado como un record inmutable para consistencia de datos y compatibilidad AOT.
/// </summary>
public record SaleFilter
{
    /// <summary>
    /// Filtra por folio de venta (búsqueda exacta o parcial).
    /// </summary>
    public string? Folio { get; init; }

    /// <summary>
    /// Filtra ventas con fecha igual o posterior a esta.
    /// </summary>
    public DateTime? MinSaleDate { get; init; }

    /// <summary>
    /// Filtra ventas con fecha igual o anterior a esta.
    /// </summary>
    public DateTime? MaxSaleDate { get; init; }

    /// <summary>
    /// Filtra por estado de la venta.
    /// </summary>
    public SaleStatus? Status { get; init; }

    /// <summary>
    /// Filtra ventas con un total de artículos igual o superior a este valor.
    /// </summary>
    public int? MinTotalItems { get; init; }

    /// <summary>
    /// Filtra ventas con un total de artículos igual o inferior a este valor.
    /// </summary>
    public int? MaxTotalItems { get; init; }

    /// <summary>
    /// Filtra ventas con un total monetario igual o superior a este valor.
    /// </summary>
    public decimal? MinTotalSale { get; init; }

    /// <summary>
    /// Filtra ventas con un total monetario igual o inferior a este valor.
    /// </summary>
    public decimal? MaxTotalSale { get; init; }
}
