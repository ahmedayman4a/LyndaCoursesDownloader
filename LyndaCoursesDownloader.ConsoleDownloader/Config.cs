using LyndaCoursesDownloader.CourseExtractor;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace LyndaCoursesDownloader.ConsoleDownloader
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

    internal class BrowserConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Browser) || t == typeof(Browser?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Chrome":
                    return Browser.Chrome;
                case "Firefox":
                    return Browser.Firefox;
            }
            throw new Exception("Cannot unmarshal type Browser");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Browser)untypedValue;
            switch (value)
            {
                case Browser.Chrome:
                    serializer.Serialize(writer, "Chrome");
                    return;
                case Browser.Firefox:
                    serializer.Serialize(writer, "Firefox");
                    return;
            }
            throw new Exception("Cannot marshal type Browser");
        }

        public static readonly BrowserConverter Singleton = new BrowserConverter();
    }

    internal class QualityConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Quality) || t == typeof(Quality?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "High":
                    return Quality.High;
                case "Medium":
                    return Quality.Medium;
                case "Low":
                    return Quality.Low;
            }
            throw new Exception("Cannot unmarshal type Quality");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Quality)untypedValue;
            switch (value)
            {
                case Quality.High:
                    serializer.Serialize(writer, "High");
                    return;
                case Quality.Medium:
                    serializer.Serialize(writer, "Medium");
                    return;
                case Quality.Low:
                    serializer.Serialize(writer, "Low");
                    return;
            }
            throw new Exception("Cannot marshal type Quality");
        }

        public static readonly QualityConverter Singleton = new QualityConverter();

    }

    internal class DirectoryInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DirectoryInfo);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            try
            {
                return new DirectoryInfo(value);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot unmarshal type DirectoryInfo", ex);
            }


        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DirectoryInfo)untypedValue;
            try
            {
                serializer.Serialize(writer, value.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot marshal type DirectoryInfo", ex);
            }


        }

        public static readonly DirectoryInfoConverter Singleton = new DirectoryInfoConverter();

    }
}
