using System.Reflection;
using U.UPrimes;
using U.WebApi;
using UZeroMedia.EntityFramework;
using UZeroMedia.Client;

namespace UZeroMedia.SOA
{
    [DependsOn(
        typeof(UWebApiUPrime),
        typeof(UZeroMediaUPrime),
        typeof(UZeroMediaEntityFramework),
        typeof(UZeroMediaClientUPrime)
        )]
    public class UZeroMediaSoaUPrime : UPrime
    {
        public override void Initialize()
        {
            Engine.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}