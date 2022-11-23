using System.ComponentModel.DataAnnotations;

namespace MarketingLead.API.Entities;

public class Team:BaseEntity
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }=string.Empty;
    public int TeamLeadId { get; set; }
    public User TeamLead { get; set; } = new User();
    public int ClientManagerId { get; set; }
    public User ClientManager { get; set; } = new User();
}
