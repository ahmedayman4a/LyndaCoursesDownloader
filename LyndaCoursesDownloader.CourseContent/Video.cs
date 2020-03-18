namespace LyndaCoursesDownloader.CourseContent
{
    public class Video : ICourse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public string VideoDownloadUrl { get; set; }
        public string CaptionText { get; set; }
        public Chapter Chapter { get; set; }

        public CurrentStatus CurrentVideoStatus { get; set; } = CurrentStatus.Ready;

        public override bool Equals(object obj)
        {
            var video = obj as Video;
            if (video == null)
            {
                return false;
            }
            return this.Id == video.Id && this.Chapter.Equals(video.Chapter);
        }

        public override int GetHashCode()
        {
            int hash = 19;
            hash = hash * 31 + this.Id;
            hash = hash * 31 + this.Chapter.GetHashCode();
            return hash;
        }

    }

}
