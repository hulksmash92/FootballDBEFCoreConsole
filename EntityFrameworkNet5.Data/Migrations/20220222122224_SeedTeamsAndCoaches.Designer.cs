﻿// <auto-generated />
using System;
using EntityFrameworkNet5.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkNet5.Data.Migrations
{
    [DbContext(typeof(FootballLeagueDbContext))]
    [Migration("20220222122224_SeedTeamsAndCoaches")]
    partial class SeedTeamsAndCoaches
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Coach", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TeamId")
                        .IsUnique()
                        .HasFilter("[TeamId] IS NOT NULL");

                    b.ToTable("Coaches");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            Name = "Antonio Conte",
                            TeamId = 26
                        },
                        new
                        {
                            Id = 3,
                            Name = "Mikel Arteta",
                            TeamId = 28
                        },
                        new
                        {
                            Id = 4,
                            Name = "Steven Gerrard",
                            TeamId = 25
                        },
                        new
                        {
                            Id = 5,
                            Name = "Thomas Frank",
                            TeamId = 27
                        });
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.League", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AwayTeamId");

                    b.HasIndex("HomeTeamId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LeagueId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LeagueId");

                    b.ToTable("Teams");

                    b.HasData(
                        new
                        {
                            Id = 25,
                            LeagueId = 1,
                            Name = "Aston Villa"
                        },
                        new
                        {
                            Id = 26,
                            LeagueId = 1,
                            Name = "Tottenham Hot Spurs"
                        },
                        new
                        {
                            Id = 27,
                            LeagueId = 1,
                            Name = "Brentford"
                        },
                        new
                        {
                            Id = 28,
                            LeagueId = 1,
                            Name = "Arsenal"
                        });
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.TeamsDetail", b =>
                {
                    b.Property<string>("CoachName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LeagueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.ToView("TeamsDetail");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Coach", b =>
                {
                    b.HasOne("EntityFrameworkNet5.Domain.Team", "Team")
                        .WithOne("Coach")
                        .HasForeignKey("EntityFrameworkNet5.Domain.Coach", "TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Match", b =>
                {
                    b.HasOne("EntityFrameworkNet5.Domain.Team", "AwayTeam")
                        .WithMany("AwayMatches")
                        .HasForeignKey("AwayTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EntityFrameworkNet5.Domain.Team", "HomeTeam")
                        .WithMany("HomeMatches")
                        .HasForeignKey("HomeTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AwayTeam");

                    b.Navigation("HomeTeam");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Team", b =>
                {
                    b.HasOne("EntityFrameworkNet5.Domain.League", "League")
                        .WithMany("Teams")
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.League", b =>
                {
                    b.Navigation("Teams");
                });

            modelBuilder.Entity("EntityFrameworkNet5.Domain.Team", b =>
                {
                    b.Navigation("AwayMatches");

                    b.Navigation("Coach");

                    b.Navigation("HomeMatches");
                });
#pragma warning restore 612, 618
        }
    }
}
