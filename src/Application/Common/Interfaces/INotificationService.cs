namespace Application.Common.Interfaces;

public interface INotificationService
{
    Task SendAsync(Guid userId, string message, CancellationToken cancellationToken = default);
}
