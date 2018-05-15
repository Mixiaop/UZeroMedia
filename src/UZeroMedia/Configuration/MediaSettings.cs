using U.Settings;

namespace UZeroMedia.Configuration
{
    [USettingsPathArribute("MediaSettings.json", "/Config/UZeroMedia")]
    public class MediaSettings : U.Settings.USettings<MediaSettings>
    {
        /// <summary>
        /// 当前 SOA 应用名称
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// UZeroMedia.Soa 应用所有接口是否开启签名
        /// </summary>
        public int SOASignatureEnabled { get; set; }

        /// <summary>
        /// SOA 的签名KEY（在请求头Headers里，如：UZeroMedia_Signature）
        /// </summary>
        public string SOASignatureKey { get; set; }

        /// <summary>
        /// SOA 签名过期时间默认 1 分钟
        /// </summary>
        public int SOAExpiresTime { get; set; }

        /// <summary>
        /// SOA 帮助页路径，默认为 /Help
        /// </summary>
        public string SOAHelpPath { get; set; }

        /// <summary>
        /// SOA介绍
        /// </summary>
        public string SOAIntroduction { get; set; }

        #region Pictures
        /// <summary>
        /// 图片根目录
        /// </summary>
        public string PicturePath { get; set; }

        /// <summary>
        /// 图片（缩略图）根目录
        /// </summary>
        public string PictureThumbPath { get; set; }

        /// <summary>
        /// 上传文件（附件）根目录
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 最大图片尺寸
        /// </summary>
        public int MaximumImageSize { get; set; }

        /// <summary>
        /// 默认图片质量
        /// </summary>
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// 存放位置（null表示当前，或：http://img.youzy.cn）
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 显示位置，读写分离时使用
        /// </summary>
        public string ShowLocation { get; set; }
        #endregion

        #region WaterMark
        /// <summary>
        /// 是否开启水印
        /// </summary>
        public bool WmEnable { get; set; }

        /// <summary>
        /// 水印图片路径
        /// </summary>
        public string WmPicturePath { get; set; }

        /// <summary>
        /// 水印位置
        /// </summary>
        public int WmPositions { get; set; }

        /// <summary>
        /// 水印比例（1-100）
        /// </summary>
        public int WmScale { get; set; }

        /// <summary>
        /// 水印透明度（1-100）
        /// </summary>
        public int WmTransparency { get; set; }

        /// <summary>
        /// 当图片宽大于此值才会加水印（像素）
        /// </summary>
        public int WmOnlyLargerThen { get; set; }
        #endregion
    }
}
