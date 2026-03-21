using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.SystemSettings.Commands.UpdateSystemSettings;

public class UpdateSystemSettingsCommandHandler : ICommandHandler<UpdateSystemSettingsCommand, bool>
{
    private readonly ISystemSettingService _settingService;
    private readonly ILogger<UpdateSystemSettingsCommandHandler> _logger;

    public UpdateSystemSettingsCommandHandler(ISystemSettingService settingService, ILogger<UpdateSystemSettingsCommandHandler> logger)
    {
        _settingService = settingService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateSystemSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.Settings == null || !request.Settings.Any())
        {
            return Result<bool>.Success(true);
        }

        try
        {
            await _settingService.UpdateSettingsAsync(request.Settings, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update system settings.");
            return Result<bool>.Failure("Failed to update system settings.");
        }
    }
}
