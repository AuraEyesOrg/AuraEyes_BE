using Domain.Common;

namespace Domain.Entities.MasterData;

public class District : BaseEntity, Domain.Common.IAggregateRoot
{
    public string Name { get; private set; }
    public int Code { get; private set; }
    public Guid ProvinceId { get; private set; }
    public Province Province { get; private set; } = null!;

    private readonly List<Ward> _wards = new();
    public IReadOnlyCollection<Ward> Wards => _wards.AsReadOnly();

    private District() { }

    public static District Create(string name, int code, Guid provinceId)
    {
        return new District
        {
            Name = name,
            Code = code,
            ProvinceId = provinceId
        };
    }
}
