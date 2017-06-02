using System.Reflection;
using U;
using U.UPrimes;
using U.AutoMapper;
using UZeroMedia.Services.Mappers;

namespace UZeroMedia
{
    [DependsOn(
       typeof(ULeadershipUPrime),
       typeof(UAutoMapperUPrime)
       )]
    public class UZeroMediaUPrime : UPrime
    {
        public override void Initialize()
        {
            Engine.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            CustomDtoMapper.CreateMappings();
        }
    }
}
