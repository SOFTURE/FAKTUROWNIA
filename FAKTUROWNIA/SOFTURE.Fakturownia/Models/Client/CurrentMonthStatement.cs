namespace SOFTURE.Fakturownia.Models.Client;

public sealed class CurrentMonthStatement
{
    private CurrentMonthStatement(decimal priceNet, Invoice proFormaInvoice)
    {
        PriceNet = priceNet;
        ProFormaInvoice = proFormaInvoice;
    }
    
    public Invoice ProFormaInvoice { get; set; }
    public Invoice? Invoice { get; set; }
    
    public decimal PriceNet { get; set; }
    
    public bool IsPaid => Invoice != null;
    
    public static CurrentMonthStatement Create(string priceNet, Invoice proFormaInvoice)
    {
        return new CurrentMonthStatement(decimal.Parse(priceNet), proFormaInvoice);
    }
    
    public void Paid(Invoice invoice)
    {
        Invoice = invoice;
    }
}