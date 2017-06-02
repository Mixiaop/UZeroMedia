using U.Settings;

namespace UZeroMedia.Client.Configuration
{
    [USettingsPathArribute("ClientSettings.json", "/Config/UZeroMedia")]
    public class ClientSettings : USettings<ClientSettings>
    {
        public string Host { get; set; }
        public string SignatureKey { get; set; }
    }
}
