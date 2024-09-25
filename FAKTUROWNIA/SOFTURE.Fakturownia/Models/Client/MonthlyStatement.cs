namespace SOFTURE.Fakturownia.Models.Client;

public sealed class MonthlyStatement
{
    private MonthlyStatement(decimal priceNet, Invoice proFormaInvoice)
    {
        PriceNet = priceNet;
        ProFormaInvoice = proFormaInvoice;
    }
    
    public Invoice ProFormaInvoice { get; set; }
    public Invoice? Invoice { get; set; }
    
    public decimal PriceNet { get; set; }
    
    public bool IsPaid => Invoice != null;
    
    public static MonthlyStatement Create(string priceNet, Invoice proFormaInvoice)
    {
        return new MonthlyStatement(decimal.Parse(priceNet), proFormaInvoice);
    }
    
    public void Paid(Invoice invoice)
    {
        Invoice = invoice;
    }
}