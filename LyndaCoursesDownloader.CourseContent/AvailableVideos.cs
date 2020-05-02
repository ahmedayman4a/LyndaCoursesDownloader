using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LyndaCoursesDownloader.CourseContent
{
    public static class AvailableVideos
    {
        public static Video GetAvailableVideo(this List<Video> Videos, object StatusLock)
        {
            Monitor.Enter(StatusLock);
            Video video = Videos.Where(vid => vid.CurrentVideoStatus == CurrentStatus.Ready).FirstOrDefault();
            if (!(video is null))
            {
                video.CurrentVideoStatus = CurrentStatus.Busy;
            }

            Monitor.Exit(StatusLock);
            return video;
        }
    }
}
