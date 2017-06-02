using UZeroMedia.Client.Net;

namespace UZeroMedia.Client.Services.Dto
{
    public class FileDto : WebApiClientModelBase
    {
        public int FileId { get; set; }
        public string FileUrl { get; set; }
    }
}
