using Domain.Entities.Screening;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class MedicalDiagnosisTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateDiagnosis()
    {
        var diagnosis = new MedicalDiagnosis(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            diagnosisCode: "H10",
            clinicalFindings: "Findings",
            severityLevel: "Moderate",
            lifestyleAdvice: "Rest",
            isUrgent: true);

        diagnosis.DiagnosisCode.Should().Be("H10");
        diagnosis.ClinicalFindings.Should().Be("Findings");
        diagnosis.IsUrgent.Should().BeTrue();
        diagnosis.LifestyleAdvice.Should().Be("Rest");
    }

    [Fact]
    public void UpdateClinicalAssessment_ShouldUpdateFields()
    {
        var diagnosis = new MedicalDiagnosis(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        diagnosis.UpdateClinicalAssessment(
            "H11",
            "ICD10",
            "updated findings",
            "High",
            91m,
            "treatment",
            "recommendations",
            "lifestyle",
            true,
            "Draft",
            DateTime.UtcNow.AddDays(7),
            true);

        diagnosis.DiagnosisCode.Should().Be("H11");
        diagnosis.CodingSystem.Should().Be("ICD10");
        diagnosis.IsUrgent.Should().BeTrue();
        diagnosis.IsReferralNeeded.Should().BeTrue();
    }

    [Fact]
    public void SetFollowUp_PastDate_ShouldThrow()
    {
        var diagnosis = new MedicalDiagnosis(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = () => diagnosis.SetFollowUp(DateTime.UtcNow.AddMinutes(-1));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Follow-up date must be in the future*");
    }

    [Fact]
    public void Confirm_ShouldFinalizeDiagnosis()
    {
        var diagnosis = new MedicalDiagnosis(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        diagnosis.Confirm();

        diagnosis.Status.Should().Be("Finalized");
        diagnosis.FinalizedAt.Should().NotBeNull();
        diagnosis.ConfirmedAt.Should().NotBeNull();
    }
}