using System.Collections.Generic;
using Newtonsoft.Json;
using UZeroMedia.Client.Net;

namespace UZeroMedia.Client.Services.Dto
{
    /// <summary>
    /// 批量获取文件输入
    /// </summary>
    public class GetFileUrlsInput : IWebApiClientModel
    {
        public List<int> FileIds { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
