﻿using System.Reflection;
using U.UPrimes;

namespace UZeroMedia.Client
{
    public class UZeroMediaClientUPrime : UPrime
    {
        public override void Initialize()
        {
            Engine.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
