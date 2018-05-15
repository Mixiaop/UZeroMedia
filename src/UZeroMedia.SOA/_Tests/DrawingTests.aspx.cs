using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using U.Utilities.Web;

namespace UZeroMedia.SOA._Tests
{
    public partial class DrawingTests : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(WebHelper.MapPath("/_Tests/test.jpg"));
            Graphics g = Graphics.FromImage(img);

            //Pen p = new Pen(Color.Black, 3);
            Point p0 = new Point(100, 330);
            g.DrawString("hello world", new Font("Microsoft YaHei", 30), new SolidBrush(Color.Red), p0);
            img.Save(WebHelper.MapPath("/_Tests/1.jpg"), System.Drawing.Imaging.ImageFormat.Gif);
        }

        public virtual void PlaceWaterMark(System.Drawing.Image img, Graphics g, int width, int height)
        {
            //if (g.DpiX > this._waterMarkImage.Width)
            g.SmoothingMode = SmoothingMode.AntiAlias;
            byte transparency = (byte)100; //透明度
            int width1 = width;
            int height1 = height;
            Rectangle destRect = new Rectangle(1, 1, 720, 1280);
            float num4 = (float)transparency / 100f;
            if ((double)num4 < 0.0 || (double)num4 > 1.0)
                num4 = 1f;
            float[][] newColorMatrix1 = new float[5][];
            float[][] numArray1 = newColorMatrix1;
            int index1 = 0;
            float[] numArray2 = new float[5];
            numArray2[0] = 1f;
            float[] numArray3 = numArray2;
            numArray1[index1] = numArray3;
            float[][] numArray4 = newColorMatrix1;
            int index2 = 1;
            float[] numArray5 = new float[5];
            numArray5[1] = 1f;
            float[] numArray6 = numArray5;
            numArray4[index2] = numArray6;
            float[][] numArray7 = newColorMatrix1;
            int index3 = 2;
            float[] numArray8 = new float[5];
            numArray8[2] = 1f;
            float[] numArray9 = numArray8;
            numArray7[index3] = numArray9;
            float[][] numArray10 = newColorMatrix1;
            int index4 = 3;
            float[] numArray11 = new float[5];
            numArray11[3] = num4;
            float[] numArray12 = numArray11;
            numArray10[index4] = numArray12;
            float[][] numArray13 = newColorMatrix1;
            int index5 = 4;
            float[] numArray14 = new float[5];
            numArray14[4] = 1f;
            float[] numArray15 = numArray14;
            numArray13[index5] = numArray15;
            ColorMatrix newColorMatrix2 = new ColorMatrix(newColorMatrix1);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(newColorMatrix2);
            imageAttr.SetColorKey(Color.White, Color.White);
            g.DrawImage(img, destRect, 0, 0, width1, height1, GraphicsUnit.Pixel);

        }
    }
}