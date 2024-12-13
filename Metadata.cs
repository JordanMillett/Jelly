namespace Jelly
{
    public class SongMetadata
    {
        public string? SongName { get; set; }
        public string? AlbumName { get; set; }
        public string? ArtistName { get; set; }
    }
    
    public class AlbumMetadata
    {
        public string? AlbumName { get; set; }
        public string? ArtistName { get; set; }
        public List<string>? SongNames { get; set; }    
    }
    
    public class ArtistMetadata
    {
        public string? ArtistName { get; set; }
        public List<string>? AlbumNames { get; set; } 
    }
}