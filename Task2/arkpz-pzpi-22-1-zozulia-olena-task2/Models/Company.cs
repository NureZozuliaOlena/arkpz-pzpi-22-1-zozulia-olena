namespace Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<User>? Employees { get; set; }
    }
}
