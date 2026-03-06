namespace JDK.Core.Filters;

/// <summary>
/// Representa los criterios para filtrar la búsqueda de productos.
/// </summary>
public partial class ProductFilter
{
    /// <summary>
    /// Filtra por nombre de producto (búsqueda parcial).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtra por SKU exacto.
    /// </summary>
    public string? SKU { get; set; }

    /// <summary>
    /// Filtra por marca (búsqueda parcial).
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Filtra productos con un precio mínimo.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Filtra productos con un precio máximo.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Filtra productos con un stock mínimo.
    /// </summary>
    public int? MinStock { get; set; }

    /// <summary>
    /// Filtra productos con un stock máximo.
    /// </summary>
    public int? MaxStock { get; set; }
}
