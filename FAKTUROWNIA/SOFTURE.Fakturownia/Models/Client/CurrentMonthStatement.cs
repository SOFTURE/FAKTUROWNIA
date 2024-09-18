namespace SOFTURE.Fakturownia.Models.Client;

public sealed class CurrentMonthStatement
{
    private CurrentMonthStatement(int proFormaInvoiceId, decimal priceGross, DateTime createdAt)
    {
        ProFormaInvoiceId = proFormaInvoiceId;
        PriceGross = priceGross;
        CreatedAt = createdAt;
    }
    
    public int ProFormaInvoiceId { get; set; }
    public int? InvoiceId { get; set; }
    
    public decimal PriceGross { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsPaid => InvoiceId.HasValue;
    
    public static CurrentMonthStatement Create(int proFormaInvoiceId, string priceGross, DateTime createdAt)
    {
        return new CurrentMonthStatement(proFormaInvoiceId, decimal.Parse(priceGross), createdAt);
    }
    
    public void Paid(int invoiceId)
    {
        InvoiceId = invoiceId;
    }
}