using System.Diagnostics;

namespace ExecuteTerminal
{
    public static class ExecuteCommandBashExtension
    {
        public static string ExecuteCommandBash(this string command)
        {
            command = command.Replace("\"", "\"\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = @$"-c {command}",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }
    }
}