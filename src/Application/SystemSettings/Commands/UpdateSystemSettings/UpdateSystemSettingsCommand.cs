using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemSettings.Commands.UpdateSystemSettings;

public class UpdateSystemSettingsCommand : ICommand<bool>
{
    public Dictionary<string, string> Settings { get; set; } = new();
}
