namespace JDK.Infrastructure.Models.Data;

public partial class VentaEntity(int ventaId, string folio)
{
    public int VentaID { get; init; } = ventaId;

    public string Folio { get; init; } = folio;

    public DateTime FechaVenta { get; set; } = DateTime.Now; // Default to current time, can be set from DB

    private byte _estatus; // TINYINT maps to byte in C#
    public byte Estatus
    {
        get => _estatus;
        set
        {
            // CHECK (Estatus IN (1, 2, 3))
            if (value is not (1 or 2 or 3))
            {
                throw new ArgumentOutOfRangeException(nameof(Estatus), "El estatus debe ser 1 (Pendiente), 2 (Completada) o 3 (Cancelada).");
            }
            _estatus = value;
        }
    }

    public int? ClienteID { get; set; }

    public int TotalArticulos { get; set; }

    public decimal TotalVenta { get; set; }
}
