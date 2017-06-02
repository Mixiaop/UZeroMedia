using U.Settings;

namespace UZeroMedia.Configuration
{
    [USettingsPathArribute("DatabaseSettings.json", "/Config/UZeroMedia")]
    public class DatabaseSettings : USettings<DatabaseSettings>
    {
        public string SqlConnectionString { get; set; }
        public string ReadonlySqlConnectionString { get; set; }
    }
}
