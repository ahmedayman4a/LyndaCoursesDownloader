using Newtonsoft.Json;
using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseContent
{
    public class Chapter
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Videos")]
        public List<Video> Videos { get; set; }
    }
}
