using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkNet5.Data.Migrations
{
    public partial class AddedGetTeamCoachSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE dbo.sp_GetTeamCoach
                    @teamId INT
                AS
                BEGIN
                    SELECT * FROM dbo.Coaches WHERE TeamId = @teamId
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.sp_GetTeamCoach");
        }
    }
}
