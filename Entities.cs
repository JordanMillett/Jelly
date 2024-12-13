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
        
        //FileNames
        public string? MP3FileName { get; set; }
        public string? CoverFileName { get; set; }
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
        
        //FileNames
        public string? CoverFileName { get; set; }
    }
    
    public class ArtistEntity
    {
        [Key]
        public int ArtistID { get; set; }
        public string? ArtistName { get; set; }

        public List<int>? AlbumIDs { get; set; }
        public List<string>? AlbumNames { get; set; } 
        
        //FileNames
        public string? PictureFileName { get; set; }
    }
}