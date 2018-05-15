using U.Domain.Repositories;

namespace UZeroMedia.Domain
{
    /// <summary>
    /// 图片仓储服务接口
    /// </summary>
    public interface IPictureRepository : U.Domain.Repositories.IRepository<Picture, int>
    {
    }
}
