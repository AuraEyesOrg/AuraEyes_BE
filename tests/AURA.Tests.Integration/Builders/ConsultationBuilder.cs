using Domain.Entities.Consultation;

namespace AURA.Tests.Integration.Builders;

public sealed class ConsultationBuilder
{
    private Guid _patientId = Guid.NewGuid();
    private Guid _aiScreeningId = Guid.NewGuid();
    private decimal _price = 150000m;
    private Guid? _ophthalmologistId;

    public ConsultationBuilder WithPatientId(Guid patientId)
    {
        _patientId = patientId;
        return this;
    }

    public ConsultationBuilder WithAiScreeningId(Guid aiScreeningId)
    {
        _aiScreeningId = aiScreeningId;
        return this;
    }

    public ConsultationBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ConsultationBuilder WithOphthalmologistId(Guid ophthalmologistId)
    {
        _ophthalmologistId = ophthalmologistId;
        return this;
    }

    public ConsultationSession BuildVerification()
    {
        return ConsultationSession.CreateVerification(
            _patientId,
            _aiScreeningId,
            _price,
            _ophthalmologistId);
    }
}
