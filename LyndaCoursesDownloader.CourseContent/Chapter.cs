using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseContent
{
    public class Chapter : ICourse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Video> Videos { get; set; }

        public override bool Equals(object obj)
        {
            var chapter = obj as Chapter;
            if (chapter == null)
            {
                return false;
            }
            return this.Id == chapter.Id;
        }

        public override int GetHashCode() => 19 * 31 + this.Id;
    }
}
