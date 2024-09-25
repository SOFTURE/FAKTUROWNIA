namespace SOFTURE.Fakturownia.Models.Client;

public sealed class MonthlyStatement
{
    private MonthlyStatement(Invoice proFormaInvoice)
    {
        ProFormaInvoice = proFormaInvoice;
    }
    
    public Invoice ProFormaInvoice { get; set; }
    public Invoice? Invoice { get; set; }
    
    public bool IsPaid => Invoice != null;
    
    public static MonthlyStatement Create(Invoice proFormaInvoice)
    {
        return new MonthlyStatement(proFormaInvoice);
    }
    
    public void Paid(Invoice invoice)
    {
        Invoice = invoice;
    }
}