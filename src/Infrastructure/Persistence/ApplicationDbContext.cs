using Domain.Common;
using Domain.Entities;
using Domain.Entities.Network;
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

    // Core Medical
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<Ophthalmologist> Ophthalmologists => Set<Ophthalmologist>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Consent> Consents => Set<Consent>();

    // AI & Diagnosis
    public DbSet<AiScreening> AiScreenings => Set<AiScreening>();
    public DbSet<RetinalImage> RetinalImages => Set<RetinalImage>();
    public DbSet<ScreeningResult> ScreeningResults => Set<ScreeningResult>();
    public DbSet<MedicalDiagnosis> MedicalDiagnoses => Set<MedicalDiagnosis>();

    // Schedule & Communication
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ConsultationSession> ConsultationSessions => Set<ConsultationSession>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    // Financial
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();

    // Permission & Audit
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    // Identity & Auth
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Professional Network
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
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.GetType().GetProperty("UpdatedAt")?.SetValue(entry.Entity, DateTime.UtcNow);
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
