using System.Collections.Generic;
using Newtonsoft.Json;
using UZeroMedia.Client.Net;

namespace UZeroMedia.Client.Services.Dto
{
    public class GetPictureUrlsInput : IWebApiClientModel
    {
        public List<int> PictureIds { get; set; }
        public int TargetSize { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
