namespace Data
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageString { get; set; } = string.Empty;

        //
        // Summary:
        //     Creation time of this entity.
        //
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}