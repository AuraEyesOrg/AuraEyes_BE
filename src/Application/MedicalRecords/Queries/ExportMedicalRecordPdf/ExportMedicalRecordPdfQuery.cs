using Application.Common.Interfaces;

namespace Application.MedicalRecords.Queries.ExportMedicalRecordPdf;

public sealed record ExportMedicalRecordPdfQuery(Guid MedicalRecordId)
    : IQuery<MedicalRecordPdfFileDto>;

public sealed record MedicalRecordPdfFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "application/pdf";
    public string FileName { get; init; } = "emr.pdf";
}
