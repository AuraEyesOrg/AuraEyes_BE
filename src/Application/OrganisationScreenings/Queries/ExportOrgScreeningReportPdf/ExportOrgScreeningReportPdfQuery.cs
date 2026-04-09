using Application.Common.Interfaces;

namespace Application.OrganisationScreenings.Queries.ExportOrgScreeningReportPdf;

public sealed record ExportOrgScreeningReportPdfQuery(Guid OrgAdminUserId, Guid ScreeningId)
    : IQuery<OrgScreeningReportPdfFileDto>;

public sealed record OrgScreeningReportPdfFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "application/pdf";
    public string FileName { get; init; } = "screening-report.pdf";
}
