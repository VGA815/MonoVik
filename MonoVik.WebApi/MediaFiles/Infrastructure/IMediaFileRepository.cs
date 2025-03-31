using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.MediaFiles.Infrastructure
{
    public interface IMediaFileRepository
    {
        Task InsertAsync(MediaFile mediaFile, ApplicationContext context);
        Task DeleteAsync(Guid fileId, ApplicationContext context);
        Task<MediaFile> GetByIdAsync(Guid fileId, ApplicationContext context);
        IQueryable<MediaFile> GetByUploader(Guid uploaderId, int page, int pageSize, ApplicationContext context);
        Task UpdateAsync(MediaFile mediaFile, ApplicationContext context);
    }
}
