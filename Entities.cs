using System.ComponentModel.DataAnnotations;

namespace Jelly
{
    public class SongEntity
    {
        [Key]
        public int SongID { get; set; }
        public string? SongName { get; set; }
        
        public int AlbumID { get; set; }
        public string? AlbumName { get; set; }
        
        public int ArtistID { get; set; }
        public string? ArtistName { get; set; }
        
        //URLS
        public string? MP3URL { get; set; }
        public string? CoverURL { get; set; }
    }
    
    public class AlbumEntity
    {
        [Key]
        public int AlbumID { get; set; }
        public string? AlbumName { get; set; }
        
        public int ArtistID { get; set; }
        public string? ArtistName { get; set; }
        
        public List<int>? SongIDs { get; set; }
        public List<string>? SongNames { get; set; }    
        
        //URLS
        public string? CoverURL { get; set; }
    }
    
    public class ArtistEntity
    {
        [Key]
        public int ArtistID { get; set; }
        public string? ArtistName { get; set; }

        public List<int>? AlbumIDs { get; set; }
        public List<string>? AlbumNames { get; set; } 
        
        //URLS
        public string? PictureURL { get; set; }
    }
}