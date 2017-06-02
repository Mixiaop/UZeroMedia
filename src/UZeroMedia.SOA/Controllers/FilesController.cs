using System;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using U;
using U.WebApi.Models;
using UZeroMedia.Configuration;
using UZeroMedia.Services;
using UZeroMedia.Services.Dto;

namespace UZeroMedia.SOA.Controllers
{
    /// <summary>
    /// 通用文件服务，负责文件：存储、获取等操作
    /// </summary>
    [RoutePrefix("Files")]    
    public class FilesController : ApiControllerBase
    {
        private readonly IFileService _fileService;
        private MediaSettings _mediaSettings;
        public FilesController()
        {
            _fileService = UPrimeEngine.Instance.Resolve<IFileService>();
            _mediaSettings = UPrimeEngine.Instance.Resolve<MediaSettings>();
        }

        /// <summary>
        /// 根据文件Id获取文件路径 
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUrl")]
        public UResponseMessage<string> GetFileUrl(int fileId) {
            UResponseMessage<string> response = new UResponseMessage<string>();
            response.Results = _fileService.GetFileUrl(fileId, _mediaSettings.Location);
            return response;
        }

        /// <summary>
        /// 通过文件Id批量获取文件路径 
        /// </summary>
        /// <param name="fileIds"></param>
        [HttpPost]
        [Route("getUrlList")]
        public UResponseMessage<IList<FileDto>> GetPictureUrls(List<int> fileIds)
        {
            UResponseMessage<IList<FileDto>> response = new UResponseMessage<IList<FileDto>>();
            if (fileIds == null || fileIds.Count == 0)
            {
                throw new Exception("文件数组不能为空");
            }
            response.Results = new List<FileDto>();
            foreach (var fid in fileIds)
            {
                var dto = new FileDto();
                dto.FileId = fid;
                dto.FileUrl = _fileService.GetFileUrl(fid, _mediaSettings.Location);
                response.Results.Add(dto);
            }

            return response;
        }

        /// <summary>
        /// 上传文件（成功则返回地址，否则返回空字符串）
        /// </summary>
        /// <param name="seoFileName">自定义文件名称</param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public async Task<UResponseMessage<FileDto>> UploadFile(string seoFileName = "")
        {
            if (seoFileName == null)
                seoFileName = "";
            UResponseMessage<FileDto> response = new UResponseMessage<FileDto>();
            var error = new UResponseMessage<FileDto>();
            error.SetMessage(UResponseStatusCode.Error, "不是有效的 MimeMultipartContent 类型");

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
                string extension = System.IO.Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\"", "")).Replace(".", "");

                //保存到应用本地
                var youfile = _fileService.Insert(data, file.Headers.ContentType.MediaType, extension,
                    seoFileName, true);

                if (youfile != null)
                {
                    //返回目标尺寸
                    string fileUrl = _fileService.GetFileUrl(youfile.Id, _mediaSettings.Location);


                    FileDto model = new FileDto();
                    model.FileUrl = fileUrl;
                    model.FileId = youfile.Id;
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
    }
}