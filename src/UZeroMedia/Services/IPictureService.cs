using U.Application.Services;
using UZeroMedia.Domain;

namespace UZeroMedia.Services
{
    /// <summary>
    /// 图片服务接口
    /// </summary>
    public interface IPictureService : IApplicationService
    {
        /// <summary>
        /// 获取图片Url
        /// </summary>
        /// <param name="pictureId">图片标识Id</param>
        /// <param name="targetSize">尺寸</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetPictureUrl(int pictureId,
            int targetSize = 0,
            string storeLocation = null);

        /// <summary>
        /// 获取图片Url
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="targetSize">尺寸</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetPictureUrl(Picture picture, int targetSize = 0, string storeLocation = null);

        /// <summary>
        /// 获取图片字节流
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        byte[] GetPictureContents(int pictureId, int targetSize = 0);

        /// <summary>
        /// 获取图片原图Url
        /// </summary>
        /// <param name="pictureId">图片标识Id</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetRawPictureUrl(int pictureId,
            string storeLocation = null);

        /// <summary>
        /// 获取图片原图Url
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="storeLocation">存储位置</param>
        /// <returns></returns>
        string GetRawPictureUrl(Picture picture, string storeLocation = null);

        /// <summary>
        /// 获取图片（缩略图）本地路径
        /// </summary>
        /// <param name="picture">图片信息</param>
        /// <param name="targetSize">尺寸</param>
        /// <returns></returns>
        string GetThumbLocalPath(Picture picture, int targetSize = 0);

        /// <summary>
        /// 裁剪图片，返回裁剪后的图片对象 
        /// </summary>
        /// <param name="picture">源图片对象</param>
        /// <param name="x">x轴</param>
        /// <param name="y">y轴</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        Picture CutPicture(Picture oriPicture, int x, int y, int w, int h);

        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="pictureId">图片标识Id</param>
        /// <returns></returns>
        Picture GetPictureById(int pictureId);

        /// <summary>
        /// 插入一张图片
        /// </summary>
        /// <param name="pictureBinary">图片的二进制</param>
        /// <param name="mimeType">图片Mime type</param>
        /// <param name="seoFilename">自定义友好名称</param>
        /// <param name="isNew">是否新图</param>
        /// <param name="validateBinary">是否验证二进制</param>
        /// <returns></returns>
        Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFilename, bool isNew, bool validateBinary = true);

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
        Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType, string seoFilename, bool isNew, bool validateBinary = true);

        /// <summary>
        /// 验证图片二进制
        /// </summary>
        /// <param name="pictureBinary">图片二进制</param>
        /// <param name="mimeType">图片Mime type</param>
        /// <returns></returns>
        byte[] ValidatePicture(byte[] pictureBinary, string mimeType);
    }
}
