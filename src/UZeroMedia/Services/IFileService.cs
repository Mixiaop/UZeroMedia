using U.Application.Services;
using UZeroMedia.Domain;

namespace UZeroMedia.Services
{
    public interface IFileService : IApplicationService
    {
        /// <summary>
        /// 获取文件Url
        /// </summary>
        /// <param name="fileId">标识Id</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetFileUrl(int fileId,
            string storeLocation = null);


        /// <summary>
        /// 获取文件Url
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetFileUrl(File file, string storeLocation = null);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileId">标识Id</param>
        /// <returns></returns>
        File GetFileById(int fileId);

        /// <summary>
        /// 插入一张文件
        /// </summary>
        /// <param name="fileBinary">文件的二进制</param>
        /// <param name="mimeType">Mime type</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新文件</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        File Insert(byte[] fileBinary, string mimeType, string extension, string seoFilename, bool isNew, bool validateBinary = true);

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
        File Update(int fileId, byte[] fileBinary, string mimeType, string extension, string seoFilename, bool isNew, bool validateBinary = true);

        /// <summary>
        /// 验证文件二进制
        /// </summary>
        /// <param name="fileBinary">文件的二进制</param>
        /// <param name="mimeType">Mime type</param>
        /// <returns></returns>
        byte[] Validate(byte[] fileBinary, string mimeType);
    }
}
