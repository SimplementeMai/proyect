namespace JDK.Core.Entities;

public enum SaleStatus : byte
{
    Pending = 1,
    Completed = 2,
    Canceled = 3
}

public partial class Sale(int saleId, string folio)
{
    public int SaleID { get; init; } = saleId;

    public string Folio { get; init; } = folio;

    public DateTime SaleDate { get; init; } = DateTime.Now;

    public int? CustomerId { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Pending;

    private readonly List<SaleDetail> _saleDetails = new();
    public IReadOnlyList<SaleDetail> SaleDetails => _saleDetails.AsReadOnly();

    public void AddSaleDetail(SaleDetail detail)
    {
        if (detail == null)
        {
            throw new ArgumentNullException(nameof(detail));
        }
        _saleDetails.Add(detail);
    }

    public int TotalItems => _saleDetails.Sum(detail => detail.Quantity);

    public decimal TotalSale => _saleDetails.Sum(detail => detail.TotalDetail);
}
