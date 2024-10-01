using CSharpFunctionalExtensions;

namespace SOFTURE.Fakturownia.Models.Client;

public sealed class Invoice : ValueObject
{
    public Invoice(int identifier, DateTime issueDate, string priceNet)
    {
        Identifier = identifier;
        IssueDate = issueDate;
        PriceNet = decimal.Parse(priceNet);
    }
    
    public int Identifier { get; }
    public DateTime IssueDate { get; }
    public decimal PriceNet { get;  }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Identifier;
        yield return IssueDate;
        yield return PriceNet;
    }
}