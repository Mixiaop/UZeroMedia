using U.Application.Services.Dto;

namespace UZeroMedia.Services.Dto
{
    /// <summary>
    /// 图片模型
    /// </summary>
    public class PictureDto : U.Application.Services.Dto.IDto
    {
        public PictureDto()
        {
            PictureId = 0;
            PictureUrl = string.Empty;
            ThumbUrl = string.Empty;
            SquareUrl = string.Empty;
            ShowPictureUrl = string.Empty;
            ShowThumbUrl = string.Empty;
            ShowSquareUrl = string.Empty;
        }

        /// <summary>
        /// 图片Id
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 小方块地址
        /// </summary>
        public string SquareUrl { get; set; }

        /// <summary>
        /// 读写分离，显示使用
        /// </summary>
        public string ShowPictureUrl { get; set; }

        /// <summary>
        /// 读写分离，显示使用
        /// </summary>
        public string ShowThumbUrl { get; set; }

        /// <summary>
        /// 读写分离，显示使用
        /// </summary>
        public string ShowSquareUrl { get; set; }
    }
}
