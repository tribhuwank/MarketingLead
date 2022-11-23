namespace MarketingLead.API.Entities
{
    public abstract class BaseEntity
    {
        public string CreatedBy { get;  set; } = string.Empty;
        public DateTime CreatedOn { get;  set; }
        public string LastUpdatedBy { get;  set; } = string.Empty;
        public DateTime LastUpdatedOn { get;  set; }
    }
}
