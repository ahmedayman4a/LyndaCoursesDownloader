using Bumblebee.Implementation;
using Bumblebee.Interfaces;
using LyndaCoursesDownloader.CourseContent;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace LyndaCoursesDownloader.CourseElements
{
    public class ChapterBlock : Block
    {
        public ChapterBlock(IBlock parent, By by) : base(parent, by)
        {
        }
        public static implicit operator Chapter(ChapterBlock chapterBlock)
        {
            Chapter chapter = new Chapter
            {
                Id = chapterBlock.ChapterId,
                Name = chapterBlock.ChapterName,
                Videos = new List<Video>()
            };

            int i = 1;
            foreach (var videoBlock in chapterBlock.VideoBlocks)
            {
                Video video = videoBlock;
                video.Chapter = chapter;
                video.Id = i;
                chapter.Videos.Add(video);
                i++;
            }
            return chapter;
        }
        public string ChapterName => TextGetter.GetText(() => FindElement(By.CssSelector("div.chapter-row div h4")).Text);

        public int ChapterId { get; set; }

        public IEnumerable<VideoBlock> VideoBlocks => new Blocks<VideoBlock>(this, By.TagName("ul.toc-items li.toc-video-item div.video-row"));
    }
}
