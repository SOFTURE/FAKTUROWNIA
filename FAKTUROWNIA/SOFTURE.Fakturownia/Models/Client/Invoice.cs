using CSharpFunctionalExtensions;

namespace SOFTURE.Fakturownia.Models.Client;

public sealed class Invoice : ValueObject
{
    public Invoice(int identifier, string number, DateTime issueDate, DateTime paymentDate, string priceNet)
    {
        Identifier = identifier;
        Number = number;
        IssueDate = issueDate;
        PaymentDate = paymentDate;
        PriceNet = decimal.Parse(priceNet);
    }
    
    public int Identifier { get; }
    public string Number { get; }
    public DateTime IssueDate { get; }
    public DateTime PaymentDate { get; }
    public decimal PriceNet { get;  }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Identifier;
        yield return Number;
        yield return IssueDate;
        yield return PaymentDate;
        yield return PriceNet;
    }
}