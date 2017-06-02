using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using U;
using UZeroMedia.Domain;
using UZeroMedia.Configuration;
using UZeroMedia.Client.Services;
namespace UZeroMedia.SOA._Tests
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Write(SignatureHelper.GetEncodeSign());
            //var mediaSettings = UPrimeEngine.Instance.Resolve<DatabaseSettings>();
            //var pictureService = UPrimeEngine.Instance.Resolve<UZeroMedia.Domain.IPictureRepository>();
            //pictureService.Insert(new Domain.Picture());
            btnUpload.Click += BtnUpload_Click;
            btnUpload2.Click += BtnUpload2_Click;
            PictureClientService pictureService = UPrimeEngine.Instance.Resolve<PictureClientService>();
            //var result = pictureService.Upload(fuUpload.PostedFile);
            Response.Write(JsonConvert.SerializeObject(pictureService.GetUrl(12798)));

            //IPictureRepository pictureRepository = UPrimeEngine.Instance.Resolve<IPictureRepository>();
            //var pic = pictureRepository.GetAll().Where(x => x.Id == 12798).FirstOrDefault();
            //Response.Write(JsonConvert.SerializeObject(pic));
        }

        private void BtnUpload2_Click(object sender, EventArgs e)
        {
            PictureClientService pictureService = UPrimeEngine.Instance.Resolve<PictureClientService>();

            string imgUrl = "http://photocdn.sohu.com/20161107/Img472457135.jpeg";
            imgUrl = "http://img.blog.csdn.net/20161107093938779";
            WebClient client = new WebClient();
            byte[] buff = client.DownloadData(imgUrl);

            string filename = Path.GetFileName(imgUrl);
            var result = pictureService.Upload(filename, buff);
            Response.Write(JsonConvert.SerializeObject(result));
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            PictureClientService pictureService = UPrimeEngine.Instance.Resolve<PictureClientService>();
            var result = pictureService.Upload(fuUpload.PostedFile);
            Response.Write(JsonConvert.SerializeObject(result));
        }
    }
}