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
            //bool hasVideos = true;
            Video video = Videos.Where(vid => vid.CurrentVideoStatus == CurrentStatus.Ready).FirstOrDefault();
            if (!(video is null))
            {
                video.CurrentVideoStatus = CurrentStatus.Busy;
            }

            //if (Videos.Where(vid => vid.CurrentVideoStatus == CurrentStatus.Ready).Count() == 0)
            //{
            //    hasVideos = false;
            //}
            Monitor.Exit(StatusLock);
            return video;
        }
    }
}
