using System.Collections.Generic;
using System.Linq;

namespace LyndaCoursesDownloader.CourseContent
{
    public static class IdFiller
    {
        public static IEnumerable<T> FillId<T>(this IEnumerable<T> contents) where T : ICourse
        {
            contents.ToList().ForEach(q => q.Id = contents.OrderByDescending(x => x.Id).Select(y => y.Id).ToList()[0] + 1);
            return contents;
        }

    }
}
