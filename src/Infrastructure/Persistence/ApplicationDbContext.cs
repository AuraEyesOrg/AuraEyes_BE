using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Entities.CarePlan;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
using Domain.Entities.Network;
using Domain.Entities.Network.InternalChat;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Platform;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Entities.MasterData;
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
    public DbSet<Ophthalmologist> Ophthalmologists => Set<Ophthalmologist>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<ClinicStaff> ClinicStaffs => Set<ClinicStaff>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    // Screening
    public DbSet<AiScreening> AiScreenings => Set<AiScreening>();
    public DbSet<RetinalImage> RetinalImages => Set<RetinalImage>();
    public DbSet<ScreeningResult> ScreeningResults => Set<ScreeningResult>();
    public DbSet<MedicalDiagnosis> MedicalDiagnoses => Set<MedicalDiagnosis>();

    // Consultation
    public DbSet<ConsultationSession> ConsultationSessions => Set<ConsultationSession>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<ClinicFeedback> ClinicFeedbacks => Set<ClinicFeedback>();

    // Scheduling
    public DbSet<ScheduleTemplate> ScheduleTemplates => Set<ScheduleTemplate>();
    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<SlotAssignment> SlotAssignments => Set<SlotAssignment>();
    public DbSet<PatientVisit> PatientVisits => Set<PatientVisit>();
    public DbSet<OphthalmologistLeaveRequest> OphthalmologistLeaveRequests => Set<OphthalmologistLeaveRequest>();

    // Financial
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Authorization
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Platform
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();

    // Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Network
    public DbSet<ProfessionalPost> ProfessionalPosts => Set<ProfessionalPost>();
    public DbSet<PostReaction> PostReactions => Set<PostReaction>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();
    
    // Internal Chat
    public DbSet<InternalGroupChat> InternalGroupChats => Set<InternalGroupChat>();
    public DbSet<InternalGroupMember> InternalGroupMembers => Set<InternalGroupMember>();
    public DbSet<InternalGroupMessage> InternalGroupMessages => Set<InternalGroupMessage>();

    // Medical Records
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();

    // Care Plan (Healthcare Roadmap)
    public DbSet<HealthRoadmap> HealthRoadmaps => Set<HealthRoadmap>();
    public DbSet<HealthRoadmapStep> HealthRoadmapSteps => Set<HealthRoadmapStep>();
    
    // Master Data
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Ward> Wards => Set<Ward>();


    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filter for soft delete
        ApplySoftDeleteFilter(builder);
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
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = false;
                entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Deleted)
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
                    // Re-applying IsDeleted = false after the state transition is critical.
                    // Changing entry.State to Added can cause EF Core to lose current
                    // property values (resetting them to CLR defaults, i.e. null for
                    // nullable-annotated booleans in shadow state), which would violate
                    // the NOT NULL constraint on the "IsDeleted" column.
                    entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = false;
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
