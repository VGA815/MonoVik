using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.MediaFiles.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.MediaFiles
{
    public class DownloadMediaFile
    {
        public const string Tag = "MediaFiles";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/media", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization()
                    .WithName("DownloadMediaFile");
            }
        }
        public static async Task<IResult> Handler(
            [FromQuery] Guid mediaId, [FromQuery] Guid uploaderId, 
            IMediaFileRepository mediaFileRepository,
            ApplicationContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            MediaFile mediaFile = await mediaFileRepository.GetByIdAsync(mediaId, context);
            if (!mediaFile.IsPublic!.Value)
            {
                if (!currentUserId.Equals(uploaderId)) return Results.Forbid();
            }
            Byte[] b = File.ReadAllBytes($"{configuration["actualPath"]}/{mediaFile.UploaderId}/{mediaFile.MediaId}");
            return Results.File(b, "image/jpeg");
        }
    }
}
