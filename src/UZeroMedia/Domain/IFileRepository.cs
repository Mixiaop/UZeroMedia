using U.Domain.Repositories;

namespace UZeroMedia.Domain
{
    /// <summary>
    /// 文件仓储接口
    /// </summary>
    public interface IFileRepository : U.Domain.Repositories.IRepository<File, int>
    {
    }
}
