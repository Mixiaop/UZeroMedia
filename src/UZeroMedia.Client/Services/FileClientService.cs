using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using U.WebApi.Models;
using U.Application.Services;
using UZeroMedia.Client.Net;
using UZeroMedia.Client.Services.Dto;

namespace UZeroMedia.Client.Services
{
    public class FileClientService : ServiceBase, IApplicationService
    {
        public FileClientService() : base() { }

        #region Upload
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public UResponseMessage<FileDto> Upload(HttpPostedFile postedFile)
        {
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            //parms.Add("seoFileName", "");
            var message = webClient.CreateRequestAsPost(
                GetUrl("/files/upload"),
                parms,
                postedFile.FileName,
                postedFile.GetBits(),
                postedFile.ContentType,
                postedFile.ContentLength, Signature.Create());
            return JsonConvert.DeserializeObject<UResponseMessage<FileDto>>(message);
        }

        public UResponseMessage<FileDto> Upload(string fileName, byte[] steam, string contentType, int contentLength, SortedDictionary<string, string> parms = null)
        {
            if (parms == null)
                parms = new SortedDictionary<string, string>();
            var message = webClient.CreateRequestAsPost(
                GetUrl("/files/upload"),
                parms,
                fileName,
                steam,
                contentType,
                contentLength, Signature.Create());
            return JsonConvert.DeserializeObject<UResponseMessage<FileDto>>(message);
        }
        #endregion

        #region Gets
        /// <summary>
        /// 根据Id获取文件路径
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<string> GetUrl(int fileId)
        {
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("fileId", fileId.ToString());

            var message = webClient.CreateRequestAsGet(
                GetUrl("/files/getUrl"),
                parms, Signature.Create());
            return JsonConvert.DeserializeObject<UResponseMessage<string>>(message);
        }
        /// <summary>
        /// 根据Id数组获取文件路径
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<FileDto> GetUrlList(GetFileUrlsInput input)
        {
            var message = webClient.CreateRequestAsPost(
                GetUrl("/files/getUrlList"),
                input, Signature.Create());
            return JsonConvert.DeserializeObject<UResponseMessage<FileDto>>(message);
        }
        #endregion
    }
}
