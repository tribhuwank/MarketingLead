using Microsoft.EntityFrameworkCore;
using MarketingLead.API.Entities;
using System.Data;
using MarketingLead.API.Pipes;

namespace MarketingLead.API.Data;

public class MarketingLeadDb:DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MarketingLeadDb(DbContextOptions<MarketingLeadDb> options, IHttpContextAccessor httpContextAccessor) : base(options) { _httpContextAccessor = httpContextAccessor; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => new { u.ContactInfo,u.Role }).IsUnique();
        modelBuilder.Entity<Team>().HasIndex(u => new { u.Name ,u.TeamLeadId}).IsUnique();
        modelBuilder.Entity<Lead>().HasIndex(u => new { u.Name, u.AccountId }).IsUnique();
        modelBuilder.Entity<Lead>().HasIndex(u => new { u.Name, u.ClientManagerId }).IsUnique();
        modelBuilder.Entity<PaymentCategory>().HasIndex(u => u.Name).IsUnique();

    }
    public DbSet<Account> UserRoles => Set<Account>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<PaymentCategory> PaymentCategorys => Set<PaymentCategory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
        .HaveConversion<DateOnlyConverter>()
        .HaveColumnType("date");
        base.ConfigureConventions(builder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
           .Where(e => e.Entity is BaseEntity && (
           e.State == EntityState.Added
           || e.State == EntityState.Modified));
        var _currentUser = _httpContextAccessor.HttpContext?.Items["CurrentUser"];

        foreach (var entityEntry in entries)
        {
            if (_currentUser != null)
                ((BaseEntity)entityEntry.Entity).CreatedBy = _currentUser.ToString();
            else ((BaseEntity)entityEntry.Entity).LastUpdatedBy = "Admin";
            ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                if (_currentUser != null)
                    ((BaseEntity)entityEntry.Entity).CreatedBy = _currentUser.ToString();
                else ((BaseEntity)entityEntry.Entity).CreatedBy = "Admin";
                ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (
            e.State == EntityState.Added
            || e.State == EntityState.Modified));
        var _currentUser = _httpContextAccessor.HttpContext?.Items["CurrentUser"];

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).LastUpdatedBy = _currentUser.ToString();
            ((BaseEntity)entityEntry.Entity).LastUpdatedOn = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedBy = _currentUser.ToString();
                ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
            }
        }
        return base.SaveChanges();
    }
}
