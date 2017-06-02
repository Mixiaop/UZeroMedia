using U;
using UZeroMedia.Client.Configuration;

namespace UZeroMedia.Client.Net
{
    /// <summary>
    /// 签名密钥，签名结果例：headers 里 { UZeroMedia_Signature: "xxxxxxx"}
    /// 验签密钥公式：通过 SignatureHelper 对时间加密
    /// </summary>
    public class Signature
    {
        private Signature()
        {
            Charset = "utf-8";
            Key = UPrimeEngine.Instance.Resolve<ClientSettings>().SignatureKey;
            Value = SignatureHelper.Create();
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Charset { get; set; }

        public static Signature Create() {
            return new Signature();
        }
    }
}
