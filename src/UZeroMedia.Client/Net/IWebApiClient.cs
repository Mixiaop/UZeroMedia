using System.Collections.Generic;

namespace UZeroMedia.Client.Net
{
    /// <summary>
    /// 请求服务接口，获取远程Http数据
    /// </summary>
    public interface IWebApiClient
    {
        #region Html from uri
        /// <summary>
        /// 创建请求，以表单Html形式构造
        /// </summary>
        /// <param name="url">请求网关url</param>
        /// <param name="method">提交方式：GET、POST</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="btnValue">提交按钮文字</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequest(string url, string method, SortedDictionary<string, string> param, string btnValue, Signature sign);
        #endregion

        #region Post from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Post请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsPost(string url, SortedDictionary<string, string> param, Signature sign);

        /// <summary>
        /// 建立请求，以模拟远程Http的POST请求方式构造（参数Uri）- 上传文件
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="fileName">文件名</param>
        /// <param name="data">文件数据</param>
        /// <param name="contentType">文件内容类型</param>
        /// <param name="contentLength">文件长度</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsPost(string url, SortedDictionary<string, string> param, string fileName, byte[] data, string contentType, int contentLength, Signature sign);

        /// <summary>
        /// 创建请求，以模拟远程Http的Post请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsPost(string url, IWebApiClientModel requestModel, Signature sign);
        #endregion

        #region Get from uri
        /// <summary>
        /// 建立请求，以模拟远程Http的Get请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsGet(string url, SortedDictionary<string, string> param, Signature sign);
        #endregion

        #region Put from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Put请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsPut(string url, SortedDictionary<string, string> param, Signature sign);

        /// <summary>
        /// 创建请求，以模拟远程Http的Put请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsPut(string url, IWebApiClientModel requestModel, Signature sign);
        #endregion

        #region Delete from uri / body
        /// <summary>
        /// 创建请求，以模拟远程Http的Delete请求方式构造（参数Uri）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="param">请求参数数组</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsDelete(string url, SortedDictionary<string, string> param, Signature sign);

        /// <summary>
        /// 创建请求，以模拟远程Http的Delete请求方式构造（参数Body）
        /// </summary>
        /// <param name="url">请求网关</param>
        /// <param name="requestModel">请求的对象模型</param>
        /// <param name="sign">签名对象</param>
        /// <returns></returns>
        string CreateRequestAsDelete(string url, IWebApiClientModel requestModel, Signature sign);
        #endregion
    }
}
