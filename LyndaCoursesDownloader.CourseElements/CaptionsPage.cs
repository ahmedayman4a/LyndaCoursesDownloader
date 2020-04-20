using Bumblebee.Implementation;
using Bumblebee.Setup;
using System.Web;

namespace LyndaCoursesDownloader.CourseElements
{
    public class CaptionsPage : Page
    {
        private Session _session;
        private static readonly string[] noCaption = { "SyntaxError: JSON.parse: expected property name or '}' at line 1 column 3 of the JSON data", "{ Status=\"NotFound\", Message=\"Transcript not found\" }" };

        public CaptionsPage(Session session) : base(session)
        {
            _session = session;
        }
        public string CaptionText
        {
            get
            {
                string source = _session.Driver.PageSource;
                if (!source.Contains(noCaption[0]) || !source.Contains(noCaption[1]))
                {
                    return TextGetter.GetText(() => HttpUtility.HtmlDecode(FindElement(By.TagName("pre")).Text));
                }
                return null;
            }
        }
    }
}
