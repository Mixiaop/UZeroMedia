using UZeroMedia.Domain;
namespace UZeroMedia.EntityFramework.Repositories
{
    public class FileRepository : UZeroMediaRepositoryBase<File>, IFileRepository
    {
        public FileRepository(UZeroMediaEFDbContext dbContext) : base(dbContext) { }
    }
}
