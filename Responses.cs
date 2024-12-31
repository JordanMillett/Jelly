namespace Jelly
{
    public class Sequence
    {
        public int Songs { get; set; }
        public int Albums { get; set; }
        public int Artists { get; set; }
    }
    
    public class LoginResponse
    {
        public string? AuthToken { get; set; }
    }
}