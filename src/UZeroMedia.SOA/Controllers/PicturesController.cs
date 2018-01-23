using System;
using System.Collections.Generic;
using System.Web.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using U;
using U.WebApi.Models;
using UZeroMedia.Configuration;
using UZeroMedia.Domain;
using UZeroMedia.Services;
using UZeroMedia.Services.Dto;
using UZeroMedia.SOA.Models;


namespace UZeroMedia.SOA.Controllers
{
    /// <summary>
    /// 通用图片服务，负责图片：存储、裁剪、获取等操作
    /// </summary>
    [RoutePrefix("pictures")]
    public class PicturesController : ApiControllerBase
    {
        #region Fields & Ctor
        private readonly IPictureService _pictureService;
        private readonly WaterMarkService _waterMarkService;
        private MediaSettings _mediaSettings;
        public PicturesController()
        {
            _pictureService = UPrimeEngine.Instance.Resolve<IPictureService>();
            _waterMarkService = UPrimeEngine.Instance.Resolve<WaterMarkService>();
            _mediaSettings = UPrimeEngine.Instance.Resolve<MediaSettings>();
        }
        #endregion

        #region Query
        /// <summary>
        /// 根据图片Id获取图片路径 
        /// </summary>
        /// <param name="pictureId">图片Id</param>
        /// <param name="targetSize">目标尺寸</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getUrl")]
        public UResponseMessage<string> GetPictureUrl(int pictureId, int targetSize = 0)
        {
            UResponseMessage<string> response = new UResponseMessage<string>();
            response.Results = _pictureService.GetPictureUrl(pictureId, targetSize, _mediaSettings.Location);
            return response;
        }

        /// <summary>
        /// 通过图片Id批量获取图片地址
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        [Route("getUrlList")]
        public UResponseMessage<IList<PictureDto>> GetPictureUrls(GetPictureUrlsInput input)
        {
            UResponseMessage<IList<PictureDto>> response = new UResponseMessage<IList<PictureDto>>();
            response.Results = new List<PictureDto>();
            if (input == null || input.PictureIds == null || input.PictureIds.Length == 0)
            {
                response.SetMessage(UResponseStatusCode.Error, "图片数组不能为空");
                return response;
            }
            foreach (var pid in input.PictureIds)
            {
                var dto = new PictureDto();
                dto.PictureId = pid;
                dto.PictureUrl = _pictureService.GetPictureUrl(pid, input.TargetSize, _mediaSettings.Location);
                response.Results.Add(dto);
            }

            return response;
        }

        /// <summary>
        /// 根据图片Id获取图片路径（带水印）
        /// </summary>
        /// <param name="pictureId">图片Id</param>
        /// <param name="targetSize">目标尺寸</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getUrlWithM")]
        public UResponseMessage<string> GetPictureUrlByWaterMark(int pictureId, int targetSize = 0)
        {
            UResponseMessage<string> response = new UResponseMessage<string>();
            response.Results = _waterMarkService.GetPictureUrl(pictureId, targetSize, _mediaSettings.Location);
            return response;
        }

        /// <summary>
        /// 根据图片Id获取图片路径（带水印）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getUrlListWithM")]
        public UResponseMessage<IList<PictureDto>> GetPictureUrlsByWaterMark(GetPictureUrlsInput input)
        {

            UResponseMessage<IList<PictureDto>> response = new UResponseMessage<IList<PictureDto>>();
            response.Results = new List<PictureDto>();
            if (input == null || input.PictureIds == null || input.PictureIds.Length == 0)
            {
                response.SetMessage(UResponseStatusCode.Error, "图片数组不能为空");
                return response;
            }

            foreach (var pid in input.PictureIds)
            {
                var dto = new PictureDto();
                dto.PictureId = pid;
                dto.PictureUrl = _waterMarkService.GetPictureUrl(pid, input.TargetSize, _mediaSettings.Location);
                response.Results.Add(dto);
            }

            return response;
        }
        #endregion

        #region Upload & Cut
        /// <summary>
        /// 上传图片（成功则返回地址，否则返回空字符串）
        /// </summary>
        /// <param name="seoFileName">自定义文件名称</param>
        /// <param name="targetSize">目标尺寸</param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public async Task<UResponseMessage<PictureDto>> UploadPicture(string seoFileName = "", int targetSize = 0)
        {
            UResponseMessage<PictureDto> response = new UResponseMessage<PictureDto>();
            var error = new UResponseMessage<PictureDto>();
            error.SetMessage(UResponseStatusCode.Error, "不是有效的 MimeMultipartContent 类型");

            try
            {
                //检查类型
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return error;
                }


                //创建图片流支持
                var provider = new MultipartMemoryStreamProvider();

                //读取到stream
                await Request.Content.ReadAsMultipartAsync(provider); ;

                if (provider.Contents.Count == 1)
                {
                    //获取图片字节数组
                    var file = provider.Contents[0];
                    
                    var dataTask = file.ReadAsByteArrayAsync();
                    byte[] data = dataTask.Result;

                    string mimeType = "";
                    if (file.Headers.ContentType != null) {
                        mimeType = file.Headers.ContentType.MediaType;
                    } else {
                        mimeType = "image/jpeg";
                    }

                    //保存到应用本地
                    var picture = _pictureService.InsertPicture(data, mimeType,
                        seoFileName, true);

                    if (picture != null)
                    {
                        //返回目标尺寸
                        string thumbUrl = _pictureService.GetPictureUrl(picture.Id, targetSize, _mediaSettings.Location);
                        //if (thumbUrl.IndexOf("http") == -1)
                        //    thumbUrl = _mediaSettings.Location.TrimEnd('/') + thumbUrl;

                        PictureDto model = new PictureDto();
                        model.PictureUrl = thumbUrl;
                        model.PictureId = picture.Id;
                        if (!string.IsNullOrEmpty(_mediaSettings.ShowLocation) && !string.IsNullOrEmpty(_mediaSettings.Location))
                        {
                            if (!string.IsNullOrEmpty(model.PictureUrl))
                                model.ShowPictureUrl = model.PictureUrl.Replace(_mediaSettings.Location, _mediaSettings.ShowLocation).Trim();

                            if (!string.IsNullOrEmpty(model.ThumbUrl))
                                model.ShowThumbUrl = model.ThumbUrl.Replace(_mediaSettings.Location, _mediaSettings.ShowLocation).Trim();

                            if (!string.IsNullOrEmpty(model.SquareUrl))
                                model.ShowSquareUrl = model.SquareUrl.Replace(_mediaSettings.Location, _mediaSettings.ShowLocation).Trim();
                        }
                        response.Results = model;
                        return response;
                    }
                    else
                    {
                        return error;
                    }
                }
                else
                {
                    return error;
                }
            }
            catch (Exception ex)
            {
                error.Message = ex.Message;
                return error;
            }
        }
       

        /// <summary>
        /// 图片裁剪处理，返回处理后的标识与地址
        /// </summary>
        /// <param name="pictureId">图片Id</param>
        /// <param name="x">x轴</param>
        /// <param name="y">y轴</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cut")]
        public UResponseMessage<PictureDto> CutPicture(int pictureId, int x, int y, int w, int h)
        {
            UResponseMessage<PictureDto> response = new UResponseMessage<PictureDto>();

            //需要裁剪的图片
            Picture oriPicture = _pictureService.GetPictureById(pictureId);
            if (oriPicture == null)
            {
                response.Code = UResponseStatusCode.RequestParameterIsWrong;
                response.Message = "picture no exists";
                return response;
            }

            //裁剪图片
            var picture = _pictureService.CutPicture(oriPicture, x, y, w, h);
            string pictureUrl = _pictureService.GetPictureUrl(picture.Id);

            PictureDto model = new PictureDto();
            model.PictureId = picture.Id;
            model.PictureUrl = pictureUrl;
            response.Results = model;

            return response;
        }
        #endregion

        #region ShowPicture
        /// <summary>
        /// 显示图片流
        /// </summary>
        /// <param name="pictureId"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("showpic")]
        public HttpResponseMessage ShowPic(int pictureId, int targetSize = 0)
        {
            //从图片中读取byte
            var imgByte = _pictureService.GetPictureContents(pictureId, targetSize);
            //从图片中读取流
            var imgStream = new MemoryStream(imgByte);

            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(imgByte)
                //或者
                //Content = new StreamContent(stream)
            };

            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            resp.Headers.CacheControl = new CacheControlHeaderValue();
            resp.Headers.CacheControl.MaxAge = new TimeSpan(0, 10, 0);  // 10 min. or 600 sec.
            resp.Headers.CacheControl.Public = true;
            imgStream.Dispose();

            return resp;

        }
        #endregion
    }
}