using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using U.Logging;
using U.Utilities.Web;
using UZeroMedia.Configuration;
using UZeroMedia.Domain;

namespace UZeroMedia.Services
{
    /// <summary>
    /// 带水印的图片服务类，继承自PictureService
    /// </summary>
    public class WaterMarkService : PictureService
    {
        #region Fields Constroctor
        private readonly string THUMB_NAME_PREFIX = "pm";
        private static readonly object s_lock = new object();
        private readonly IPictureRepository _pictureRepository;
        private readonly MediaSettings _mediaSettings;
        private Image _waterMarkImage;

        public WaterMarkService(IPictureRepository pictureRepository,
            MediaSettings mediaSettings) :
            base(pictureRepository, mediaSettings)
        {
            _pictureRepository = pictureRepository;
            _mediaSettings = mediaSettings;

            if (_mediaSettings.WmEnable)
                this._waterMarkImage = Image.FromFile(WebHelper.MapPath(_mediaSettings.WmPicturePath));
        }
        #endregion

        #region Utilities
        private Rectangle Rectangle(int imageWidth, int imageHeight, int waterMarkWidth, int waterMarkHeight, WaterMarkPositions position)
        {
            imageWidth = imageWidth > 0 ? imageWidth : 100;
            imageHeight = imageHeight > 0 ? imageHeight : 100;
            waterMarkWidth = waterMarkWidth > 0 ? waterMarkWidth : 100;
            waterMarkHeight = waterMarkHeight > 0 ? waterMarkHeight : 100;
            int left = 0;
            int top = 0;
            int width = imageWidth / 100 * (int)this._mediaSettings.WmScale;
            int height = waterMarkHeight * width / waterMarkWidth;

            switch (position)
            {
                case WaterMarkPositions.TopLeft:
                    left = 10;
                    top = 10;
                    break;
                case WaterMarkPositions.TopRight:
                    left = imageWidth - width - 10;
                    top = 10;
                    break;
                case WaterMarkPositions.TopMiddle:
                    left = (imageWidth - width - 10) / 2;
                    top = 10;
                    break;
                case WaterMarkPositions.BottomLeft:
                    left = 10;
                    top = imageHeight - height - 10;
                    break;
                case WaterMarkPositions.BottomRight:
                    left = imageWidth - width - 10;
                    top = imageHeight - height - 10;
                    break;
                case WaterMarkPositions.BottomMiddle:
                    left = (imageWidth - width - 10) / 2;
                    top = imageHeight - height - 10;
                    break;
                case WaterMarkPositions.MiddleLeft:
                    left = 10;
                    top = (imageHeight - height - 10) / 2;
                    break;
                case WaterMarkPositions.MiddleRight:
                    left = imageWidth - width - 10;
                    top = (imageHeight - height - 10) / 2;
                    break;
                case WaterMarkPositions.Center:
                    left = (imageWidth - width - 10) / 2;
                    top = (imageHeight - height - 10) / 2;
                    break;
            }
            return new Rectangle(left > 0 ? left : 10, top > 0 ? top : 10, width > 0 ? width : 100, height > 0 ? height : 100);
        }

        public virtual void PlaceWaterMark(ref Graphics g, int width, int height, WaterMarkPositions position)
        {
            if (g.DpiX > this._waterMarkImage.Width)
                g.SmoothingMode = SmoothingMode.AntiAlias;
            byte transparency = (byte)this._mediaSettings.WmTransparency;
            int width1 = this._waterMarkImage.Width;
            int height1 = this._waterMarkImage.Height;
            Rectangle destRect = Rectangle(width, height, width1, height1, position);
            float num4 = (float)transparency / 100f;
            if ((double)num4 < 0.0 || (double)num4 > 1.0)
                num4 = 1f;
            float[][] newColorMatrix1 = new float[5][];
            float[][] numArray1 = newColorMatrix1;
            int index1 = 0;
            float[] numArray2 = new float[5];
            numArray2[0] = 1f;
            float[] numArray3 = numArray2;
            numArray1[index1] = numArray3;
            float[][] numArray4 = newColorMatrix1;
            int index2 = 1;
            float[] numArray5 = new float[5];
            numArray5[1] = 1f;
            float[] numArray6 = numArray5;
            numArray4[index2] = numArray6;
            float[][] numArray7 = newColorMatrix1;
            int index3 = 2;
            float[] numArray8 = new float[5];
            numArray8[2] = 1f;
            float[] numArray9 = numArray8;
            numArray7[index3] = numArray9;
            float[][] numArray10 = newColorMatrix1;
            int index4 = 3;
            float[] numArray11 = new float[5];
            numArray11[3] = num4;
            float[] numArray12 = numArray11;
            numArray10[index4] = numArray12;
            float[][] numArray13 = newColorMatrix1;
            int index5 = 4;
            float[] numArray14 = new float[5];
            numArray14[4] = 1f;
            float[] numArray15 = numArray14;
            numArray13[index5] = numArray15;
            ColorMatrix newColorMatrix2 = new ColorMatrix(newColorMatrix1);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(newColorMatrix2);
            imageAttr.SetColorKey(Color.White, Color.White);
            g.DrawImage(this._waterMarkImage, destRect, 0, 0, width1, height1, GraphicsUnit.Pixel);

        }

        /// <summary>
        /// 通过指定的mime type,返回第一个ImageCodecInfo的实例
        /// </summary>
        /// <param name="mimeType">mime type</param>
        /// <returns></returns>
        private ImageCodecInfo GetImageCodecInfoFromMimeType(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            foreach (var ici in info)
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return ici;
            return null;
        }

        /// <summary>
        /// 通过指定的后缀，返回第一个ImageCodecInfo的实例
        /// </summary>
        /// <param name="fileExt">后缀</param>
        /// <returns></returns>
        private ImageCodecInfo GetImageCodecInfoFromExtension(string fileExt)
        {
            fileExt = fileExt.TrimStart(".".ToCharArray()).ToLower().Trim();
            switch (fileExt)
            {
                case "jpg":
                case "jpeg":
                    return GetImageCodecInfoFromMimeType("image/jpeg");
                case "png":
                    return GetImageCodecInfoFromMimeType("image/png");
                case "gif":
                    //use png codec for gif to preserve transparency
                    //return GetImageCodecInfoFromMimeType("image/gif");
                    return GetImageCodecInfoFromMimeType("image/png");
                default:
                    return GetImageCodecInfoFromMimeType("image/jpeg");
            }
        }

        /// <summary>
        /// 是否同意生成水印
        /// </summary>
        /// <param name="pictureId"></param>
        /// <returns></returns>
        private bool ShouldApply(int pictureId)
        {
            return _mediaSettings.WmEnable;
        }
        #endregion

        public override string GetPictureUrl(int pictureId, int targetSize = 0, string storeLocation = null)
        {
            var picture = base.GetPictureById(pictureId);
            if (picture != null)
            {
                return this.GetPictureUrl(picture, targetSize, storeLocation);
            }

            return string.Empty;
        }

        public override string GetPictureUrl(Picture picture, int targetSize = 0, string storeLocation = null)
        {
            if (picture != null)
            {
                bool shouldGenerateWaterMark = ShouldApply(picture.Id);

                if (!shouldGenerateWaterMark)
                    return base.GetPictureUrl(picture, targetSize, storeLocation);
            }


            string url = string.Empty;
            byte[] pictureBinary = null;
            if (picture != null)
                pictureBinary = LoadPictureBinary(picture);

            if (picture == null || pictureBinary == null || pictureBinary.Length == 0)
            {
                //未找到图片，返回默认图片URL
                return url;
            }

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string thumbFileName;
            if (picture.IsNew)
            {
                DeletePictureThumbs(picture);

                //不确认图片二进制是否验证，确保没有异常抛出
                picture = UpdatePicture(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    picture.SeoFilename,
                    false,
                    false);
            }
            lock (s_lock)
            {
                string seoFileName = picture.SeoFilename;
                if (targetSize == 0)
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{3}{0}_{1}.{2}", picture.Id.ToString("0000000"), seoFileName, lastPart, THUMB_NAME_PREFIX) :
                        string.Format("{2}{0}.{1}", picture.Id.ToString("0000000"), lastPart, THUMB_NAME_PREFIX);

                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        if (pictureBinary != null)
                        {
                            using (MemoryStream ms = new MemoryStream(pictureBinary))
                            {
                                if (ms.Length > 0)
                                {
                                    Image img = Image.FromStream(ms);
                                    if (img.Width >= this._mediaSettings.WmOnlyLargerThen)
                                    {
                                        Graphics g = Graphics.FromImage(img);
                                        foreach (WaterMarkPositions position in Enum.GetValues(typeof(WaterMarkPositions)))
                                        {
                                            if (((int)position & (int)this._mediaSettings.WmPositions) == (int)position)
                                            {
                                                PlaceWaterMark(ref g, img.Width, img.Height, position);
                                            }
                                        }
                                        if (g != null)
                                            g.Dispose();
                                    }
                                    EncoderParameters encoderParameters = new EncoderParameters();
                                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, this._mediaSettings.DefaultImageQuality);
                                    ImageCodecInfo imageCodecInfo = this.GetImageCodecInfoFromExtension(lastPart) ??
                                                                    this.GetImageCodecInfoFromMimeType(picture.MimeType);
                                    img.Save(thumbFilePath, imageCodecInfo, encoderParameters);
                                }
                            }
                        }
                        //File.WriteAllBytes(thumbFilePath, pictureBinary);
                    }
                }
                else
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{4}{0}_{1}_{2}.{3}", picture.Id.ToString("00000000"), seoFileName, targetSize, lastPart, THUMB_NAME_PREFIX) :
                        string.Format("{3}{0}_{1}.{2}", picture.Id.ToString("00000000"), targetSize, lastPart, THUMB_NAME_PREFIX);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        using (var stream = new MemoryStream(pictureBinary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch 确保图片二进制正确
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException exc)
                            {
                                LogHelper.Logger.Error(string.Format("错误：生成缩略图.Id={0}", picture.Id), exc);
                            }
                            if (b == null)
                            {
                                //无法加载bitmap的一些原因
                                return url;
                            }

                            var newSize = CalculateDimensions(b.Size, targetSize);
                            if (newSize.Width < 1)
                                newSize.Width = 1;
                            if (newSize.Height < 1)
                                newSize.Height = 1;

                            using (var newBitMap = new Bitmap(newSize.Width, newSize.Height))
                            {
                                var g = Graphics.FromImage(newBitMap);

                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.CompositingQuality = CompositingQuality.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.DrawImage(b, 0, 0, newSize.Width, newSize.Height);
                                var ep = new EncoderParameters();
                                ep.Param[0] = new EncoderParameter(Encoder.Quality, _mediaSettings.DefaultImageQuality);
                                ImageCodecInfo ici = GetImageCodecInfoFromExtension(lastPart);
                                if (ici == null)
                                    ici = GetImageCodecInfoFromMimeType("image/jpeg");

                                if (newSize.Width >= this._mediaSettings.WmOnlyLargerThen)
                                {
                                    foreach (WaterMarkPositions position in Enum.GetValues(typeof(WaterMarkPositions)))
                                    {
                                        if (((int)position & (int)this._mediaSettings.WmPositions) == (int)position)
                                        {
                                            PlaceWaterMark(ref g, newSize.Width, newSize.Height, position);
                                        }
                                    }
                                }

                                newBitMap.Save(thumbFilePath, ici, ep);
                                if (g != null)
                                    g.Dispose();
                            }
                            b.Dispose();
                        }
                    }
                }
            }
            url = GetThumbUrl(thumbFileName, storeLocation);
            return url;
        }
        public void ClearThumbs()
        {
            string searchPattern = "*.*";

            string path = WebHelper.MapPath(_mediaSettings.PictureThumbPath);
            if (!Directory.Exists(path))
                return;
            foreach (string str in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
            {
                try
                {
                    System.IO.File.Delete(this.GetThumbLocalPath(str));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
