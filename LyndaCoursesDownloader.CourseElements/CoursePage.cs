using Bumblebee.Implementation;
using Bumblebee.Setup;
using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseElements
{
    public class CoursePage : Page
    {
        public CoursePage(Session session) : base(session)
        {

        }
        public string CourseName => TextGetter.GetText(() => FindElement(By.CssSelector("h1.default-title")).GetAttribute("data-course"));

        public IEnumerable<ChapterBlock> ChapterBlocks => new Blocks<ChapterBlock>(this, By.CssSelector("ul.course-toc > li"));

        public VideoContainer VideoBlock => new VideoContainer(this, By.Id("video-container"));
    }
}
