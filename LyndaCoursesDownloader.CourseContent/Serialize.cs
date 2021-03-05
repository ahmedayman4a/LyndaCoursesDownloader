using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LyndaCoursesDownloader.CourseContent
{
    public static class Serialize
    {
        public static string ToJson(this Course self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
