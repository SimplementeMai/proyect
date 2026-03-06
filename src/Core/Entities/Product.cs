namespace JDK.Core.Entities;

public partial class Product(int productId, string sku)
{
    public int ProductID { get; init; } = productId;

    public string Name { get; set; } = string.Empty;

    public string SKU { get; init; } = sku;

    public string Brand { get; set; } = string.Empty;

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Price), "El precio no puede ser negativo.");
            }
            _price = value;
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
