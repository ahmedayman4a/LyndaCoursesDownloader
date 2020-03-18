using System;

namespace LyndaCoursesDownloader.CourseExtractor
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message)
            : base(message)
        {
        }

        public InvalidTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
