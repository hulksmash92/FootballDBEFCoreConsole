using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkNet5.Domain;

public class Match : BaseDomainObject
{
    public DateTime Date { get; set; }

    [Precision(18, 2)]
    public decimal TicketPrice { get; set; }

    public int HomeTeamId { get; set; }
    public virtual Team HomeTeam { get; set; }
    public int AwayTeamId { get; set; }
    public virtual Team AwayTeam { get; set; }
}
