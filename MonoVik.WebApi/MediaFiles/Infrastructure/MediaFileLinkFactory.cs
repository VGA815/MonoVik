namespace MonoVik.WebApi.MediaFiles.Infrastructure
{
    public class MediaFileLinkFactory(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        public string Create(Guid uploaderId, Guid mediaId)
        {
            string? verificationLink = linkGenerator.GetUriByName(
                httpContextAccessor.HttpContext!,
                "DownloadMediaFile",
                new { mediaId = mediaId, uploaderId = uploaderId });
            return verificationLink ?? throw new Exception("Could not create media file link");
        }
    }
}
