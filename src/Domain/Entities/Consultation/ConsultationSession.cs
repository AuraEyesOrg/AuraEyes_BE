using Domain.Common;
using Domain.Entities.Screening;
using Domain.Enums;

namespace Domain.Entities.Consultation;

/// <summary>
/// Central aggregate for all medical interactions (Verification, VideoCall, ClinicBooking).
/// </summary>
public class ConsultationSession : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid? OphthalmologistId { get; private set; }
    public Guid? OrganisationId { get; private set; }
    public Guid? AiScreeningId { get; private set; }

    public ConsultationSessionType Type { get; private set; }
    public SessionStatus Status { get; private set; }
    public ChatStatus ChatStatus { get; private set; }

    /// <summary>Platform fee / deposit charged online (phí nền tảng / tiền cọc).</summary>
    public decimal PlatformFee { get; private set; }

    /// <summary>Fee paid in cash at the clinic counter (optional for O2O model).</summary>
    public decimal? OfflineClinicFee { get; private set; }

    /// <summary>Patient consent flag: share retinal images with the assigned doctor.</summary>
    public bool IsRetinalImagesShared { get; private set; }

    /// <summary>Patient consent flag: share AI screening result with the assigned doctor.</summary>
    public bool IsAIResultShared { get; private set; }

    public DateTime? AppointmentTime { get; private set; }
    public string? MeetingLink { get; private set; }
    public string? CalendarEventId { get; private set; }

    public DateTime LastActivityAt { get; private set; }
    public DateTime? LastReminderSentAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Guid? ClosedBy { get; private set; }
    public string? ClosingReason { get; private set; }

    // Navigation properties
    private readonly List<Conversation> _conversations = new();
    public IReadOnlyCollection<Conversation> Conversations => _conversations.AsReadOnly();

    private readonly List<MedicalDiagnosis> _medicalDiagnoses = new();
    public IReadOnlyCollection<MedicalDiagnosis> MedicalDiagnoses => _medicalDiagnoses.AsReadOnly();

    private ConsultationSession() { } // EF Core

    /// <summary>
    /// Factory: create a Verification session (chat starts Locked).
    /// </summary>
    public static ConsultationSession CreateVerification(
        Guid patientId,
        Guid aiScreeningId,
        decimal platformFee,
        Guid? ophthalmologistId = null)
    {
        return new ConsultationSession
        {
            PatientId = patientId,
            AiScreeningId = aiScreeningId,
            OphthalmologistId = ophthalmologistId,
            Type = ConsultationSessionType.Verification,
            Status = SessionStatus.Pending,
            ChatStatus = ChatStatus.Locked,
            PlatformFee = platformFee,
            LastActivityAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory: create a VideoCall session (chat starts as MemoOnly so patients can leave notes).
    /// </summary>
    public static ConsultationSession CreateVideoCall(
        Guid patientId,
        decimal platformFee,
        DateTime appointmentTime,
        Guid? ophthalmologistId = null,
        string? meetingLink = null,
        string? calendarEventId = null)
    {
        if (appointmentTime <= DateTime.UtcNow)
            throw new ArgumentException("Appointment time must be in the future", nameof(appointmentTime));

        return new ConsultationSession
        {
            PatientId = patientId,
            OphthalmologistId = ophthalmologistId,
            Type = ConsultationSessionType.VideoCall,
            Status = SessionStatus.Pending,
            ChatStatus = ChatStatus.MemoOnly,
            PlatformFee = platformFee,
            AppointmentTime = appointmentTime,
            MeetingLink = meetingLink,
            CalendarEventId = calendarEventId,
            LastActivityAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory: create a ClinicBooking session (O2O model: deposit online, pay remainder at clinic).
    /// </summary>
    public static ConsultationSession CreateClinicBooking(
        Guid patientId,
        Guid organisationId,
        decimal platformFee,
        DateTime appointmentTime,
        decimal? offlineClinicFee = null,
        Guid? ophthalmologistId = null)
    {
        if (appointmentTime <= DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("Appointment time must be in the future", nameof(appointmentTime));

        return new ConsultationSession
        {
            PatientId = patientId,
            OrganisationId = organisationId,
            OphthalmologistId = ophthalmologistId,
            Type = ConsultationSessionType.ClinicBooking,
            Status = SessionStatus.Pending,
            ChatStatus = ChatStatus.Locked,
            PlatformFee = platformFee,
            OfflineClinicFee = offlineClinicFee,
            AppointmentTime = appointmentTime,
            LastActivityAt = DateTime.UtcNow
        };
    }

    public void Confirm()
    {
        if (Status != SessionStatus.Pending)
            throw new InvalidOperationException("Only pending sessions can be confirmed");

        Status = SessionStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDoctor(Guid ophthalmologistId)
    {
        OphthalmologistId = ophthalmologistId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMeetingInfo(string meetingLink, string? calendarEventId = null)
    {
        if (string.IsNullOrWhiteSpace(meetingLink))
            throw new ArgumentException("Meeting link cannot be empty", nameof(meetingLink));

        MeetingLink = meetingLink;
        CalendarEventId = calendarEventId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearMeetingInfo()
    {
        MeetingLink = null;
        CalendarEventId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Opens 2-way chat (e.g. after doctor submits verification report).
    /// </summary>
    public void OpenChat()
    {
        if (ChatStatus == ChatStatus.Archived)
            throw new InvalidOperationException("Cannot reopen an archived session");

        ChatStatus = ChatStatus.Open;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Records activity on the session (must be called on every new message).
    /// </summary>
    public void RecordActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks that a stale-session reminder was sent, preventing duplicate notifications.
    /// </summary>
    public void RecordReminderSent()
    {
        LastReminderSentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ends the session. Only the assigned doctor can call this.
    /// </summary>
    public void EndSession(Guid doctorId, string reason = "DoctorFinished")
    {
        if (OphthalmologistId.HasValue && OphthalmologistId.Value != doctorId)
            throw new InvalidOperationException("Only the assigned ophthalmologist can end this session");

        Status = SessionStatus.Completed;
        ChatStatus = ChatStatus.Archived;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = doctorId;
        ClosingReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(Guid cancelledBy, string reason = "UserCancelled")
    {
        if (Status == SessionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed session");

        Status = SessionStatus.Cancelled;
        ChatStatus = ChatStatus.Archived;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = cancelledBy;
        ClosingReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddConversation(Conversation conversation)
    {
        _conversations.Add(conversation);
        UpdatedAt = DateTime.UtcNow;
    }
}
