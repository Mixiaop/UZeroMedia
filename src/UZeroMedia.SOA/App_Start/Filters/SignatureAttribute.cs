using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using U;
using U.WebApi.Models;
using UZeroMedia.Configuration;

namespace UZeroMedia.SOA.Filters
{
    /// <summary>
    /// 接口验签，可以通过 MediaSettings 来关闭或配置
    /// </summary>
    public class SignatureAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            if (actionContext == null || actionContext.ControllerContext == null)
                return;

            if (actionContext.Request == null)
                return;

            var mediaSettings = UPrimeEngine.Instance.Resolve<MediaSettings>();

            if (mediaSettings.SOASignatureEnabled == 1)
            {
                #region 开启签名
                string sign = ""; //获取的签名
                var signObj = actionContext.Request.Headers.Where(x => x.Key == mediaSettings.SOASignatureKey).FirstOrDefault();
                if (signObj.Value != null)
                    sign = signObj.Value.FirstOrDefault();


                UResponseMessage errorResponse = new UResponseMessage(UResponseStatusCode.SignatureFailed, "Signature Invalid");
                //签名默认1分钟过期
                if (!SignatureHelper.Validation(sign, DateTime.Now, mediaSettings.SOAExpiresTime))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, errorResponse);
                    return;
                }
                #endregion
            }
        }
    }
}