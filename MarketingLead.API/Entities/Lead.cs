namespace MarketingLead.API.Entities;

public class Lead:BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ClientManagerId { get; set; }
    public User ClientManager { get; set; } = new User();
    public int AccountId { get; set; }
    public Account Account { get; set; } = new Account();
    public bool Status { get; set; }
}
