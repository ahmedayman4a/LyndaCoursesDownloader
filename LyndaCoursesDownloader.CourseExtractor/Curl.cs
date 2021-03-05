using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LyndaCoursesDownloader.CourseExtractor
{
    internal class Curl
    {
        private readonly string _token;

        internal Curl(string token)
        {
            _token = token;
        }
        internal async Task<string> GetCurlRedirectUrl(string url)
        {
            string request = await CurlCustomRequest(String.Format("-I -X GET \"{0}\" -b token=\"{1}\"", url, _token));
            Regex pattern = new Regex(@"(\n|\r)Location: (?<downloadUrl>(https?\:\/\/.*))(\n|\r)");
            string redirectUrl = pattern.Match(request).Groups["downloadUrl"].Value.Trim();
            return redirectUrl;
        }
        internal async Task<string> CurlRequest(string url)
        {
            return await CurlCustomRequest(String.Format("-X GET \"{0}\" -b token=\"{1}\"", url, _token));
        }
        internal async Task<string> CurlCustomRequest(string customArguments)
        {
            return await Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    startInfo.FileName = "curl.exe";
                    startInfo.WorkingDirectory = "./curl/bin/";
                }
                else
                {
                    startInfo.FileName = "curl"; //curl must be installed on linux and mac os
                }

                startInfo.Arguments = customArguments + " -A \"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0\"";
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                Process curlProcess = Process.Start(startInfo);
                string output = "";
                while (!curlProcess.StandardOutput.EndOfStream)
                {
                    output += curlProcess.StandardOutput.ReadLine() + Environment.NewLine;
                }
                curlProcess.WaitForExit();
                return output.TrimEnd(Environment.NewLine.ToCharArray());
            });

        }
    }
}
