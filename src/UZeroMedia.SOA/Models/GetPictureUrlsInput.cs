namespace UZeroMedia.SOA.Models
{
    public class GetPictureUrlsInput
    {
        public GetPictureUrlsInput()
        {
            TargetSize = 0;
        }
        /// <summary>
        /// 图片Id数组
        /// </summary>
        public int[] PictureIds { get; set; }

        /// <summary>
        /// 目标尺寸（默认为0）
        /// </summary>
        public int TargetSize { get; set; }
    }
}