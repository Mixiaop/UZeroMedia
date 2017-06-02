using System;
using System.Collections.Generic;
using System.Drawing;
using ImageResizer;
using U;
using U.Logging;
using U.Utilities.Web;
using UZeroMedia.Configuration;
using UZeroMedia.Domain;

namespace UZeroMedia.Services
{
    /// <summary>
    /// 图片服务接口实现
    /// </summary>
    public class PictureService : IPictureService
    {
        #region Fields & Ctor
        private static readonly object s_lock = new object();
        private readonly IPictureRepository _pictureRepository;
        private readonly MediaSettings _mediaSettings;
        private readonly string _fileLength = "00000000"; //文件长度位数，目前到千W
        public PictureService(IPictureRepository pictureRepository, MediaSettings mediaSettings)
        {
            _pictureRepository = pictureRepository;
            _mediaSettings = mediaSettings;
        }
        #endregion

        #region Utilities
        protected virtual Size CalculateDimensions(Size originalSize, int targetSize,
            ResizeType resizeType = ResizeType.LongestSide, bool ensureSizePositive = true)
        {
            var newSize = new Size();
            switch (resizeType)
            {
                case ResizeType.LongestSide:
                    if (originalSize.Height > originalSize.Width)
                    {
                        // portrait 
                        newSize.Width = (int)(originalSize.Width * (float)(targetSize / (float)originalSize.Height));
                        newSize.Height = targetSize;
                    }
                    else
                    {
                        // landscape or square
                        newSize.Height = (int)(originalSize.Height * (float)(targetSize / (float)originalSize.Width));
                        newSize.Width = targetSize;
                    }
                    break;
                case ResizeType.Width:
                    newSize.Height = (int)(originalSize.Height * (float)(targetSize / (float)originalSize.Width));
                    newSize.Width = targetSize;
                    break;
                case ResizeType.Height:
                    newSize.Width = (int)(originalSize.Width * (float)(targetSize / (float)originalSize.Height));
                    newSize.Height = targetSize;
                    break;
                default:
                    throw new Exception("Not supported ResizeType");
            }

            if (ensureSizePositive)
            {
                if (newSize.Width < 1)
                    newSize.Width = 1;
                if (newSize.Height < 1)
                    newSize.Height = 1;
            }

            return newSize;
        }

        /// <summary>
        /// 保存图片到文件系统
        /// </summary>
        /// <param name="pictureId">图片标识id</param>
        /// <param name="pictureBinary">图片二进制</param>
        /// <param name="mimeType">Mime type</param>
        protected virtual void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType)
        {
            string lastPart = GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("p{0}.{1}", pictureId.ToString("00000000"), lastPart);
            System.IO.File.WriteAllBytes(GetPictureLocalPath(fileName), pictureBinary);
        }

        /// <summary>
        /// 从Mime type中返回文件的扩展名
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>文件扩展名</returns>
        protected virtual string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            //also see System.Web.MimeMapping for more mime types
            if (mimeType.Contains("\""))
                mimeType = mimeType.Replace("\"", "");
            mimeType = mimeType.Trim();

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        /// <summary>
        /// 获取图片存放路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        protected virtual string GetPictureLocalPath(string fileName)
        {
            var imagesDirectoryPath = WebHelper.MapPath(_mediaSettings.PicturePath);
            if (!System.IO.Directory.Exists(imagesDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(imagesDirectoryPath);
            }
            var filePath = System.IO.Path.Combine(imagesDirectoryPath, fileName);
            return filePath;
        }

        /// <summary>
        /// 获取图片（缩略图）本地存放地址
        /// </summary>
        /// <param name="thumbFileName"></param>
        /// <returns></returns>
        protected virtual string GetThumbLocalPath(string thumbFileName)
        {
            var thumbsDirectoryPath = WebHelper.MapPath(_mediaSettings.PictureThumbPath);
            if (!System.IO.Directory.Exists(thumbsDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(thumbsDirectoryPath);
            }
            var thumbFilePath = System.IO.Path.Combine(thumbsDirectoryPath, thumbFileName);
            return thumbFilePath;
        }

        /// <summary>
        /// 加载图片的二进制（根据图片存储文件）
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        protected virtual byte[] LoadPictureBinary(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            return LoadPictureBinary(picture.Id, lastPart);
        }

        /// <summary>
        /// 加载图片的二进制（根据图片Id及内容类型）
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        protected virtual byte[] LoadPictureBinary(int pictureId, string mimeType)
        {
            string fileName = string.Format("p{0}.{1}", pictureId.ToString(_fileLength), mimeType);
            var filePath = GetPictureLocalPath(fileName);
            if (!System.IO.File.Exists(filePath))
                return new byte[0];

            return System.IO.File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// 加载图片的二进制（根据图片Id遍历内容类型）
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        protected virtual PictureBinary LoadPictureBinary(int pictureId)
        {
            PictureBinary picture = new PictureBinary();
            picture.PictureId = pictureId;
            if (pictureId > 0)
            {
                List<string> mimeTypes = new List<string>();
                mimeTypes.Add("jpeg");
                mimeTypes.Add("jpg");
                mimeTypes.Add("png");
                mimeTypes.Add("gif");
                mimeTypes.Add("ico");
                mimeTypes.Add("bmp");
                foreach (var type in mimeTypes)
                {
                    picture.FileName = string.Format("p{0}.{1}", pictureId.ToString(_fileLength), type);
                    var path = GetPictureLocalPath(picture.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        picture.FilePath = path;
                        picture.MineType = type;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(picture.FilePath))
                {
                    picture.Binary = System.IO.File.ReadAllBytes(picture.FilePath);
                }
                else
                    picture.Binary = new byte[0];

            }
            return picture;
        }

        /// <summary>
        /// 删除图片缩略图
        /// </summary>
        /// <param name="picture">图片</param>
        protected virtual void DeletePictureThumbs(Picture picture)
        {
            string filter = string.Format("p{0}*.*", picture.Id.ToString("0000000"));
            var thumbDirectoryPath = WebHelper.MapPath(_mediaSettings.PictureThumbPath);
            string[] currentFiles = System.IO.Directory.GetFiles(thumbDirectoryPath, filter, System.IO.SearchOption.AllDirectories);
            foreach (string currentFileName in currentFiles)
            {
                var thumbFilePath = GetThumbLocalPath(currentFileName);
                System.IO.File.Delete(thumbFilePath);
            }
        }

        /// <summary>
        /// 获取图片（缩略图）URL
        /// </summary>
        /// <param name="thumbFileName">文件名称</param>
        /// <param name="storeLocation">存储位置，null则使用当前的存储位置</param>
        /// <returns></returns>
        protected virtual string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            storeLocation = !String.IsNullOrEmpty(storeLocation)
                                    ? storeLocation
                                    : WebHelper.GetLocation();
            var url = storeLocation + _mediaSettings.PictureThumbPath.Remove(0, 1);

            url = url + thumbFileName;
            return url;
        }
        #endregion

        #region Getting picture local path/URL methods
        /// <summary>
        /// 获取图片Url
        /// </summary>
        /// <param name="picture">图片标识Id</param>
        /// <param name="targetSize">尺寸</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public virtual string GetPictureUrl(int pictureId,
            int targetSize = 0,
            string storeLocation = null)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, targetSize, storeLocation);
        }

        /// <summary>
        /// 获取图片Url
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="targetSize">尺寸</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public virtual string GetPictureUrl(Picture picture, int targetSize = 0, string storeLocation = null)
        {
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
                        string.Format("p{0}_{1}.{2}", picture.Id.ToString("00000000"), seoFileName, lastPart) :
                        string.Format("p{0}.{1}", picture.Id.ToString("00000000"), lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        System.IO.File.WriteAllBytes(thumbFilePath, pictureBinary);
                    }
                }
                else
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("p{0}_{1}_{2}.{3}", picture.Id.ToString("00000000"), seoFileName, targetSize, lastPart) :
                        string.Format("p{0}_{1}.{2}", picture.Id.ToString("00000000"), targetSize, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        using (var stream = new System.IO.MemoryStream(pictureBinary))
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

                            var destStream = new System.IO.MemoryStream();
                            ImageBuilder.Current.Build(b, destStream, new ResizeSettings()
                            {
                                Width = newSize.Width,
                                Height = newSize.Height,
                                Scale = ScaleMode.Both,
                                Quality = _mediaSettings.DefaultImageQuality
                            });
                            var destBinary = destStream.ToArray();
                            System.IO.File.WriteAllBytes(thumbFilePath, destBinary);

                            b.Dispose();
                        }
                    }
                }
            }
            url = GetThumbUrl(thumbFileName, storeLocation);
            return url;
        }

        /// <summary>
        /// 获取图片字节流
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public byte[] GetPictureContents(int pictureId, int targetSize = 0)
        {
            PictureBinary pBinary = LoadPictureBinary(pictureId);
            pBinary.PictureId = pictureId;

            if (pBinary.Binary == null || pBinary.Binary.Length == 0)
            {
                var picture = GetPictureById(pictureId);

                if (picture != null)
                    pBinary.Binary = LoadPictureBinary(picture);

                if (picture == null || pBinary.Binary == null || pBinary.Binary.Length == 0)
                {
                    //未找到图片，返回默认图片URL
                    return pBinary.Binary;
                }
                string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
                pBinary.MineType = lastPart;

                if (picture.IsNew)
                {
                    DeletePictureThumbs(picture);

                    picture = UpdatePicture(picture.Id,
                        pBinary.Binary,
                        picture.MimeType,
                        picture.SeoFilename,
                        false,
                        false);
                }
            }
            lock (s_lock)
            {
                string thumbFileName;
                if (targetSize == 0)
                {
                    thumbFileName = string.Format("p{0}.{1}", pBinary.PictureId.ToString(_fileLength), pBinary.MineType);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        System.IO.File.WriteAllBytes(thumbFilePath, pBinary.Binary);
                    }
                }
                else
                {
                    thumbFileName = string.Format("p{0}_{1}.{2}", pBinary.PictureId.ToString(_fileLength), targetSize, pBinary.MineType);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!System.IO.File.Exists(thumbFilePath))
                    {
                        using (var stream = new System.IO.MemoryStream(pBinary.Binary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch 确保图片二进制正确
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException exc)
                            {
                                LogHelper.Logger.Error(string.Format("错误：生成缩略图.Id={0}", pBinary.PictureId), exc);
                            }
                            if (b == null)
                            {
                                //无法加载bitmap的一些原因
                                return pBinary.Binary;
                            }

                            var newSize = CalculateDimensions(b.Size, targetSize);

                            var destStream = new System.IO.MemoryStream();
                            ImageBuilder.Current.Build(b, destStream, new ResizeSettings()
                            {
                                Width = newSize.Width,
                                Height = newSize.Height,
                                Scale = ScaleMode.Both,
                                Quality = _mediaSettings.DefaultImageQuality
                            });
                            var destBinary = destStream.ToArray();
                            System.IO.File.WriteAllBytes(thumbFilePath, destBinary);
                            pBinary.Binary = destBinary;
                            b.Dispose();
                        }
                    }
                    else
                    {
                        pBinary.Binary = System.IO.File.ReadAllBytes(thumbFilePath);
                    }
                }
            }

            return pBinary.Binary;
        }


        /// <summary>
        /// 获取图片原图Url
        /// </summary>
        /// <param name="picture">图片标识Id</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public virtual string GetRawPictureUrl(int pictureId, string storeLocation = null)
        {
            var picture = GetPictureById(pictureId);
            return GetRawPictureUrl(picture, storeLocation);
        }

        /// <summary>
        /// 获取图片原图Url
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public virtual string GetRawPictureUrl(Picture picture, string storeLocation = null)
        {
            string lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            string fileName = string.Format("p{0}.{1}", picture.Id.ToString("00000000"), lastPart);
            var filePath = GetPictureLocalPath(fileName);
            if (!System.IO.File.Exists(filePath))
                return string.Empty;

            storeLocation = !String.IsNullOrEmpty(storeLocation)
                                    ? storeLocation
                                    : WebHelper.GetLocation();

            var url = storeLocation + _mediaSettings.PicturePath.Remove(0, 1);
            url = url + fileName;
            return url;
        }

        /// <summary>
        /// 获取图片（缩略图）本地路径
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="targetSize">尺寸</param>
        /// <returns></returns>
        public string GetThumbLocalPath(Picture picture, int targetSize = 0)
        {
            string url = GetPictureUrl(picture, targetSize);
            if (String.IsNullOrEmpty(url))
                return String.Empty;
            else
                return GetThumbLocalPath(System.IO.Path.GetFileName(url));
        }
        #endregion

        #region Cut picture
        /// <summary>
        /// 裁剪图片，返回裁剪后的图片对象 
        /// </summary>
        /// <param name="oriPicture">源图片对象</param>
        /// <param name="x">x轴</param>
        /// <param name="y">y轴</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        public Picture CutPicture(Picture oriPicture, int x, int y, int w, int h)
        {
            //返回的图片对象
            Picture picture = null;

            //源图字节
            byte[] oriPictureBinary = null;
            if (oriPicture != null)
                oriPictureBinary = LoadPictureBinary(oriPicture);

            if (oriPicture == null || oriPictureBinary == null || oriPictureBinary.Length == 0)
            {
                //未找到图片
                return picture;
            }

            lock (s_lock)
            {
                using (var stream = new System.IO.MemoryStream(oriPictureBinary))
                {
                    Bitmap b = null;
                    try
                    {
                        //try-catch 确保图片二进制正确
                        b = new Bitmap(stream);
                    }
                    catch (ArgumentException exc)
                    {
                        LogHelper.Logger.Error(string.Format("错误：剪切源图.Id={0}", oriPicture.Id), exc);
                    }
                    if (b == null)
                    {
                        //无法加载bitmap的一些原因
                        return picture;
                    }

                    //目标流
                    var destStream = new System.IO.MemoryStream();
                    int bottom = w + Math.Abs(x);
                    int right = h + Math.Abs(y);

                    var resizeSettings = new ResizeSettings();
                    resizeSettings.Mode = FitMode.Crop;
                    resizeSettings.CropXUnits = 0;
                    resizeSettings.CropYUnits = 0;
                    resizeSettings.CropTopLeft = new PointF(x, y); //从哪开始
                    resizeSettings.CropBottomRight = new PointF(x + h, y + w); //从哪开始
                    //resizeSettings.Width = w;
                    //resizeSettings.Height = h;
                    //resizeSettings.Mode = FitMode.Crop;
                    //resizeSettings.CropTopLeft = new PointF(x, y); //从哪开始
                    ////resizeSettings.CropBottomRight = new PointF(w, h); //从哪结束
                    ImageBuilder.Current.Build(b, destStream, resizeSettings);

                    var destBinary = destStream.ToArray();

                    if (!string.IsNullOrEmpty(oriPicture.MimeType))
                        oriPicture.MimeType = oriPicture.MimeType.Trim();

                    picture = InsertPicture(destBinary, oriPicture.MimeType, oriPicture.SeoFilename, true);
                    b.Dispose();
                }

                return picture;
            }
        }
        #endregion

        #region CURD
        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="pictureId">图片标识Id</param>
        /// <returns></returns>
        public virtual Picture GetPictureById(int pictureId)
        {
            if (pictureId == 0)
                return null;

            return _pictureRepository.Get(pictureId);
        }

        /// <summary>
        /// 插入一张图片
        /// </summary>
        /// <param name="pictureBinary">图片的二进制码</param>
        /// <param name="mineType">图片Mime type</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新图</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        public virtual Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFilename, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);
            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = new Picture()
            {
                MimeType = mimeType,
                IsNew = isNew,
                SeoFilename = seoFilename
            };

            int pictureId = _pictureRepository.InsertAndGetId(picture);

            SavePictureInFile(pictureId, pictureBinary, mimeType);


            return picture;
        }

        /// <summary>
        /// 更新图片
        /// </summary>
        /// <param name="pictureId">需要更新的源图标识Id</param>
        /// <param name="pictureBinary">图片的二进制</param>
        /// <param name="mimeType">图片Mime type</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新图</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        public Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType, string seoFilename, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            if (seoFilename != picture.SeoFilename)
                DeletePictureThumbs(picture);

            picture.MimeType = mimeType;
            picture.SeoFilename = seoFilename;
            picture.IsNew = isNew;

            _pictureRepository.Update(picture);

            SavePictureInFile(picture.Id, pictureBinary, mimeType);

            return picture;

        }
        /// <summary>
        /// 验证图片二进制
        /// </summary>
        /// <param name="pictureBinary">图片二进制</param>
        /// <param name="mimeType">图片Mime type</param>
        /// <returns></returns>
        public virtual byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            var destStream = new System.IO.MemoryStream();
            ImageBuilder.Current.Build(pictureBinary, destStream, new ResizeSettings()
            {
                MaxWidth = _mediaSettings.MaximumImageSize,
                MaxHeight = _mediaSettings.MaximumImageSize,
                Quality = _mediaSettings.DefaultImageQuality
            });
            return destStream.ToArray();
        }
        #endregion

        /// <summary>
        /// 图片二进制类
        /// </summary>
        public class PictureBinary
        {
            public int PictureId { get; set; }
            public string MineType { get; set; }

            public string FileName { get; set; }

            public string FilePath { get; set; }

            public byte[] Binary { get; set; }
        }
    }
}
