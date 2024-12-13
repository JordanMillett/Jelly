using Jelly;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;

public class JellyDB : DbContext
{
    public JellyDB(DbContextOptions<JellyDB> options) : base(options) {}

    public DbSet<SongEntity>? Songs { get; set; }
    public DbSet<AlbumEntity>? Albums { get; set; }
    public DbSet<ArtistEntity>? Artists { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Jelly.db");
        }
    }
    
    public void LoadSongMetadataFromJson()
    {
        Dictionary<string, SongMetadata> UniqueSongs = new Dictionary<string, SongMetadata>();
        Dictionary<string, AlbumMetadata> UniqueAlbums = new Dictionary<string, AlbumMetadata>();
        Dictionary<string, ArtistMetadata> UniqueArtists = new Dictionary<string, ArtistMetadata>();
        Dictionary<string, string> FoundCovers = new Dictionary<string, string>(); //URL to JPG
        Dictionary<string, string> FoundPictures = new Dictionary<string, string>(); //URL to JPG
        Dictionary<string, string> FoundMP3s = new Dictionary<string, string>(); //URL to MP3
        
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        if (!Directory.Exists(dir))
            return;
            
        string[] jsonFiles = Directory.GetFiles(dir, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string jsonContent = File.ReadAllText(filePath);
            SongMetadata Data = JsonSerializer.Deserialize<SongMetadata>(jsonContent)!;

            
        }
        
        /*
        if (songMetadata != null)
                {
                    // Create new SongEntity based on SongMetadata
                    var songEntity = new SongEntity
                    {
                        SongName = songMetadata.SongName,
                        AlbumName = songMetadata.AlbumName,
                        ArtistName = songMetadata.ArtistName
                    };

                    Songs?.Add(songEntity);
                }  
                */
        
        SaveChanges();
    }
}