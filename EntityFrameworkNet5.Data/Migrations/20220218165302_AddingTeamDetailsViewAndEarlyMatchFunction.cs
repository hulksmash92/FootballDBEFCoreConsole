using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkNet5.Data.Migrations
{
    public partial class AddingTeamDetailsViewAndEarlyMatchFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE FUNCTION [dbo].[GetEarliestMatch] (@teamId int)
                                    RETURNS datetime
                                    BEGIN
                                        DECLARE @result datetime
                                        SELECT TOP (1) @result = date 
                                        FROM [dbo].Matches 
                                        ORDER BY Date
                                        RETURN @result
                                    END
                                ");

            migrationBuilder.Sql(@"CREATE VIEW [dbo].[TeamsDetail]
                                   AS
                                   SELECT t.Name, c.Name AS CoachName, l.Name AS LeagueName
                                   FROM dbo.Teams AS t
                                   LEFT JOIN dbo.Coaches AS c ON c.TeamId = t.Id
                                   LEFT JOIN dbo.Leagues AS l ON l.Id = t.LeagueId
                                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION [dbo].[GetEarliestMatch]");
            migrationBuilder.Sql(@"DROP VIEW [dbo].[TeamsDetail]");
        }
    }
}
