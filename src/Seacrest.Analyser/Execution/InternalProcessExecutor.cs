using System.Diagnostics;

namespace Seacrest.Analyser.Execution
{
    public class InternalProcessExecutor
    {
        public static Process Start(string command, string arguments, string workingDirectory)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments)
                                             {
                                                 RedirectStandardError = true,
                                                 RedirectStandardOutput = true,
                                                 CreateNoWindow = true,
                                                 UseShellExecute = false,
                                                 WorkingDirectory = workingDirectory
                                             };

            var process = new Process {StartInfo = startInfo};
            process.Start();

            return process;
        }
    }
}