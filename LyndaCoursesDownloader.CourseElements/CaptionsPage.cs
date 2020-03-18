using Bumblebee.Implementation;
using Bumblebee.Setup;
using System.Web;

namespace LyndaCoursesDownloader.CourseElements
{
    public class CaptionsPage : Page
    {
        public CaptionsPage(Session session) : base(session)
        {

        }

        public string CaptionText => TextGetter.GetText(() => HttpUtility.HtmlDecode(FindElement(By.TagName("pre")).Text));
    }
}
