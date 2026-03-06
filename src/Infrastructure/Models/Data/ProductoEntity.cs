namespace JDK.Infrastructure.Models.Data;

public partial class ProductoEntity(int productoId, string sku)
{
    public int ProductoID { get; init; } = productoId;

    public string Nombre { get; set; } = string.Empty;

    public string SKU { get; init; } = sku;

    public string Marca { get; set; } = string.Empty;

    private decimal _precio;
    public decimal Precio
    {
        get => _precio;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Precio), "El precio no puede ser negativo.");
            }
            _precio = value;
        }
    }

    private int _stock;
    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Stock), "El stock no puede ser negativo.");
            }
            _stock = value;
        }
    }
}
