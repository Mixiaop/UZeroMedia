using U.Domain.Entities;
using U.EntityFramework.Repositories;

namespace UZeroMedia.EntityFramework.Repositories
{
    public abstract class UZeroMediaRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<UZeroMediaEFDbContext, TEntity, TPrimaryKey>
       where TEntity : class, IEntity<TPrimaryKey>
    {
        protected UZeroMediaRepositoryBase(UZeroMediaEFDbContext dbContext)
            : base(dbContext)
        {

        }
    }

    public abstract class UZeroMediaRepositoryBase<TEntity> : EfRepositoryBase<UZeroMediaEFDbContext, TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected UZeroMediaRepositoryBase(UZeroMediaEFDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
