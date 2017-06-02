using Newtonsoft.Json;
using U;
using U.WebApi.Models;
using UZeroMedia.Client.Net;
using UZeroMedia.Client.Configuration;

namespace UZeroMedia.Client.Services
{
    public abstract class ServiceBase
    {
        protected readonly IWebApiClient webClient;
        protected readonly ClientSettings settings;
        public ServiceBase()
        {
            webClient = new WebApiClient();

            settings = UPrimeEngine.Instance.Resolve<ClientSettings>();
        }

        public string GetUrl(string actionUrl)
        {
            if (!actionUrl.StartsWith("/"))
                actionUrl = "/" + actionUrl;

            return settings.Host.TrimEnd('/') + actionUrl;
        }

        public UResponseMessage<T> ToObject<T>(string response) where T : class
        {
            return JsonConvert.DeserializeObject<UResponseMessage<T>>(response);
        }
    }
}
