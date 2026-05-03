using Domain.Entities.Consultation;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Screening;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Application.ClinicQueue.Common;

/// <summary>
/// Single source for clinic pipeline state labels shared by queue and appointments list.
/// </summary>
public static class ClinicFlowStateResolver
{
    public static string Resolve(
        PatientVisit visit,
        AiScreening? screening,
        ConsultationSession? consultation,
        MedicalRecord? medicalRecord = null)
    {
        if (visit.Status == PatientVisitStatus.WaitingForPayment)
            return "Finalized";

        if (visit.Status == PatientVisitStatus.Completed)
            return "Finalized";

        if (consultation != null)
        {
            if (consultation.Status == SessionStatus.Confirmed)
                return "ConsultationInProgress";

            if (consultation.Status == SessionStatus.Completed)
                return "Finalized";

            return "SentToDoctor";
        }

        if (screening != null)
        {
            if (screening.ScreeningResults.Count > 0)
                return "AICompleted";

            return "ScreeningPending";
        }

        bool isErmReady = medicalRecord != null
            && medicalRecord.Status != MedicalRecordStatus.DraftAdmin;

        if (!isErmReady)
            return "ErmPending";

        return "CheckedIn";
    }
}
