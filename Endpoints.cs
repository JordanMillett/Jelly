using Microsoft.EntityFrameworkCore;

namespace Jelly
{
    public class GetSong : Endpoint<IDRequest, SongEntity>
    {
        private readonly JellyDB Context;
        
        public GetSong(JellyDB context)
        {
            Context = context;
        }
        
        public override void Configure()
        {
            Post("/api/getsong");
            AllowAnonymous();
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

    public class GetImage : Endpoint<ResourceRequest>
    {
        private readonly IWebHostEnvironment Context;

        public GetImage(IWebHostEnvironment context)
        {
            Context = context;
        }

        public override void Configure()
        {
            Get("/api/getimage/{URL}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ResourceRequest request, CancellationToken token)
        {
            string imagesFolder = Path.Combine(Context.WebRootPath, "imgs");

            string filePath = Path.Combine(imagesFolder, request.URL!);

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
}
