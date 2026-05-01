using Domain.Common;

namespace Domain.Entities.MasterData;

public class Province : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public int Code { get; private set; }
    
    private readonly List<District> _districts = new();
    public IReadOnlyCollection<District> Districts => _districts.AsReadOnly();

    private Province() { }

    public static Province Create(string name, int code)
    {
        return new Province
        {
            Name = name,
            Code = code
        };
    }
}
