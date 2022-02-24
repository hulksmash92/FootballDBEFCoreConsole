namespace EntityFrameworkNet5.Domain;
public class Team : BaseDomainObject
{
    public Team()
    {
        HomeMatches = new HashSet<Match>();
        AwayMatches = new HashSet<Match>();
    }

    public string Name { get; set; }
    public int LeagueId { get; set; }
    public virtual League League { get; set; }
    public virtual Coach Coach { get; set; }
    public virtual ICollection<Match> HomeMatches { get; set; }
    public virtual ICollection<Match> AwayMatches { get; set; }
}
