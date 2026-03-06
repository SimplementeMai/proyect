namespace JDK.Core.Entities;

public partial class SaleDetail(int saleDetailId, Product product, int quantity)
{
    public int SaleDetailID { get; init; } = saleDetailId;

    public Product Product { get; init; } = product;

    // UnitPrice se captura en el momento de la venta para preservar el precio histórico
    public decimal UnitPrice { get; init; } = product.Price;

    private int _quantity = quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "La cantidad no puede ser negativa.");
            }
            _quantity = value;
        }
    }

    public decimal TotalDetail => UnitPrice * Quantity;
}
