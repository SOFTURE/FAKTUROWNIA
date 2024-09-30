using CSharpFunctionalExtensions;

namespace SOFTURE.Fakturownia.Models.Client;

public sealed class Invoice : ValueObject
{
    public Invoice(int identifier, DateTime createdAt, string priceNet)
    {
        Identifier = identifier;
        CreatedAt = createdAt;
        PriceNet = decimal.Parse(priceNet);
    }
    
    public int Identifier { get; }
    public DateTime CreatedAt { get; }
    public decimal PriceNet { get;  }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Identifier;
        yield return CreatedAt;
        yield return PriceNet;
    }
}