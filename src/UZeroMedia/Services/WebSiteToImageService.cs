using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace UZeroMedia.Services
{
    public class WebSiteToImageService : IWebSiteToImageService
    {
        private Bitmap _image;
        private string _webSiteUrl;
        public WebSiteToImageService()
        {

        }

        #region Utilities
        private void DoGenerate()
        {
            var browser = new WebBrowser { ScrollBarsEnabled = false };
            browser.Navigate(_webSiteUrl);
            browser.DocumentCompleted += WebBrowser_DocumentCompleted;

            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                //  Application.DoEvents();
            }

            browser.Dispose();
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Capture 
            var browser = (WebBrowser)sender;
            browser.ClientSize = new Size(browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Bottom);
            browser.ScrollBarsEnabled = false;
            _image = new Bitmap(browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Bottom);
            browser.BringToFront();
            browser.DrawToBitmap(_image, browser.Bounds);
        }
        #endregion

        /// <summary>
        /// 生成并返回一张Bitmap图
        /// </summary>
        /// <param name="webSiteUrl">网站url</param>
        /// <returns></returns>
        public Bitmap Generate(string webSiteUrl)
        {
            _webSiteUrl = webSiteUrl;
            //开始线程
            var thread = new Thread(DoGenerate);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return _image;
        }
    }
}
