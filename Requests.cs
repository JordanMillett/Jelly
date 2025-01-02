namespace Jelly
{
    public class IDRequest
    {
        public int ID { get; set; }
    }
    
    public class ResourceRequest
    {
        public string? URL { get; set; }
    }
    
    public class PartialResourceRequest
    {
        public string? URL { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }
    
    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}