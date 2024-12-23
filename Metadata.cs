namespace Jelly
{
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
        
        //Info
        public string? Tagline { get; set; } 
        public string? Description { get; set; } 
    }
}