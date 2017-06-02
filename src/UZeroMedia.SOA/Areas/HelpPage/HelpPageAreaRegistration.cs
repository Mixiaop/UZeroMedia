using System.Web.Http;
using System.Web.Mvc;
using U;
using UZeroMedia.Configuration;

namespace UZeroMedia.SOA.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var mediaSettings = UPrimeEngine.Instance.Resolve<MediaSettings>();

            context.MapRoute(
                "HelpPage_Default",
                (mediaSettings.SOAHelpPath.TrimStart('/').TrimEnd('/').Trim() + "/{action}/{apiId}"),
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}