namespace EntityFrameworkNet5.Domain;
public class League : BaseDomainObject
{
    public League()
    {
        Teams = new HashSet<Team>();
    }

    public string Name { get; set; }
    public virtual ICollection<Team> Teams { get; set; }
}
