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
}
