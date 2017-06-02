using System;
using U;
using U.Utilities.Web;
using UZeroMedia.Configuration;
using UZeroMedia.Domain;

namespace UZeroMedia.Services
{
    public class FileService : IFileService
    {
        #region Fields & Ctor
        private static readonly object s_lock = new object();
        private readonly IFileRepository _fileRepository;
        private readonly MediaSettings _mediaSettings;
        public FileService(IFileRepository fileRepository, MediaSettings mediaSettings)
        {
            _fileRepository = fileRepository;
            _mediaSettings = mediaSettings;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// 保存文件到文件系统
        /// </summary>
        /// <param name="fileId">标识id</param>
        /// <param name="fileBinary">文件二进制</param>
        /// <param name="mimeType">Mime type</param>
        protected virtual void SaveFileInFile(int fileId, byte[] fileBinary, string extension)
        {
            string fileName = string.Format("f{0}.{1}", fileId.ToString("00000000"), extension);
            System.IO.File.WriteAllBytes(GetFileLocalPath(fileName), fileBinary);
        }

        /// <summary>
        /// 获取文件存放路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        protected virtual string GetFileLocalPath(string fileName)
        {
            var filesDirectoryPath = WebHelper.MapPath(_mediaSettings.FilePath);
            if (!System.IO.Directory.Exists(filesDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(filesDirectoryPath);
            }
            var filePath = System.IO.Path.Combine(filesDirectoryPath, fileName);
            return filePath;
        }


        /// <summary>
        /// 加载文件的二进制（根据图片存储文件）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual byte[] LoadFileBinary(UZeroMedia.Domain.File file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            string fileName = string.Format("f{0}.{1}", file.Id.ToString("00000000"), file.Extension);
            var filePath = GetFileLocalPath(fileName);
            if (!System.IO.File.Exists(filePath))
                return new byte[0];
            var bytes = System.IO.File.ReadAllBytes(@filePath);
            return bytes;
        }

        /// <summary>
        /// 获取文件URL
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="storeLocation">存储位置，null则使用当前的存储位置</param>
        /// <returns></returns>
        protected virtual string GetFileUrl(string fileName, string storeLocation = null)
        {
            storeLocation = !String.IsNullOrEmpty(storeLocation)
                                    ? storeLocation
                                    : WebHelper.GetLocation();
            var url = storeLocation + _mediaSettings.FilePath.Remove(0, 1);

            url = url + fileName;
            return url;
        }
        #endregion

        #region Getting file local path/URL methods
        /// <summary>
        /// 获取文件Url
        /// </summary>
        /// <param name="fileId">标识Id</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public string GetFileUrl(int fileId, string storeLocation = null)
        {
            var file = GetFileById(fileId);
            return GetFileUrl(file, storeLocation);
        }

        /// <summary>
        /// 获取文件Url
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        public string GetFileUrl(File file, string storeLocation = null)
        {
            string url = string.Empty;
            byte[] fileBinary = null;
            if (file != null)
                fileBinary = LoadFileBinary(file);

            if (file == null || fileBinary == null || fileBinary.Length == 0)
            {
                //未找到文件，返回默认文件URL
                return url;
            }

            string lastPart = file.Extension.Replace(".", "");
            string fileName = "";
            if (file.IsNew)
            {
                file = Update(file.Id,
                    fileBinary,
                    file.MimeType,
                    file.Extension,
                    file.SeoFilename,
                    false,
                    false);
            }
            lock (s_lock)
            {
                string seoFileName = file.SeoFilename;

                fileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("f{0}_{1}.{2}", file.Id.ToString("00000000"), seoFileName, lastPart) :
                        string.Format("f{0}.{1}", file.Id.ToString("00000000"), lastPart);
                var filePath = GetFileLocalPath(fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllBytes(filePath, fileBinary);
                }

            }
            url = GetFileUrl(fileName, storeLocation);
            return url;
        }
        #endregion

        #region CURD
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileId">标识Id</param>
        /// <returns></returns>
        public File GetFileById(int fileId)
        {
            if (fileId == 0)
                return null;

            var file = _fileRepository.Get(fileId);
            return file;
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        /// <param name="fileBinary">文件的二进制</param>
        /// <param name="mimeType">Mime type</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新文件</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        public File Insert(byte[] fileBinary, string mimeType, string extension, string seoFilename, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);
            if (validateBinary)
                fileBinary = Validate(fileBinary, mimeType);

            var file = new File()
            {
                MimeType = mimeType,
                Extension = extension,
                IsNew = isNew,
                SeoFilename = seoFilename
            };

            int fileId = _fileRepository.InsertAndGetId(file);

            SaveFileInFile(fileId, fileBinary, extension);


            return file;
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="fileId">标识Id</param>
        /// <param name="fileBinary">文件的二进制</param>
        /// <param name="mimeType">Mime type</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新文件</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        public File Update(int fileId, byte[] fileBinary, string mimeType, string extension, string seoFilename, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                fileBinary = Validate(fileBinary, mimeType);

            var file = GetFileById(fileId);
            if (file == null)
                return null;

            file.MimeType = mimeType;
            file.SeoFilename = seoFilename;
            file.IsNew = isNew;
            file.Extension = extension;

            _fileRepository.Update(file);

            SaveFileInFile(file.Id, fileBinary, extension);

            return file;
        }

        /// <summary>
        /// 验证文件二进制
        /// </summary>
        /// <param name="fileBinary">文件的二进制</param>
        /// <param name="mimeType">Mime type</param>
        /// <returns></returns>
        public byte[] Validate(byte[] fileBinary, string mimeType)
        {
            var destStream = new System.IO.MemoryStream();
            destStream = new System.IO.MemoryStream(fileBinary);
            //ImageBuilder.Current.Build(fileBinary, destStream, new ResizeSettings()
            //{
            //    MaxWidth = _mediaConfig.MaximumImageSize,
            //    MaxHeight = _mediaConfig.MaximumImageSize,
            //    Quality = _mediaConfig.DefaultImageQuality
            //});
            return destStream.ToArray();
        }
        #endregion
    }
}
