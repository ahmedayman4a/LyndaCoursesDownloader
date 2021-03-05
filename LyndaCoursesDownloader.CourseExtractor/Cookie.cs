
namespace LyndaCoursesDownloader.CourseExtractor
{
    class Cookie
    {
        internal Cookie() { }
        internal Cookie(string name, string value)
        {
            Name = name;
            Value = value;
        }
        internal string Name { get; set; }
        internal string Value { get; set; }
    }
}
