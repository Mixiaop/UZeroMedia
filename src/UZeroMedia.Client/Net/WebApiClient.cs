using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Generic;

namespace UZeroMedia.Client.Net
{
    /// <summary>
    /// 请求服务接口实现，获取远程Http数据
    /// </summary>
    public class WebApiClient : IWebApiClient
    {
        #region Fieds & Ctor
        public WebApiClient()
        {
        }
        #endregion

        #region Utilities
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        private string CreateQueryString(Dictionary<string, string> paramTemp)
        {
            if (paramTemp != null && paramTemp.Count == 0)
                return string.Empty;

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in paramTemp)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            return prestr.Remove(nLen - 1, 1).ToString();
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>拼接完成以后的字符串</returns>
        public string CreateQueryStringUrlencode(Dictionary<string, string> paramTemp, Encoding code)
        {
            if (paramTemp != null && paramTemp.Count == 0)
                return string.Empty;

            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in paramTemp)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, code) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 过滤掉数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="paramTemp"></param>
        /// <returns></returns>
        private Dictionary<string, string> FilterParam(SortedDictionary<string, string> paramTemp)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (paramTemp != null)
            {
                foreach (KeyValuePair<string, string> p in paramTemp)
                {
                    param.Add(p.Key.ToLowerInvariant(), p.Value.ToLowerInvariant());
                }
            }

            return param;
        }

        /// <summary>
        /// 生成请求需要的参数数组
        /// </summary>
        /// <param name="paramTemp">参数集合</param>
        /// <param name="signKey">签名</param>
        /// <returns></returns>
        private Dictionary<string, string> CreateRequestParam(SortedDictionary<string, string> paramTemp)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            //签名
            string sign = string.Empty;
            //过滤签名参数数组
            param = FilterParam(paramTemp);

            return param;
        }

        /// <summary>
        /// 生成要请求的参数数组
        /// </summary>
        /// <param name="paramTemp">请求前的参数数组</param>
        /// <param name="code">字符编码</param>
        /// <param name="signKey">签名对象</param>
        /// <returns>要请求的参数数组字符串</returns>
        private string CreateRequestParaToString(SortedDictionary<string, string> paramTemp, Encoding code)
        {
            //待签名请求参数数组
            Dictionary<string, string> param = new Dictionary<string, string>();
            param = CreateRequestParam(paramTemp);

            //把参数组中所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
            string requestData = CreateQueryStringUrlencode(param, code);

            return requestData;
        }
        #endregion

        #region Html form
        /// <summary>
        /// 创建请求，以表单Html形式构造
        /// </summary>
        /// <param name="url">请求网关url</param>
        /// <param name="method">提交方式：GET、POST</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="btnValue">提交按钮文字</param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public string CreateRequest(string url, string method, SortedDictionary<string, string> param, string btnValue, Signature sign)
        {
            Dictionary<string, string> requestParam = new Dictionary<string, string>();
            requestParam = CreateRequestParam(param);

            StringBuilder sb = new StringBuilder();
            string gateway = url + "?inputcharset=" + sign.Charset;

            sb.AppendFormat("<form id='abpFormSubmit' name='abpFormSubmit' action='{0}' method='{1}'>", gateway, method.ToLowerInvariant().Trim());
            foreach (KeyValuePair<string, string> p in requestParam)
            {
                sb.AppendFormat("<input type='hidden' name='{0}' value='{1}'/>", p.Key, p.Value);
            }
            sb.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", sign.Key, sign.Value);
            sb.AppendFormat("<input type='submit' value='{0}' style='display:none;'></form>", btnValue);
            sb.Append("<script>document.forms['uzysubmit'].submit();</script>");

            return sb.ToString();
        }
        #endregion

        #region Post from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Post请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="signKey">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsPost(string url, SortedDictionary<string, string> param, Signature sign)
        {
            return CreateRequestFromUri(url, param, sign, "POST");
        }

        /// <summary>
        /// 建立请求，以模拟远程Http的POST请求方式构造（参数Uri）- 带上传文件功能
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="fileName">文件名</param>
        /// <param name="data">文件数据</param>
        /// <param name="contentType">文件内容类型</param>
        /// <param name="contentLength">文件长度</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsPost(string url, SortedDictionary<string, string> param, string fileName, byte[] data, string contentType, int contentLength, Signature sign)
        {

            Encoding encoding = Encoding.GetEncoding(sign.Charset);

            //获取签名
            string requestData = CreateRequestParaToString(param, encoding);
            //请求地址
            StringBuilder gateway = new StringBuilder();
            gateway.Append(url);
            gateway.Append("?inputcharset=" + sign.Charset);
            gateway.Append("&" + requestData);

            //设置HttpWebRequest基本信息
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway.ToString());
            request.Method = "POST";

            //设置签名
            request.Headers.Set(sign.Key, sign.Value);

            //设置boundaryValue
            string boundaryValue = DateTime.Now.Ticks.ToString("x");
            string boundary = "--" + boundaryValue;
            request.ContentType = "\r\nmultipart/form-data; boundary=" + boundaryValue;

            //设置KeepAlive
            request.KeepAlive = true;
            //设置请求数据，拼接成字符串
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append(boundary + "\r\nContent-Disposition: form-data; name=\"withhold_file\"; filename=\"");
            sbHtml.Append(fileName);
            sbHtml.Append("\"\r\nContent-Type: " + contentType + "\r\n\r\n");
            string postHeader = sbHtml.ToString();

            //将请求数据字符串类型根据编码格式转换成字节流
            byte[] postHeaderBytes = encoding.GetBytes(postHeader);
            byte[] boundayBytes = Encoding.ASCII.GetBytes("\r\n" + boundary + "--\r\n");
            //设置长度
            long length = postHeaderBytes.Length + contentLength + boundayBytes.Length;
            request.ContentLength = length;

            //请求远程HTTP
            Stream requestStream = request.GetRequestStream();
            Stream myStream;
            try
            {
                //发送数据请求服务器
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                requestStream.Write(data, 0, contentLength);
                requestStream.Write(boundayBytes, 0, boundayBytes.Length);
                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                myStream = HttpWResp.GetResponseStream();
            }
            catch (WebException e)
            {
                return e.ToString();
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
            }

            //读取返回处理的结果
            StreamReader reader = new StreamReader(myStream, encoding);
            StringBuilder responseData = new StringBuilder();

            String line;
            while ((line = reader.ReadLine()) != null)
            {
                responseData.Append(line);
            }
            myStream.Close();
            return responseData.ToString();
        }

        #region body
        /// <summary>
        /// 创建请求，以模拟远程Http的Post请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="signKey">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsPost(string url, IWebApiClientModel requestModel, Signature sign)
        {
            return CreateRequestFromBody(url, requestModel, sign, "POST");
        }
        #endregion
        #endregion

        #region Get from uri
        /// <summary>
        /// 建立请求，以模拟远程Http的Get请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsGet(string url, SortedDictionary<string, string> param, Signature sign)
        {
            Encoding encoding = Encoding.GetEncoding(sign.Charset);

            //待请求参数数组字符串
            string requestData = CreateRequestParaToString(param, encoding);

            //把数组转换成流中所需字节数组类型
            StringBuilder gateway = new StringBuilder();
            gateway.Append(url);
            gateway.Append("?inputcharset=" + sign.Charset);
            gateway.Append("&" + requestData);

            //请求远程HTTP

            string responseMessage = "";
            try
            {
                //设置HttpWebRequest基本信息
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway.ToString());
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                //设置签名
                request.Headers.Set(sign.Key, sign.Value);

                //发送POST数据请求服务器
                StringBuilder responseData = new StringBuilder();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, encoding))
                        {
                            String line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                responseData.Append(line);
                            }
                        }
                    }

                    responseMessage = responseData.ToString();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //_logger.Error(ex.Message, ex);
            }

            return responseMessage;
        }
        #endregion

        #region Put from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Put请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsPut(string url, SortedDictionary<string, string> param, Signature sign)
        {
            return CreateRequestFromUri(url, param, sign, "PUT");
        }

        /// <summary>
        /// 创建请求，以模拟远程Http的Put请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="signKey">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsPut(string url, IWebApiClientModel requestModel, Signature sign)
        {
            return CreateRequestFromBody(url, requestModel, sign, "PUT");
        }
        #endregion

        #region Delete from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Delete请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsDelete(string url, SortedDictionary<string, string> param, Signature sign)
        {
            return CreateRequestFromUri(url, param, sign, "DELETE");
        }

        /// <summary>
        /// 创建请求，以模拟远程Http的Delete请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        public string CreateRequestAsDelete(string url, IWebApiClientModel requestModel, Signature sign)
        {
            return CreateRequestFromBody(url, requestModel, sign, "DELETE");
        }
        #endregion

        #region private from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <param name="method">请求类型,如GET</param>
        /// <returns></returns>
        private string CreateRequestFromUri(string url, SortedDictionary<string, string> param, Signature sign, string method)
        {

            Encoding encoding = Encoding.GetEncoding(sign.Charset);
            //创建请求数据如：name=value&name=value
            string requestData = CreateRequestParaToString(param, encoding);

            StringBuilder gateway = new StringBuilder();
            gateway.Append(url);
            gateway.Append("?inputcharset=" + sign.Charset);
            gateway.Append("&" + requestData);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway.ToString());
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";

            //设置签名
            request.Headers.Set(sign.Key, sign.Value);

            //请求远程HTTP
            string responseMessage = string.Empty;
            try
            {
                //将请求数据字符串类型根据编码格式转换成字节流
                byte[] bytesRequestData = encoding.GetBytes(requestData);
                request.ContentLength = bytesRequestData.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                }

                //发送POST数据请求服务器，获取服务器返回信息
                StringBuilder responseData = new StringBuilder();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, encoding))
                        {
                            String line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                responseData.Append(line);
                            }
                        }
                    }

                    responseMessage = responseData.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //_logger.Error(ex.Message, ex);
            }

            return responseMessage;
        }

        /// <summary>
        /// 创建请求，以模拟远程Http的请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="signKey">签名对象</param>
        /// <param name="method">请求类型，如：GET</param>
        /// <returns></returns>
        private string CreateRequestFromBody(string url, IWebApiClientModel requestModel, Signature sign, string method, string authorization = "")
        {

            Encoding encoding = Encoding.GetEncoding(sign.Charset);

            //待请求参数数组
            string requestData = requestModel.ToJsonString();
            //string requestData = "{\"ConfirmPassword\":\"111111\",\"RegistrationFromType\":2,\"Username\":\"15221755492\",\"Password\":\"111111\",\"GuestUserId\":\"00000000-0000-0000-0000-000000000000\"}";
            StringBuilder gateway = new StringBuilder();
            gateway.Append(url);
            gateway.Append("?inputcharset=" + sign.Charset);

            //设置HttpWebRequest基本信息
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway.ToString());
            request.Method = method;
            request.ContentType = "application/json";
            //设置签名
            request.Headers.Set(sign.Key, sign.Value);

            //请求远程HTTP
            string responseMessage = "";
            try
            {
                //将请求数据字符串类型根据编码格式转换成字节流
                byte[] bytesRequestData = encoding.GetBytes(requestData);
                request.ContentLength = bytesRequestData.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                }

                //发送POST数据请求服务器，获取服务器返回信息
                StringBuilder responseData = new StringBuilder();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, encoding))
                        {
                            String line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                responseData.Append(line);
                            }
                        }
                    }

                    responseMessage = responseData.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //_logger.Error(ex.Message, ex);
            }

            return responseMessage;
        }
        #endregion

        #region Test
        /// <summary>
        /// 以Get方式测试请求，并返回请求的数据
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public string TestRequestAsGet(string gateway)
        {
            try
            {
                var sign = Signature.Create();
                Encoding encoding = Encoding.GetEncoding(sign.Charset);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(gateway);
                request.Method = "GET";

                StringBuilder results = new StringBuilder();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream, encoding))
                        {
                            String line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                results.Append(line);
                            }
                        }
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        results.AppendFormat("StatusCode：{0}", response.StatusCode);
                    }

                    return results.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //_logger.Error(ex.Message, ex);
                return ex.Message;
            }
        }
        #endregion
    }
}
