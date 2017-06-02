using System.Reflection;
using U.UPrimes;
using U.EntityFramework;

namespace UZeroMedia.EntityFramework
{
    [DependsOn(
       typeof(UZeroMediaUPrime),
       typeof(UEntityFrameworkUPrime)
       )]
    public class UZeroMediaEntityFramework : UPrime
    {
        public override void Initialize()
        {
            Engine.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
