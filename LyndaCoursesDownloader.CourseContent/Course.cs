using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseContent
{
    public class Course
    {
        [JsonProperty("Chapters")]
        public List<Chapter> Chapters { get; set; }

        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        public string ExerciseFilesDownloadUrl { get; set; }

        public static Course FromJson(string json)
        {
            Course course = JsonConvert.DeserializeObject<Course>(json, Converter.Settings);
            course.Chapters.ForEach(chapter =>
                chapter.Videos.ForEach(video =>
                {
                    video.ApiUrl = String.Format("https://www.lynda.com/ajax/course/{0}/{1}/play", course.Id, video.Id);
                    video.SubtitlesUrl = String.Format("https://www.lynda.com/ajax/player/transcript?courseId={0}&videoId={1}", course.Id, video.Id);
                })
                );
            return course;
        }
    }
}
