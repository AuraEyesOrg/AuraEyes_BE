using Domain.Common;

namespace Domain.Entities.MasterData;

public class Country : BaseEntity, Domain.Common.IAggregateRoot
{
    public string Name { get; private set; }
    public string IsoCode { get; private set; } // cca2

    private Country() { }

    public static Country Create(string name, string isoCode)
    {
        return new Country
        {
            Name = name,
            IsoCode = isoCode
        };
    }
}
