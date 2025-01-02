namespace Jelly
{
    public static class LogService
    { 
        public static void AddLog(string message)
        {
            string localTimeFormatted = DateTime.UtcNow.ToLocalTime().ToString("HH:mm:ss:fff");
            Console.WriteLine($"[{localTimeFormatted}] {message}");
        }
    }
}