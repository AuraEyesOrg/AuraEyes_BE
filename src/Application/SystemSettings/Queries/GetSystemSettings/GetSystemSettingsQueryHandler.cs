using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;

namespace Application.SystemSettings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler : IQueryHandler<GetSystemSettingsQuery, Dictionary<string, string>>
{
    private readonly ISystemSettingService _settingService;

    public GetSystemSettingsQueryHandler(ISystemSettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task<Result<Dictionary<string, string>>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settingsDict = await _settingService.GetAllSettingsAsync(cancellationToken);
        return Result<Dictionary<string, string>>.Success(settingsDict);
    }
}
