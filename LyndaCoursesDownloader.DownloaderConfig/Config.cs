using LyndaCoursesDownloader.CourseContent;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace LyndaCoursesDownloader.DownloaderConfig
{
    public class Config
    {
        [JsonProperty("EncryptedAuthenticationToken")]
        private string AuthenticationTokenEncrypted { get; set; }

        [JsonIgnore]
        public string AuthenticationToken
        {
            get
            {
                return DecryptToken(AuthenticationTokenEncrypted);
            }
            set
            {
                AuthenticationTokenEncrypted = EncryptToken(value);
            }
        }
        [JsonProperty("CourseDirectory")]
        public DirectoryInfo CourseDirectory { get; set; }

        [JsonProperty("Browser")]
        public Browser Browser { get; set; }

        [JsonProperty("Quality")]
        public Quality Quality { get; set; }


        private string EncryptToken(string token)
        {
            byte[] b = ASCIIEncoding.ASCII.GetBytes(token);
            string encryptedToken = Convert.ToBase64String(b);
            return encryptedToken;
        }

        public string DecryptToken(string encryptedToken)
        {
            byte[] b;
            string decryptedToken;
            try
            {
                b = Convert.FromBase64String(encryptedToken);
                decryptedToken = ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException)
            {
                decryptedToken = "";
            }
            return decryptedToken;
        }

        public static Config FromJson(string json) => JsonConvert.DeserializeObject<Config>(json, Converter.Settings);

    }
    public static class SerializeConfig
    {
        public static string ToJson(this Config self) => JsonConvert.SerializeObject(self, Formatting.Indented, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                BrowserConverter.Singleton,
                QualityConverter.Singleton,
                DirectoryInfoConverter.Singleton
            },
        };
    }
    
}
