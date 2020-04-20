using System;
using LyndaCoursesDownloader.CourseContent;
using Newtonsoft.Json;

namespace LyndaCoursesDownloader.DownloaderConfig
{
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
}
