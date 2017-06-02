using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using U.WebApi.Models;
using U.Application.Services;
using UZeroMedia.Client.Net;
using UZeroMedia.Client.Services.Dto;

namespace UZeroMedia.Client.Services
{
    public class PictureClientService : ServiceBase, IApplicationService
    {
        public PictureClientService() : base() { }

        #region Upload / Cut
        /// <summary>
        /// 上传本地图片到图片应用
        /// </summary>
        /// <param name="path">图片绝对路径</param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<PictureDto> Upload(string path, int targetSize = 0)
        {
            var response = new UResponseMessage<PictureDto>();
            if (!string.IsNullOrEmpty(path))
            {
                Stream stream = new FileStream(path, FileMode.OpenOrCreate);
                if (stream != null)
                {
                    var contentType = MimeMapping.GetMimeMapping(path);
                    var fileName = Path.GetFileName(path);
                    int contentLength = (int)stream.Length;
                    byte[] imageArr = new byte[contentLength];
                    stream.Read(imageArr, 0, contentLength);
                    stream.Close();

                    response = Upload(fileName, imageArr, contentType, contentLength, targetSize);
                    return response;
                }
            }

            response.SetMessage(UResponseStatusCode.Error, "上传失败");

            return response;

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<PictureDto> Upload(HttpPostedFile postedFile, int targetSize = 0)
        {

            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            string contentType = postedFile.ContentType;
            string extention = System.IO.Path.GetExtension(postedFile.FileName).Replace(".", "").ToLower();
            if (string.Equals(extention, "jpe") || string.Equals(extention, "jpeg") || string.Equals(extention, "jpg"))
            {
                extention = "jpeg";
            }
            if (string.Equals(extention, "tif") || string.Equals(extention, "tiff"))
            {
                extention = "tiff";
            }

            if (!postedFile.ContentType.Contains("image"))
            {
                contentType = "image/" + extention;
            }

            parms.Add("targetSize", targetSize.ToString());

            var message = webClient.CreateRequestAsPost(
                GetUrl("/pictures/upload"),
                parms,
                postedFile.FileName,
                postedFile.GetBits(),
                contentType,
                postedFile.ContentLength, Signature.Create());

            return ToObject<PictureDto>(message);

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="imgArr">图片二进制流数组</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="contentLength">内容长度</param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<PictureDto> Upload(string fileName, byte[] imgArr, string contentType = "", int contentLength = 0, int targetSize = 0)
        {

            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            if (contentType.IsNullOrEmpty()) {
                string extention = System.IO.Path.GetExtension(fileName).Replace(".", "").ToLower();
                if (string.Equals(extention, "jpe") || string.Equals(extention, "jpeg") || string.Equals(extention, "jpg"))
                {
                    extention = "jpeg";
                }
                if (string.Equals(extention, "tif") || string.Equals(extention, "tiff"))
                {
                    extention = "tiff";
                }
                contentType = "image/" + extention;
            }

            if (contentLength <= 0)
                contentLength = imgArr.Length;

            parms.Add("targetSize", targetSize.ToString());

            var message = webClient.CreateRequestAsPost(
                GetUrl("/pictures/upload"),
                parms,
                fileName,
                imgArr,
                contentType,
                contentLength, Signature.Create());

            return ToObject<PictureDto>(message);

        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public UResponseMessage<PictureDto> Cut(int pictureId, int x, int y, int w, int h)
        {
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("pictureId", pictureId.ToString());
            parms.Add("x", x.ToString());
            parms.Add("y", y.ToString());
            parms.Add("w", w.ToString());
            parms.Add("h", h.ToString());

            var message = webClient.CreateRequestAsGet(
                GetUrl("/pictures/cut"),
                parms, Signature.Create());

            return ToObject<PictureDto>(message);
        }
        #endregion

        #region Gets
        /// <summary>
        /// 根据图片Id获取图片路径
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<string> GetUrl(int pictureId, int targetSize = 0)
        {
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("pictureId", pictureId.ToString());
            parms.Add("targetSize", targetSize.ToString());

            var message = webClient.CreateRequestAsGet(
                GetUrl("/pictures/getUrl"),
                parms, Signature.Create());

            return ToObject<string>(message);
        }

        public string GetPictureUrl(int pictureId, int targetSize = 0)
        {
            var response = GetUrl(pictureId, targetSize);
            var returnUrl = "";
            if (response != null && response.Results != null)
            {
                returnUrl = response.Results;
            }

            return returnUrl;
        }

        

        /// <summary>
        /// 根据图片Id数组获取图片路径
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public UResponseMessage<IList<PictureDto>> GetUrlList(GetPictureUrlsInput input)
        {
            string message = webClient.CreateRequestAsPost(
                GetUrl("pictures/getUrlList"),
                input, Signature.Create());
            return JsonConvert.DeserializeObject<UResponseMessage<IList<PictureDto>>>(message);
        }
        #endregion
    }
}
