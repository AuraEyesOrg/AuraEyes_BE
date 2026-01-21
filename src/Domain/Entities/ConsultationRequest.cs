using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Consultation Request entity - patient's request for consultation
/// </summary>
public class ConsultationRequest : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public ConsultationStatus Status { get; private set; }
    public DateTime RequestAt { get; private set; }
    public string? RequestMessage { get; private set; }
    public string? MeetingLink { get; private set; }
    public string? DiagnosisNote { get; private set; }
    public bool IsFeedbackRequested { get; private set; }

    // Navigation properties
    private readonly List<Conversation> _conversations = new();
    public IReadOnlyCollection<Conversation> Conversations => _conversations.AsReadOnly();

    private ConsultationRequest() { } // EF Core

    public ConsultationRequest(Guid patientId, string? requestMessage = null)
    {
        PatientId = patientId;
        Status = ConsultationStatus.Pending;
        RequestAt = DateTime.UtcNow;
        RequestMessage = requestMessage;
        IsFeedbackRequested = false;
    }

    public void Approve(string? meetingLink = null)
    {
        if (Status != ConsultationStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved");

        Status = ConsultationStatus.Approved;
        MeetingLink = meetingLink;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != ConsultationStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected");

        Status = ConsultationStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartConsultation()
    {
        if (Status != ConsultationStatus.Approved)
            throw new InvalidOperationException("Only approved requests can start consultation");

        Status = ConsultationStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? diagnosisNote = null)
    {
        if (Status != ConsultationStatus.InProgress)
            throw new InvalidOperationException("Only in-progress consultations can be completed");

        Status = ConsultationStatus.Completed;
        DiagnosisNote = diagnosisNote;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ConsultationStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequestFeedback()
    {
        IsFeedbackRequested = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddConversation(Conversation conversation)
    {
        _conversations.Add(conversation);
        UpdatedAt = DateTime.UtcNow;
    }
}
