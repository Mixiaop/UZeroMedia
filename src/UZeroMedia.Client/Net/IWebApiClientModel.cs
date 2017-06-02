namespace UZeroMedia.Client.Net
{
    /// <summary>
    /// Web请求模型接口，所有使用IAbpWebRequest POST from body形式的都应用继承此接口
    /// </summary>
    public interface IWebApiClientModel
    {
        /// <summary>
        /// to json string 
        /// </summary>
        /// <returns></returns>
        string ToJsonString();
    }
}
