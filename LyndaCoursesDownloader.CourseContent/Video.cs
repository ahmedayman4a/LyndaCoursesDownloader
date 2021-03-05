using Newtonsoft.Json;

namespace LyndaCoursesDownloader.CourseContent
{
    public class Video
    {

        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        public string ApiUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string SubtitlesUrl { get; set; }

        public string Subtitles { get; set; }
    }

}
