using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.MediaFiles
{
    public class MediaFile
    {
        public Guid MediaId { get; set; }

        public Guid? UploaderId { get; set; }

        public string FileUrl { get; set; } = null!;

        public string FileType { get; set; } = null!;

        public int FileSize { get; set; }

        public DateTime? UploadDate { get; set; }

        public bool? IsPublic { get; set; }

        public virtual User? Uploader { get; set; }
    }
}
