using U.EntityFramework.Mapping;
using UZeroMedia.Domain;

namespace UZeroMedia.EntityFramework.Mappers
{
    public partial class PictureMap : UEntityTypeConfiguration<Picture>
    {
        public PictureMap()
        {
            this.ToTable(DbConsts.DbTableName.Pictures);
            this.HasKey(x => x.Id);
        }
    }
}
