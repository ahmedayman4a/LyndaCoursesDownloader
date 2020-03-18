using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseContent
{
    public class Course : ICourse
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; }
        public IList<Chapter> Chapters { get; set; }

    }
}
