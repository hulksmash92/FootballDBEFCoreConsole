namespace EntityFrameworkNet5.Domain.Models;
public class TeamDetail
{
    public string Name { get; set; }
    public string CoachName { get; set; }
    public string LeagueName { get; set; }

    public override string ToString()
    {
        return string.Format(
                "Name: {0} | Coach: {1} | League {2}", 
                Name, 
                CoachName, 
                LeagueName
            );
    }
}
