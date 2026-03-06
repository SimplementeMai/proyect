namespace JDK.Infrastructure.Models.Data;

public partial class DetalleVentaEntity(int detalleId, int ventaId, int productoId)
{
    public int DetalleID { get; init; } = detalleId;

    public int VentaID { get; init; } = ventaId;

    public int ProductoID { get; init; } = productoId;

    private decimal _precioUnitario;
    public decimal PrecioUnitario
    {
        get => _precioUnitario;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(PrecioUnitario), "El precio unitario no puede ser negativo.");
            }
            _precioUnitario = value;
        }
    }

    private int _cantidad;
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Cantidad), "La cantidad no puede ser negativa.");
            }
            _cantidad = value;
        }
    }

    public decimal TotalDetalle { get; set; }
}
