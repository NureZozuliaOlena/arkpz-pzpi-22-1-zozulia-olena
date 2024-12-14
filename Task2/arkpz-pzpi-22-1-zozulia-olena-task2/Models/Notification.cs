namespace Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public Guid UserId { get; set; }
    }
}
