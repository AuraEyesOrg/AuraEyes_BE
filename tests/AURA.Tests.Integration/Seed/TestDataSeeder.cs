using AURA.Tests.Integration.Builders;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Seed;

public static class TestDataSeeder
{
    public static async Task<Patient> GetSeededPatientAsync(ApplicationDbContext dbContext)
    {
        var patient = await dbContext.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (patient is null)
        {
            throw new InvalidOperationException("Seeded patient profile not found.");
        }

        return patient;
    }

    public static async Task<Ophthalmologist> GetSeededOphthalmologistAsync(ApplicationDbContext dbContext)
    {
        var ophthalmologist = await dbContext.Ophthalmologists
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (ophthalmologist is null)
        {
            throw new InvalidOperationException("Seeded ophthalmologist profile not found.");
        }

        return ophthalmologist;
    }

    public static async Task<ApplicationUser> GetSeededPatientUserAsync(ApplicationDbContext dbContext)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == "patient@gmail.com");

        if (user is null)
        {
            throw new InvalidOperationException("Seeded patient user not found.");
        }

        return user;
    }

    public static async Task<(AiScreening Screening, RetinalImage Image, ScreeningResult Result)> CreateAiScreeningWithResultAsync(
        ApplicationDbContext dbContext,
        Guid patientId,
        RiskLevel riskLevel = RiskLevel.Moderate)
    {
        var builder = new ScreeningSessionBuilder()
            .WithPatientId(patientId)
            .WithRisk(riskLevel, 88m);

        var (screening, image, result) = builder.Build();

        await dbContext.AiScreenings.AddAsync(screening);
        await dbContext.RetinalImages.AddAsync(image);
        await dbContext.ScreeningResults.AddAsync(result);
        await dbContext.SaveChangesAsync();

        return (screening, image, result);
    }

    public static async Task<ConsultationSession> CreateCompletedConsultationAsync(
        ApplicationDbContext dbContext,
        Guid patientId,
        Guid ophthalmologistId,
        Guid aiScreeningId)
    {
        var session = new ConsultationBuilder()
            .WithPatientId(patientId)
            .WithOphthalmologistId(ophthalmologistId)
            .WithAiScreeningId(aiScreeningId)
            .BuildVerification();

        session.Confirm();
        session.EndSession(ophthalmologistId, "CompletedForIntegrationTest");

        await dbContext.ConsultationSessions.AddAsync(session);
        await dbContext.SaveChangesAsync();

        return session;
    }
}
