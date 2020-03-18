using Bumblebee.Implementation;
using Bumblebee.Interfaces;
using LyndaCoursesDownloader.CourseContent;
using OpenQA.Selenium;

namespace LyndaCoursesDownloader.CourseElements
{
    public class VideoBlock : Block
    {
        public VideoBlock(IBlock parent, By by) : base(parent, by)
        {
        }

        public static implicit operator Video(VideoBlock videoBlock)
        {
            return new Video
            {
                Name = videoBlock.VideoName,
                VideoUrl = videoBlock.VideoUrl
            };
        }

        public string VideoName => TextGetter.GetText(() => FindElement(By.CssSelector("div a")).Text);

        public string VideoUrl => TextGetter.GetText(() => FindElement(By.CssSelector("div a")).GetAttribute("href"));
    }
}
