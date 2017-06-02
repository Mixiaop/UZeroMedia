using U.Application.Services.Dto;

namespace UZeroMedia.Services.Dto
{
    public class FileDto : IDto
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// 文件路径地址
        /// </summary>
        public string FileUrl { get; set; }
    }
}
