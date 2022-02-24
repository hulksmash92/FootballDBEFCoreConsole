using EntityFrameworkNet5.Data.Configurations.Entities;
using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkNet5.Data;
public class FootballLeagueDbContext : AuditableFootballLeagueDbContext
{
    public DbSet<Team> Teams { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<TeamsDetail> TeamsDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FootbalLeague_EfCore;")
            .LogTo(
                Console.WriteLine, 
                new[] { DbLoggerCategory.Database.Command.Name }, 
                LogLevel.Information
            )
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamsDetail>()
            .HasNoKey()
            .ToView("TeamsDetail");

        // Configure entities using config files
        modelBuilder.ApplyConfiguration(new LeagueConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new CoachConfiguration());
        modelBuilder.ApplyConfiguration(new MatchConfiguration());

        // Get any foreign keys with cascading set
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(q => q.GetForeignKeys())
            .Where(q => !q.IsOwnership && q.DeleteBehavior == DeleteBehavior.Cascade);

        // Turn off cascading on delete of a related row
        foreach (var foreignKey in foreignKeys)
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

        // Indicate tables with a History table
        modelBuilder
            .Entity<Team>()
            .ToTable("Teams", b => b.IsTemporal());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.Properties<string>().HaveMaxLength(50);
    }

}
