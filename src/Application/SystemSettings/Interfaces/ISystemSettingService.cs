namespace Application.SystemSettings.Interfaces;

public interface ISystemSettingService
{
    Task<string?> GetSettingAsync(string key, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(Dictionary<string, string> settings, CancellationToken cancellationToken = default);
}
