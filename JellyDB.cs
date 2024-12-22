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
    public DbSet<Sequence> Sequence { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Jelly.db");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sequence>(entity =>
        {
            entity.HasNoKey();
        });
    }
    
    public void ConstructDBFromData()
    {
        Console.WriteLine("Constructing");
        
        Dictionary<string, ArtistMetadata> UniqueArtists = new Dictionary<string, ArtistMetadata>();
        Dictionary<string, AlbumMetadata> UniqueAlbums = new Dictionary<string, AlbumMetadata>();
        string artistsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "artists");
        string albumsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "albums");
        
        
        string[] foundFiles = Directory.GetFiles(artistsDir, "*.jpg");
        foreach (string Current in foundFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(Current);

            UniqueArtists[fileName] = new ArtistMetadata
            {
                ArtistName = fileName,
                AlbumNames = new List<string>()
            };
        }
        
        foundFiles = Directory.GetDirectories(albumsDir);
        foreach (string Current in foundFiles)
        {
            string albumName = Path.GetFileName(Current);
            string albumDir = Path.Combine(albumsDir, albumName);
            string metadataFilePath = Path.Combine(albumDir, $"{albumName}.json");
       
            string jsonContent = File.ReadAllText(metadataFilePath);
            AlbumMetadata albumMetadata = JsonSerializer.Deserialize<AlbumMetadata>(jsonContent)!;
            UniqueAlbums[albumName] = albumMetadata;
        }
        
        Songs?.RemoveRange(Songs);
        Albums?.RemoveRange(Albums);
        Artists?.RemoveRange(Artists);
        SaveChanges();
        
        List<ArtistEntity> artistEntities = new List<ArtistEntity>();
        List<AlbumEntity> albumEntities = new List<AlbumEntity>();
        List<SongEntity> songEntities = new List<SongEntity>();
        
        int artistID = 1;
        foreach (var artistPair in UniqueArtists)
        {
            artistEntities.Add(new ArtistEntity
            {
                ArtistID = artistID,
                ArtistName = artistPair.Value.ArtistName,
                AlbumNames = artistPair.Value.AlbumNames,
                PictureFileName =  Uri.EscapeDataString("artists/" + artistPair.Value.ArtistName + ".jpg"),
                AlbumIDs = new List<int>()
            });

            artistID++;
        }
        
        int albumID = 1;
        int songID = 1;
        foreach (var albumPair in UniqueAlbums)
        {
            int? artistIdForAlbum = artistEntities
                .FirstOrDefault(a => a.ArtistName == albumPair.Value.ArtistName)?.ArtistID;

            var albumEntity = new AlbumEntity
            {
                AlbumID = albumID,
                AlbumName = albumPair.Value.AlbumName,
                ArtistID = artistIdForAlbum ?? 0,
                ArtistName = albumPair.Value.ArtistName,
                CoverFileName = Uri.EscapeDataString("albums/" + albumPair.Value.AlbumName + "/" + albumPair.Value.AlbumName + ".jpg"),
                SongIDs = new List<int>(),
                SongNames = albumPair.Value.SongNames
            };

            albumEntities.Add(albumEntity);

            // Add songs in the album
            foreach (var songName in albumPair.Value.SongNames ?? new List<string>())
            {
                var songEntity = new SongEntity
                {
                    SongID = songID,
                    SongName = songName,
                    AlbumID = albumID,
                    AlbumName = albumPair.Value.AlbumName,
                    ArtistID = artistIdForAlbum ?? 0,
                    ArtistName = albumPair.Value.ArtistName,
                    MP3FileName = Uri.EscapeDataString("albums/" + albumPair.Value.AlbumName + "/" + songName + ".mp3"),
                    CoverFileName = Uri.EscapeDataString("albums/" + albumPair.Value.AlbumName + "/" + albumPair.Value.AlbumName + ".jpg"),
                };

                songEntities.Add(songEntity);
                albumEntity.SongIDs?.Add(songID);

                songID++;
            }

            albumID++;
        }
        
        foreach (var album in albumEntities)
        {
            var artist = artistEntities.FirstOrDefault(a => a.ArtistID == album.ArtistID);
            artist?.AlbumIDs?.Add(album.AlbumID);
            artist?.AlbumNames?.Add(album.AlbumName!);
        }
        
        Artists?.AddRange(artistEntities);
        Albums?.AddRange(albumEntities);
        Songs?.AddRange(songEntities);
        
        SaveChanges();
    }
}