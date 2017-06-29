using System;
using System.Data.Entity;
using System.Reflection;
using System.Linq;
using U;
using U.EntityFramework;
using U.EntityFramework.Mapping;
using UZeroMedia.Configuration;
using UZeroMedia.Domain;

namespace UZeroMedia.EntityFramework
{
    public class UZeroMediaEFDbContext : UDbContext
    {
        public virtual IDbSet<File> Files { get; set; }
        public virtual IDbSet<Picture> Pictures { get; set; }

        public UZeroMediaEFDbContext(string nameOrConnectionString)
            : base(UPrimeEngine.Instance.Resolve<DatabaseSettings>().SqlConnectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(UEntityTypeConfiguration<>));
            if (typesToRegister != null && typesToRegister.Count() > 0)
            {
                foreach (var type in typesToRegister)
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.Configurations.Add(configurationInstance);
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
