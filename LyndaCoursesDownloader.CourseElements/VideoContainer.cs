using Bumblebee.Implementation;
using Bumblebee.Interfaces;
using OpenQA.Selenium;

namespace LyndaCoursesDownloader.CourseElements
{
    public class VideoContainer : Block
    {
        public VideoContainer(IBlock parent, By by) : base(parent, by)
        {

        }
        public int VideoId { get; set; }
        public IClickable<VideoContainer> QualitySettings => new Clickable<VideoContainer>(this, By.Id("player-settings"));

        public IClickable<VideoContainer> Quality360 => new Clickable<VideoContainer>(this, By.CssSelector("ul.stream-qualities li a[data-quality=\"360\"]"));

        public IClickable<VideoContainer> Quality540 => new Clickable<VideoContainer>(this, By.CssSelector("ul.stream-qualities li a[data-quality=\"540\"]"));

        public IClickable<VideoContainer> Quality720 => new Clickable<VideoContainer>(this, By.CssSelector("ul.stream-qualities li a[data-quality=\"720\"]"));

        public IClickable<VideoContainer> CaptionElement => new Clickable<VideoContainer>(this, By.CssSelector("video track"));

        public IClickable<VideoContainer> WatchVideoButton => new Clickable<VideoContainer>(this, By.Id("banner-play"));

        //public IClickable<VideoContainer> PlayPauseButton => new Clickable<VideoContainer>(this, By.Id("player-playpause"));

        public string VideoDownloadUrl => TextGetter.GetText(() => FindElement(By.CssSelector("video")).GetAttribute("src"));
    }
}
