using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Entities.Consultation;
using Domain.Entities.Contracts;
using Domain.Entities.Financial;
using Domain.Entities.Network;
using Domain.Entities.Platform;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

/// <summary>
/// Application DbContext with ASP.NET Core Identity integration
/// Inherits from IdentityDbContext for Identity support
/// </summary>
public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region DbSets - Domain Entities

    // Users
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<OrganisationOnboardingRequest> OrganisationOnboardingRequests => Set<OrganisationOnboardingRequest>();
    public DbSet<Ophthalmologist> Ophthalmologists => Set<Ophthalmologist>();
    public DbSet<OphthalmologistEmploymentTypeChangeRequest> OphthalmologistEmploymentTypeChangeRequests => Set<OphthalmologistEmploymentTypeChangeRequest>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<ClinicStaff> ClinicStaffs => Set<ClinicStaff>();
    public DbSet<OrganisationPatientLink> OrganisationPatientLinks => Set<OrganisationPatientLink>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Consent> Consents => Set<Consent>();

    // Screening
    public DbSet<AiScreening> AiScreenings => Set<AiScreening>();
    public DbSet<RetinalImage> RetinalImages => Set<RetinalImage>();
    public DbSet<ScreeningResult> ScreeningResults => Set<ScreeningResult>();
    public DbSet<MedicalDiagnosis> MedicalDiagnoses => Set<MedicalDiagnosis>();
    public DbSet<PatientRoadmap> PatientRoadmaps => Set<PatientRoadmap>();

    // Consultation
    public DbSet<ConsultationSession> ConsultationSessions => Set<ConsultationSession>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<WebsiteFeedback> WebsiteFeedbacks => Set<WebsiteFeedback>();
    public DbSet<OrganisationFeedback> OrganisationFeedbacks => Set<OrganisationFeedback>();
    public DbSet<OphthalmologistFeedback> OphthalmologistFeedbacks => Set<OphthalmologistFeedback>();

    // Scheduling
    public DbSet<ScheduleTemplate> ScheduleTemplates => Set<ScheduleTemplate>();
    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<OphthalmologistLeaveRequest> OphthalmologistLeaveRequests => Set<OphthalmologistLeaveRequest>();
    public DbSet<ExperiencePricingRule> ExperiencePricingRules => Set<ExperiencePricingRule>();

    // Financial
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<DepositRequest> DepositRequests => Set<DepositRequest>();
    public DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Contracts
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();

    // Authorization
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Platform
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<WorkloadRequirement> WorkloadRequirements => Set<WorkloadRequirement>();

    // Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Network
    public DbSet<ProfessionalPost> ProfessionalPosts => Set<ProfessionalPost>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filter for soft delete
        ApplySoftDeleteFilter(modelBuilder);
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        // Apply global query filter for soft delete to all entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                var comparison = System.Linq.Expressions.Expression.Equal(property, falseConstant);
                var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        await DispatchDomainEventsAsync(cancellationToken);
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            throw new Domain.Common.ConcurrencyException(
                "A concurrent write conflict occurred. Another request may have modified the same data.", ex);
        }
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added
                     || e.State == EntityState.Modified
                     || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                // For all other BaseEntity types: convert hard delete → soft delete.
                entry.State = EntityState.Modified;
                entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                // When a new entity with a pre-set GUID key is added to a tracked
                // navigation collection, EF Core may track it as Modified instead of
                // Added.  Detect this by checking whether immutable columns (CreatedAt)
                // are flagged as modified — that never happens for genuine updates.
                if (entry.Property(nameof(BaseEntity.CreatedAt)).IsModified)
                {
                    entry.State = EntityState.Added;
                    continue;
                }

                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // Here you would typically publish domain events using MediatR or message bus
        await Task.CompletedTask;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }
}
