using EntityFrameworkNet5.Data;
using EntityFrameworkNet5.Domain;
using EntityFrameworkNet5.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkNet5.ConsoleApp;
public class Program
{
    private static readonly FootballLeagueDbContext context = new FootballLeagueDbContext();

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        /* Simple INSERT ops */
        //await AddNewLeagueSimple();

        /* Simple SELECT */
        //await SimpleSelectQuery();

        /* Record filtering */
        // await QueryFilters();

        /* Additional Execution Methods */
        //await AdditionalExecutionMethods();

        /* Alternative Linq Syntax */
        //await AlternativeLinqSyntax();

        /* Simple Update Query */
        //await SimpleUpdateLeague();
        //await SimpleUpdateTeam();

        /* Simple Delete Query */
        //await SimpleDelete();
        //await SimpleDeleteWithRelationship();

        /* Tracking vs No Tracking */
        //await TrackingVsNoTracking();

        /* ----- Adding records with relationships -----  */
        /* OneToMany */
        //await AddScottishPremiership();
        //await AddMufcToPremierLeague();

        /* ManyToMany */
        //await AddMatchesNationalLeague();

        /* OneToOne */
        //await AddTuchel();

        /* Include related data - Eager Loading */
        //await QueryRelatedData();

        /* Projections to Other Data Types or Anonymous Data Types */
        //await SelectOneProperty();
        //await AnonymousProjection();
        //await StronglyTypedProjection();

        /* Filtering With Related Data */
        //await FilteringWithRelatedData();

        /* Interacting with a view */
        //await QueryView();

        /* Query with Raw SQL */
        //await RawSQLQuery();

        /* Query Stored Procedure */
        //await ExecStoredProcedure();

        /* Execute Non-Query command */
        //await ExecuteNonQuery();

        /* Get Temporal Data */
        await TeamsTemporalQuery();

        Console.ForegroundColor = ConsoleColor.Gray;
    }

    static async Task TeamsTemporalQuery()
    {
        var teamsHistory = await context.Teams.TemporalAll().ToListAsync();
    }

    static async Task ExecuteNonQuery()
    {
        var teamId = 11;
        var affectedRows = await context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteTeamById {0}", teamId);

        var teamId2 = 24;
        var affectedRows2 = await context.Database.ExecuteSqlInterpolatedAsync($"EXEC sp_DeleteTeamById {teamId2}");
    }

    static async Task ExecStoredProcedure()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n------ EF Query SP -----\n");

        var teamId = 7;
        var result = await context.Coaches
            .FromSqlRaw("EXEC dbo.sp_GetTeamCoach {0}", teamId)
            .ToListAsync();

        foreach (var item in result)
            Console.WriteLine(item.Name);
    }

    static async Task RawSQLQuery()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n------ Query EF with Raw SQL -----\n");

        // Have to return all properties of the entity type
        // column names must match the entity properties
        var name = "Grimsby Town";
        var teams1 = await context.Teams
            //.FromSqlRaw($"SELECT * FROM Teams WHERE Name = '{name}'") // Don't do this, SQLi!!
            .FromSqlRaw("SELECT * FROM Teams WHERE Name = {0}", name) // Do this to stop SQLi
            .ToListAsync();

        var teams2 = await context.Teams
            .FromSqlInterpolated($"SELECT * FROM Teams WHERE Name = {name}")
            .ToListAsync();
    }

    static async Task QueryView()
    {
        var teamsDetails = await context.TeamsDetails.ToListAsync();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n------ Teams Details View -----\n");

        foreach (var item in teamsDetails)
            Console.WriteLine("Name: {0} | Coach: {1} | League {2}", item.Name, item.CoachName, item.LeagueName);
    }

    static async Task FilteringWithRelatedData()
    {
        var leagues = await context.Leagues
            .Where(l => l.Teams.Any(t => t.Name.Contains("United")))
            .ToListAsync();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n------ Leagues -----\n");

        foreach (var league in leagues)
            Console.WriteLine("{0}: {1}", league.Id, league.Name);
    }

    static async Task SelectOneProperty()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n------ Team Names -----\n");

        var teams = await context.Teams.Select(t => t.Name).ToListAsync();

        foreach (var team in teams)
            Console.WriteLine(team);
    }

    static async Task AnonymousProjection()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n------ Teams with coach (Anonymous projection) -----\n");

        var teams = await context.Teams
            .Include(t => t.Coach)
            .Select(t => new
            {
                TeamName = t.Name,
                CoachName = t.Coach != null ? t.Coach.Name : ""
            })
            .ToListAsync();

        foreach (var item in teams)
            Console.WriteLine("Team: {0} | Coach: {1}", item.TeamName, item.CoachName);
    }

    static async Task StronglyTypedProjection()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n------ Teams with coach and league (strongly typed projection) -----\n");

        var teams = await context.Teams
            .Include(t => t.Coach)
            .Include(t => t.League)
            .Select(t => new TeamDetail
            {
                Name = t.Name,
                CoachName = t.Coach != null ? t.Coach.Name : "",
                LeagueName = t.League.Name
            })
            .ToListAsync();

        foreach (var item in teams)
            Console.WriteLine(item.ToString());
    }

    static async Task QueryRelatedData()
    {
        // Get many related records: Leagues -> Teams
        var leagues = await context.Leagues
            .Include(l => l.Teams)
            .ToListAsync();

        // Get one related record: Team -> Coach
        var teamWithCoach = await context.Teams
            .Include(t => t.Coach)
            .FirstOrDefaultAsync(t => t.Id == 7);

        // Get "grand children" related record: Team -> Matches -> Home/Away Team
        var teamsWithMatchesAndOpponents = await context.Teams
            .Include(q => q.AwayMatches).ThenInclude(q => q.HomeTeam).ThenInclude(q => q.Coach)
            .Include(q => q.HomeMatches).ThenInclude(q => q.AwayTeam).ThenInclude(q => q.Coach)
            .FirstOrDefaultAsync(q => q.Id == 1);

        // Get Includes with filters
        var teams = await context.Teams
            .Where(q => q.HomeMatches.Count() > 0)
            .Include(q => q.Coach)
            .ToListAsync();
    }

    static async Task AddScottishPremiership()
    {
        var teams = new List<Team>
        {
            new Team { Name = "Celtic" },
            new Team { Name = "Rangers" },
            new Team { Name = "Heart of Midlothian" },
            new Team { Name = "Dundee United" },
            new Team { Name = "Motherwell" },
            new Team { Name = "St Mirren" },
            new Team { Name = "Hibernian" },
            new Team { Name = "Aberdeen" },
            new Team { Name = "Livingston" },
            new Team { Name = "Ross County" },
            new Team { Name = "Dundee" },
            new Team { Name = "St Johnstone" },
        };
        var league = new League
        {
            Name = "Scottish Premiership",
            Teams = teams,
        };

        await context.AddAsync(league);
        await context.SaveChangesAsync();
    }

    static async Task AddMufcToPremierLeague()
    {
        // Add to Premier League
        var mufc = new Team
        {
            Name = "Manchester United",
            LeagueId = 1
        };

        await context.Teams.AddAsync(mufc);
        await context.SaveChangesAsync();
    }

    static async Task AddMatchesNationalLeague()
    {
        var matches = new List<Match>
        {
            new Match
            {
                AwayTeamId = 1,
                HomeTeamId = 2,
                Date = new DateTime(2022, 2, 19)
            },
            new Match
            {
                AwayTeamId = 3,
                HomeTeamId = 4,
                Date = new DateTime(2022, 2, 19)
            },
            new Match
            {
                AwayTeamId = 1,
                HomeTeamId = 3,
                Date = new DateTime(2022, 3, 5)
            },
            new Match
            {
                AwayTeamId = 4,
                HomeTeamId = 1,
                Date = new DateTime(2022, 3, 19)
            },
        };
        await context.AddRangeAsync(matches);
        await context.SaveChangesAsync();
    }

    static async Task AddTuchel()
    {
        var coach = new Coach
        {
            Name = "Thomas Tuchel",
            TeamId = 7
        };
        await context.AddAsync(coach);
        await context.SaveChangesAsync();
    }

    static async Task TrackingVsNoTracking()
    {
        var withTracking = await context.Teams.FirstOrDefaultAsync(l => l.Id == 11);
        var withNoTracking = await context.Teams.AsNoTracking().FirstOrDefaultAsync(l => l.Id == 1);

        withTracking.Name = "Manchester City";
        withNoTracking.Name = "GTFC";

        var entriesBeforeSave = context.ChangeTracker.Entries();
        Console.WriteLine("Entries Before Save: {0}", entriesBeforeSave.Count());

        await context.SaveChangesAsync();

        var entriesAfterSave = context.ChangeTracker.Entries();
        Console.WriteLine("Entries After Save: {0}", entriesAfterSave.Count());
    }

    static async Task SimpleDelete()
    {
        Console.WriteLine("-----------------------------------------------------------");
        Console.WriteLine("-------------- Leagues: Delete without teams --------------");
        Console.WriteLine("-----------------------------------------------------------\n");

        // Retrieve the record
        var league = await context.Leagues.FindAsync(8);

        // Delete the record
        context.Leagues.Remove(league);

        // Save the changes
        await context.SaveChangesAsync();
    }

    static async Task SimpleDeleteWithRelationship()
    {
        Console.WriteLine("-----------------------------------------------------------");
        Console.WriteLine("---------------- Leagues: Delete with teams ---------------");
        Console.WriteLine("-----------------------------------------------------------\n");

        var league = await context.Leagues.FindAsync(7);
        context.Leagues.Remove(league);
        await context.SaveChangesAsync();
    }

    static async Task SimpleUpdateTeam()
    {
        var team = new Team
        {
            Id = 7,
            Name = "Chelsea",
            LeagueId = 1
        };

        // Will run an update sql statement if the item exists and the id is specified, else:
        // If the Id is missing from the instance, it will insert a new record
        // If the Id has been added but does not exist, an exception will be thrown
        context.Teams.Update(team);

        // Save the changes
        await context.SaveChangesAsync("Test Audit User");

        Console.WriteLine("{0} - {1}, League: {2}", team.Id, team.Name, team.LeagueId);
    }

    static async Task SimpleUpdateLeague()
    {
        // Retrieve record
        var league = await context.Leagues.FindAsync(5);

        // Make changes to record
        league.Name = "Fifa World Cup";

        // Save the changes
        await context.SaveChangesAsync("Test Audit User");

        Console.WriteLine("{0} - {1}", league.Id, league.Name);
    }

    static async Task AlternativeLinqSyntax()
    {
        Console.Write("Enter Team Name (Or Part Of): ");
        var nameInput = Console.ReadLine();

        var teams = await (from t in context.Teams
                            where EF.Functions.Like(t.Name, $"%{nameInput}%")
                            select t).ToListAsync();

        foreach (var team in teams)
            Console.WriteLine("{0}: {1}, League: {2}", team.Id, team.Name, team.LeagueId);
    }

    static async Task AdditionalExecutionMethods()
    {
        var leagues = context.Leagues;

        var list = await leagues.ToListAsync();

        // Options for getting first item
        var first = await leagues.FirstAsync();
        var firstOrDefault = await leagues.FirstOrDefaultAsync();

        // Options for getting a single item
        //var single = await leagues.SingleAsync();
        //var singleOrDefault = await leagues.SingleOrDefaultAsync();

        // Aggregates
        var count = await leagues.CountAsync();
        var longCount = await leagues.LongCountAsync();
        // var min = await leagues.MinAsync();
        // var max = await leagues.MaxAsync();

        // DbSet method that will execute
        var league = await leagues.FindAsync(2);
    }

    static async Task QueryFilters()
    {
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("-------------- Leagues Filtered --------------");
        Console.WriteLine("----------------------------------------------\n");

        Console.Write("Enter League Name (Or Part Of): ");
        var nameInput = Console.ReadLine();

        // Will parameterise the query when the filter value is not hard coded to combat SQLi
        // Otherwise hard coded values are inserted directly into the SQL query by EF
        // without parametisation as they're deemed "safe"
        var exactMatches = await context.Leagues.Where(l => l.Name == nameInput).ToListAsync();
        Console.WriteLine("\nExact Matches:");
        foreach (var league in exactMatches)
            Console.WriteLine("{0}: {1}", league.Id, league.Name);

        Console.WriteLine("");

        // Option 1 - using straight up linq
        //var partialMatches = await context.Leagues.Where(l => l.Name.Contains(name)).ToListAsync();

        // Options 2 - using EF.Functions
        var partialMatches = await context.Leagues.Where(l => EF.Functions.Like(l.Name, $"%{nameInput}%")).ToListAsync();

        Console.WriteLine("\nPartial Matches:");
        foreach (var league in partialMatches)
            Console.WriteLine("{0}: {1}", league.Id, league.Name);
    }

    static async Task SimpleSelectQuery()
    {
        // Most efficient way
        // using context.Leagues without the Linq op would
        // leave the underlying db connection open
        var leagues = await context.Leagues.ToListAsync();

        Console.WriteLine("-------------------------------------");
        Console.WriteLine("-------------- Leagues --------------");
        Console.WriteLine("-------------------------------------\n");

        foreach (var league in leagues)
            Console.WriteLine("{0}: {1}", league.Id, league.Name);
    }

    static async Task AddNewLeagueSimple()
    {
        var league = new League { Name = "Sample League" };
        await context.Leagues.AddAsync(league);
        await context.SaveChangesAsync();

        await AddTeamsWithLeagueSimple(league);
        await context.SaveChangesAsync();
    }

    static async Task AddTeamsWithLeagueSimple(League league)
    {
        var teams = new List<Team> 
        {
            new Team 
            { 
                Name = "Sample Team 1", 
                LeagueId = league.Id 
            },
            new Team
            {
                Name = "Sample Team 2",
                LeagueId = league.Id
            },
        };

        await context.Teams.AddRangeAsync(teams);
    }
}
