using UZeroMedia.Client.Net;

namespace UZeroMedia.Client.Services.Dto
{
    public class PictureDto : WebApiClientModelBase
    {
        /// <summary>
        /// 图片标识Id
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

        public string ShowPictureUrl { get; set; }

        public string ShowThumbUrl { get; set; }

        public string ShowSquareUrl { get; set; }
    }
}
