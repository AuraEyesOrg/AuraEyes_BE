using Application.Common.Interfaces;

namespace Application.Network.Posts.Commands.ShareConsultationCase;

/// <summary>
/// Command to create a network post from an internal consultation case.
/// </summary>
public class ShareConsultationToNetworkCommand : ICommand<Guid>
{
    public Guid ConsultationSessionId { get; set; }
    public Guid CurrentUserId { get; set; }
    public Guid CurrentProfileId { get; set; }
    public string? DoctorNote { get; set; }
    public string? AiSummary { get; set; }
    public string? FinalDiagnosis { get; set; }
}
