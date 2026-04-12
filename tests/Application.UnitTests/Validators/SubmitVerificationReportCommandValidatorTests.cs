using Application.ConsultationSessions.Commands.SubmitVerificationReport;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class SubmitVerificationReportCommandValidatorTests
{
    private readonly SubmitVerificationReportCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_WithNewFields_ShouldPass()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Mild retinopathy detected",
            CodingSystem = "ICD-10",
            SeverityLevel = "Moderate",
            ConfidenceLevel = 85m,
            TreatmentPlan = "Monitor every 6 months",
            Recommendations = "Reduce sugar intake",
            LifestyleAdvice = "Exercise regularly",
            Status = "Finalized"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidCommand_WithLegacyFields_ShouldPass()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosesCode = "H35.30",
            DiagnosesText = "Mild retinopathy detected"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptySessionId_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.Empty,
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SessionId)
            .WithErrorMessage("Session ID is required.");
    }

    [Fact]
    public void Validate_EmptyDoctorId_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.Empty,
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DoctorId)
            .WithErrorMessage("Doctor ID is required.");
    }

    [Fact]
    public void Validate_NoDiagnosisCodeAtAll_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = null,
            DiagnosesCode = null,
            ClinicalFindings = "Findings"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Diagnosis code is required.");
    }

    [Fact]
    public void Validate_NoClinicalFindingsAtAll_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = null,
            DiagnosesText = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Clinical findings are required.");
    }

    [Fact]
    public void Validate_DiagnosisCodeExceeds50Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = new string('A', 51),
            ClinicalFindings = "Findings"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiagnosisCode)
            .WithErrorMessage("Diagnosis code must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_DiagnosesCodeExceeds50Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosesCode = new string('A', 51),
            DiagnosesText = "Findings"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiagnosesCode)
            .WithErrorMessage("Diagnosis code must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_CodingSystemExceeds50Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            CodingSystem = new string('A', 51)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CodingSystem)
            .WithErrorMessage("Coding system must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_ClinicalFindingsExceeds2000Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ClinicalFindings)
            .WithErrorMessage("Clinical findings must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_DiagnosesTextExceeds2000Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosesCode = "H35.30",
            DiagnosesText = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiagnosesText)
            .WithErrorMessage("Clinical findings must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_SeverityLevelExceeds50Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            SeverityLevel = new string('A', 51)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SeverityLevel)
            .WithErrorMessage("Severity level must not exceed 50 characters.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_ConfidenceLevelOutOfRange_ShouldFail(decimal confidence)
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            ConfidenceLevel = confidence
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConfidenceLevel)
            .WithErrorMessage("Confidence level must be between 0 and 100.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_ConfidenceLevelBoundaryValues_ShouldPass(decimal confidence)
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            ConfidenceLevel = confidence
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ConfidenceLevel);
    }

    [Fact]
    public void Validate_NullConfidenceLevel_ShouldPass()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            ConfidenceLevel = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ConfidenceLevel);
    }

    [Fact]
    public void Validate_TreatmentPlanExceeds2000Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            TreatmentPlan = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TreatmentPlan)
            .WithErrorMessage("Treatment plan must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_RecommendationsExceeds2000Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            Recommendations = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Recommendations)
            .WithErrorMessage("Recommendations must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_LifestyleAdviceExceeds2000Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            LifestyleAdvice = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LifestyleAdvice)
            .WithErrorMessage("Lifestyle advice must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_StatusExceeds50Characters_ShouldFail()
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            DiagnosisCode = "H35.30",
            ClinicalFindings = "Findings",
            Status = new string('A', 51)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must not exceed 50 characters.");
    }
}
