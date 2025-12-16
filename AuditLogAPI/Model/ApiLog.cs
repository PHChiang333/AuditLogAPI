namespace AuditLogAPI.Model
{
    public class ApiLog
    {
        public int Id { get; set; }
        public string Path { get; set; } = "";
        public string Method { get; set; } = "";
        public string RequestBody { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
