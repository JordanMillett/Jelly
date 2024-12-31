using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Security;

namespace Jelly
{
    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        public override void Configure()
        {
            Post("/api/auth/login");
            AllowAnonymous();
            Throttle(
                hitLimit: 10,
                durationSeconds: 10
            );
        }

        public override async Task HandleAsync(LoginRequest request, CancellationToken token)
        {
            if (request.Username != Config["SecretConfig:Username"] || request.Password != Config["SecretConfig:Password"])
            {
                ThrowError("Invalid username or password");
                return;
            }
            
            string auth = JwtBearer.CreateToken
            (
                o =>
                {
                    o.ExpireAt = DateTime.UtcNow.AddDays(1);
                    o.User.Claims.Add(("UserName", request.Username!));
                }
            );

            await SendAsync(new LoginResponse { AuthToken = auth });
        }
    }
    
    public class GetSequence : EndpointWithoutRequest<Sequence>
    {
        private readonly JellyDB Context;

        public GetSequence(JellyDB context)
        {
            Context = context;
        }

        public override void Configure()
        {
            Get("/api/get/sequence");
        }

        public override async Task HandleAsync(CancellationToken token)
        {
            var sql = @"
                SELECT 
                    COALESCE((SELECT seq FROM sqlite_sequence WHERE name = 'Songs'), 0) AS Songs,
                    COALESCE((SELECT seq FROM sqlite_sequence WHERE name = 'Albums'), 0) AS Albums,
                    COALESCE((SELECT seq FROM sqlite_sequence WHERE name = 'Artists'), 0) AS Artists";

            Sequence result = await Context.Sequence.FromSqlRaw(sql).FirstOrDefaultAsync(token) ?? null!;

            if (result == null)
            {
                await SendNotFoundAsync();
                return;
            }

            await SendAsync(result);
        }
    }

    public class GetSong : Endpoint<IDRequest, SongEntity>
    {
        private readonly JellyDB Context;
        
        public GetSong(JellyDB context)
        {
            Context = context;
        }
        
        public override void Configure()
        {
            Get("/api/get/song/{ID}");
        }

        public override async Task HandleAsync(IDRequest request, CancellationToken token)
        {  
            SongEntity? Song = await Context.Songs!.FirstOrDefaultAsync(s => s.SongID == request.ID);

            if (Song == null)
            {
                await SendNotFoundAsync();
                return;
            }

            await SendAsync(Song);
        }
    }
    
    public class GetAlbum : Endpoint<IDRequest, AlbumEntity>
    {
        private readonly JellyDB Context;
        
        public GetAlbum(JellyDB context)
        {
            Context = context;
        }
        
        public override void Configure()
        {
            Get("/api/get/album/{ID}");
        }

        public override async Task HandleAsync(IDRequest request, CancellationToken token)
        {  
            AlbumEntity? Album = await Context.Albums!.FirstOrDefaultAsync(s => s.AlbumID == request.ID);

            if (Album == null)
            {
                await SendNotFoundAsync();
                return;
            }

            await SendAsync(Album);
        }
    }
    
    public class GetArtist : Endpoint<IDRequest, ArtistEntity>
    {
        private readonly JellyDB Context;
        
        public GetArtist(JellyDB context)
        {
            Context = context;
        }
        
        public override void Configure()
        {
            Get("/api/get/artist/{ID}");
        }

        public override async Task HandleAsync(IDRequest request, CancellationToken token)
        {  
            ArtistEntity? Artist = await Context.Artists!.FirstOrDefaultAsync(s => s.ArtistID == request.ID);

            if (Artist == null)
            {
                await SendNotFoundAsync();
                return;
            }

            await SendAsync(Artist);
        }
    }

    public class GetImage : Endpoint<ResourceRequest>
    {
        private readonly IWebHostEnvironment Context;

        public GetImage(IWebHostEnvironment context)
        {
            Context = context;
        }

        public override void Configure()
        {
            Get("/api/get/image/{URL}");
        }

        public override async Task HandleAsync(ResourceRequest request, CancellationToken token)
        {
            string decodedUrl = Uri.UnescapeDataString(request.URL!);
            string filePath = Path.Combine(Context.WebRootPath, decodedUrl);

            if (!File.Exists(filePath))
            {
                await SendNotFoundAsync();
                return;
            }
            
            var fileInfo = new FileInfo(filePath);

            var contentType = "image/jpeg";
    
            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            await SendFileAsync(fileInfo, contentType);
        }
    }
    
    public class GetFileSize : Endpoint<ResourceRequest>
    {
        private readonly IWebHostEnvironment Context;

        public GetFileSize(IWebHostEnvironment context)
        {
            Context = context;
        }

        public override void Configure()
        {
            Get("/api/get/size/{URL}");
        }

        public override async Task HandleAsync(ResourceRequest request, CancellationToken token)
        {
            string decodedUrl = Uri.UnescapeDataString(request.URL!);
            string filePath = Path.Combine(Context.WebRootPath, decodedUrl);

            if (!File.Exists(filePath))
            {
                await SendNotFoundAsync();
                return;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            long fileSize = fileInfo.Length;

            await SendAsync(fileSize);
        }
    }
    
    public class GetAudioChunk : Endpoint<PartialResourceRequest>
    {
        private readonly IWebHostEnvironment Context;

        public GetAudioChunk(IWebHostEnvironment context)
        {
            Context = context;
        }

        public override void Configure()
        {
            Get("/api/get/audio/");
        }

        public override async Task HandleAsync(PartialResourceRequest request, CancellationToken token)
        {
            string decodedUrl = Uri.UnescapeDataString(request.URL!);
            string filePath = Path.Combine(Context.WebRootPath, decodedUrl);

            if (!File.Exists(filePath))
            {
                await SendNotFoundAsync();
                return;
            }

            long chunkSize = request.End - request.Start + 1;

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Seek(request.Start, SeekOrigin.Begin);
    
            await stream.CopyToAsync(HttpContext.Response.Body, (int)chunkSize, token);
        }
    }
}
