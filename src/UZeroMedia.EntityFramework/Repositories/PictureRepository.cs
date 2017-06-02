using UZeroMedia.Domain;

namespace UZeroMedia.EntityFramework.Repositories
{
    public class PictureRepository : UZeroMediaRepositoryBase<Picture>, IPictureRepository
    {
        public PictureRepository(UZeroMediaEFDbContext dbContext) : base(dbContext) { }
    }
}
