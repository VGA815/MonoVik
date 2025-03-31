using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.MediaFiles.Infrastructure
{
    public class MediaFileRepository : IMediaFileRepository
    {
        public async Task DeleteAsync(Guid fileId, ApplicationContext context)
        {
            await context.MediaFiles
                .Where(mf => mf.MediaId == fileId)
                .ExecuteDeleteAsync();
        }

        public async Task<MediaFile> GetByIdAsync(Guid fileId, ApplicationContext context)
        {
            return await context.MediaFiles.FirstOrDefaultAsync(mf => mf.MediaId.Equals(fileId));
        }

        public IQueryable<MediaFile> GetByUploader(Guid uploaderId, int page, int pageSize, ApplicationContext context)
        {
            return context.MediaFiles
                .Where(mf => mf.UploaderId == uploaderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task InsertAsync(MediaFile mediaFile, ApplicationContext context)
        {
            context.MediaFiles.Add(mediaFile);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MediaFile mediaFile, ApplicationContext context)
        {
            await context.MediaFiles
                .Where(mf => mediaFile.MediaId ==  mf.MediaId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(mf => mf.IsPublic, mediaFile.IsPublic));
        }
    }
}
