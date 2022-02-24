using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkNet5.Data.Configurations.Entities;
public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasMany(m => m.HomeMatches)
            .WithOne(m => m.HomeTeam)
            .HasForeignKey(m => m.HomeTeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.AwayMatches)
            .WithOne(m => m.AwayTeam)
            .HasForeignKey(m => m.AwayTeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();

        // Add Seed Data
        builder.HasData(
            new Team
            {
                Id = 25,
                Name = "Aston Villa",
                LeagueId = 1
            },
            new Team
            {
                Id = 26,
                Name = "Tottenham Hot Spurs",
                LeagueId = 1
            },
            new Team
            {
                Id = 27,
                Name = "Brentford",
                LeagueId = 1
            },
            new Team
            {
                Id = 28,
                Name = "Arsenal",
                LeagueId = 1
            }
        );
    }
}
