using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkNet5.Data.Configurations.Entities;
public class CoachConfiguration : IEntityTypeConfiguration<Coach>
{
    public void Configure(EntityTypeBuilder<Coach> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.Name);

        // Add Seed Data
        builder.HasData(
            new Coach 
            { 
                Id = 2, 
                Name = "Antonio Conte", 
                TeamId = 26 
            },
            new Coach 
            { 
                Id = 3, 
                Name = "Mikel Arteta",
                TeamId = 28
            },
            new Coach 
            { 
                Id = 4, 
                Name = "Steven Gerrard",
                TeamId = 25
            },
            new Coach 
            { 
                Id = 5, 
                Name = "Thomas Frank",
                TeamId = 27
            }
        );
    }
}
