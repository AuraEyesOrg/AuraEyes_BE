using Domain.Entities.Users;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PatientTests
{
    [Fact]
    public void Constructor_ShouldCreatePatient()
    {
        var userId = Guid.NewGuid();

        var patient = new Patient(userId, 24.5m, "Type-2 Diabetes");

        patient.UserId.Should().Be(userId);
        patient.BMI.Should().Be(24.5m);
        patient.DiseaseHistory.Should().Be("Type-2 Diabetes");
        patient.RetinalImages.Should().BeEmpty();
        patient.AiScreenings.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_DefaultValues_ShouldBeNull()
    {
        var patient = new Patient(Guid.NewGuid());

        patient.BMI.Should().BeNull();
        patient.DiseaseHistory.Should().BeNull();
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateFields()
    {
        var patient = new Patient(Guid.NewGuid());

        patient.UpdateProfile(28.0m, "Hypertension, Diabetes");

        patient.BMI.Should().Be(28.0m);
        patient.DiseaseHistory.Should().Be("Hypertension, Diabetes");
    }

    [Fact]
    public void UpdateProfile_NullValues_ShouldClearFields()
    {
        var patient = new Patient(Guid.NewGuid(), 24.5m, "Diabetes");

        patient.UpdateProfile(null, null);

        patient.BMI.Should().BeNull();
        patient.DiseaseHistory.Should().BeNull();
    }
}
