using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.IO;

namespace UZeroMedia.Services
{
    public static class Extensions
    {
        /// <summary>
        /// 获取图片二进制数组
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static byte[] GetBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            byte[] img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }

        /// <summary>
        /// 获取图片二进制数组
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static byte[] GetBits(this HttpPostedFile postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            byte[] img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }

        /// <summary>
        /// 保存一张图像
        /// </summary>
        /// <param name="bmp">bitmap</param>
        /// <param name="filename">文件名（包含路径）</param>
        public static void SaveJPG100(this Bitmap bmp, string filename)
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        /// <summary>
        /// 保存一张图像
        /// </summary>
        /// <param name="bmp">bitmap</param>
        /// <param name="filename">文件名（包含路径）</param>
        public static void SaveJPG100(this Bitmap bmp, Stream stream)
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        /// <summary>
        /// 获取图像编码信息
        /// </summary>
        /// <param name="format">图像格式</param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            // Return 
            return null;
        }

        /// <summary>
        /// bitmap convert byte[]
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Bitmap bitmap)
        {
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
