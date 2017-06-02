using U.EntityFramework.Mapping;
using UZeroMedia.Domain;

namespace UZeroMedia.EntityFramework.Mappers
{
    public partial class FileMap  : UEntityTypeConfiguration<File>
    {
        public FileMap() {
            this.ToTable(DbConsts.DbTableName.Files);
            this.HasKey(x => x.Id);
        }
    }
}
