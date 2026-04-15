using Application.Common.Interfaces;

namespace Application.Screenings.Queries.ExportPatientScreeningReportPdf;

public sealed record ExportPatientScreeningReportPdfQuery(Guid PatientUserId, Guid ScreeningId)
    : IQuery<PatientScreeningReportPdfFileDto>;

public sealed record PatientScreeningReportPdfFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "application/pdf";
    public string FileName { get; init; } = "patient-screening-report.pdf";
}
