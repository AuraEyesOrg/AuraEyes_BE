using Domain.Entities.Users;

namespace AURA.Tests.Integration.Builders;

public sealed class PatientBuilder
{
    private Guid _userId = Guid.NewGuid();
    private decimal? _bmi;
    private string? _diseaseHistory;

    public PatientBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public PatientBuilder WithBmi(decimal bmi)
    {
        _bmi = bmi;
        return this;
    }

    public PatientBuilder WithDiseaseHistory(string diseaseHistory)
    {
        _diseaseHistory = diseaseHistory;
        return this;
    }

    public Patient Build()
    {
        return new Patient(_userId, _bmi, _diseaseHistory);
    }
}
