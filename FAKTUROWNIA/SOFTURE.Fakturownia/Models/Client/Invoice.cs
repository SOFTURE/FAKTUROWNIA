using CSharpFunctionalExtensions;

namespace SOFTURE.Fakturownia.Models.Client;

public sealed class Invoice : ValueObject
{
    public Invoice(int identifier, DateTime createdAt)
    {
        Identifier = identifier;
        CreatedAt = createdAt;
    }
    
    public int Identifier { get; }
    public DateTime CreatedAt { get; }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Identifier;
        yield return CreatedAt;
    }
}