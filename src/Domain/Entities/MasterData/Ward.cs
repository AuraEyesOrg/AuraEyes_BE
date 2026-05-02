using Domain.Common;

namespace Domain.Entities.MasterData;

public class Ward : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public int Code { get; private set; }
    public Guid DistrictId { get; private set; }
    public District District { get; private set; } = null!;

    private Ward() { }

    public static Ward Create(string name, int code, Guid districtId)
    {
        return new Ward
        {
            Name = name,
            Code = code,
            DistrictId = districtId
        };
    }
}
