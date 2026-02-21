using Microsoft.EntityFrameworkCore;
using HorkosAPI.Database.Models;
namespace HorkosAPI.Database.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DatabaseContext : DbContext
{
    private readonly string _connectionString;
    private ConfigurationService ConfigurationService { get; set; }

    public DatabaseContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, DbContextOptions<DatabaseContext> options) : base(options)
    {
        ConfigurationService = new ConfigurationService(configuration, httpContextAccessor);
        _connectionString = ConfigurationService.DatabaseConnexionString;
    }


    public virtual DbSet<Models.User> Users { get; set; }
    public virtual DbSet<Models.Role> Roles { get; set; }
    public virtual DbSet<Models.Entity> Entities { get; set; }
    public virtual DbSet<Models.FactCheck> FactChecks { get; set; }
    public virtual DbSet<Models.FactEntity> FactEntities { get; set; }
    public virtual DbSet<Models.FactSource> FactSources { get; set; }
    public virtual DbSet<Models.FactCheckSource> FactCheckSources { get; set; }
    public virtual DbSet<Models.Source> Sources { get; set; }
    public virtual DbSet<Models.UserContribution> UserContributions { get; set; }
    public virtual DbSet<Models.Comment> Comments { get; set; }
    public virtual DbSet<Models.Vote> Votes { get; set; }
    public virtual DbSet<Models.Report> Reports { get; set; }
    public virtual DbSet<Models.Fact> Facts { get; set; }
    public virtual DbSet<Models.GamificationAction> GamificationActions { get; set; }
    public virtual DbSet<Models.UserPoints> UserPoints { get; set; }
    public virtual DbSet<Models.UserActionHistory> UserActionHistories { get; set; }
    public virtual DbSet<Models.Level> Levels { get; set; }
    public virtual DbSet<Models.UserBadge> UserBadges { get; set; }
    public virtual DbSet<Models.Badge> Badges { get; set; }
    public virtual DbSet<Models.Group> Groups { get; set; }
    public virtual DbSet<Models.FeaturedItem> FeaturedItems { get; set; }
    public virtual DbSet<Models.GroupEntity> GroupEntities { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.CommandTimeout(60));
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.FactEntity>()
            .HasKey(fe => new { fe.FactId, fe.EntityId });

        modelBuilder.Entity<Models.FactEntity>()
            .HasOne(fe => fe.Fact)
            .WithMany(f => f.FactEntities)
            .HasForeignKey(fe => fe.FactId);

        modelBuilder.Entity<Models.FactEntity>()
            .HasOne(fe => fe.Entity)
            .WithMany(e => e.FactEntities)
            .HasForeignKey(fe => fe.EntityId);

        modelBuilder.Entity<Models.GroupEntity>()
            .HasKey(ge => new { ge.GroupId, ge.EntityId });

        modelBuilder.Entity<Models.GroupEntity>()
            .HasOne(ge => ge.Group)
            .WithMany(g => g.GroupEntities)
            .HasForeignKey(ge => ge.GroupId);

        modelBuilder.Entity<Models.GroupEntity>()
            .HasOne(ge => ge.Entity)
            .WithMany(e => e.GroupEntities)
            .HasForeignKey(ge => ge.EntityId);

        modelBuilder.Entity<Models.FactSource>()
            .HasKey(fs => new { fs.FactId, fs.SourceId });

        modelBuilder.Entity<Models.FactSource>()
            .HasOne(fs => fs.Fact)
            .WithMany(f => f.FactSources)
            .HasForeignKey(fs => fs.FactId);

        modelBuilder.Entity<Models.FactSource>()
            .HasOne(fs => fs.Source)
            .WithMany(s => s.FactSources)
            .HasForeignKey(fs => fs.SourceId);

        modelBuilder.Entity<Models.FactCheckSource>()
            .HasKey(fcs => new { fcs.FactCheckId, fcs.SourceId });

        modelBuilder.Entity<Models.FactCheckSource>()
            .HasOne(fcs => fcs.FactCheck)
            .WithMany(fc => fc.FactCheckSources)
            .HasForeignKey(fcs => fcs.FactCheckId);

        modelBuilder.Entity<Models.FactCheckSource>()
            .HasOne(fcs => fcs.Source)
            .WithMany(s => s.FactCheckSources)
            .HasForeignKey(fcs => fcs.SourceId);

        modelBuilder.Entity<Models.User>()
            .HasOne(u => u.Points)
            .WithOne(p => p.User)
            .HasForeignKey<UserPoints>(p => p.UserId);


        modelBuilder.Entity<Models.UserActionHistory>()
            .HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId);

        modelBuilder.Entity<Models.UserActionHistory>()
            .HasOne(ua => ua.Action)
            .WithMany()
            .HasForeignKey(ua => ua.ActionId);

        modelBuilder.Entity<Models.UserActionHistory>()
            .HasOne(ua => ua.Contribution)
            .WithMany()
            .HasForeignKey(ua => ua.ContributionId);

        modelBuilder.Entity<UserBadge>()
            .HasKey(ub => new { ub.UserId, ub.BadgeId });

        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.Badges)
            .HasForeignKey(ub => ub.UserId);

        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.Badge)
            .WithMany(b => b.Users)
            .HasForeignKey(ub => ub.BadgeId);

        modelBuilder.Entity<Models.User>();
        modelBuilder.Entity<Models.Role>();
        modelBuilder.Entity<Models.Entity>();
        modelBuilder.Entity<Models.FactCheck>();
        modelBuilder.Entity<Models.Source>();
        modelBuilder.Entity<Models.UserContribution>();
        modelBuilder.Entity<Models.Comment>();
        modelBuilder.Entity<Models.Vote>();
        modelBuilder.Entity<Models.Fact>();
        modelBuilder.Entity<Models.Report>();
        modelBuilder.Entity<Models.GamificationAction>();
        modelBuilder.Entity<Models.UserPoints>();
        modelBuilder.Entity<Models.UserActionHistory>();
        modelBuilder.Entity<Models.Level>();
        modelBuilder.Entity<Models.UserBadge>();
        modelBuilder.Entity<Models.Badge>();
        modelBuilder.Entity<Models.Group>();
        modelBuilder.Entity<Models.GroupEntity>();
        modelBuilder.Entity<Models.FeaturedItem>();

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => v
    );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
            v => v
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

}