using System.Drawing;
using U.Dependency;


namespace UZeroMedia.Services
{
    /// <summary>
    /// 网站转图片服务，如：将www.youzy.cn保存成一张图片
    /// </summary>
    public interface IWebSiteToImageService : U.Dependency.ITransientDependency
    {
        /// <summary>
        /// 生成并返回一张Bitmap图
        /// </summary>
        /// <param name="webSiteUrl">网站url</param>
        /// <returns></returns>
        Bitmap Generate(string webSiteUrl);
    }
}
