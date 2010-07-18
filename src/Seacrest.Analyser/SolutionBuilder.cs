using System.Diagnostics;
using Seacrest.Analyser.Exceptions;
using System.IO;
using Seacrest.Analyser.Execution;

namespace Seacrest.Analyser
{
    public class SolutionBuilder : ISolutionBuilder
    {
        public bool Build(string pathToSolution, string outDir)
        {
            string cmd = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";

            string options = string.Format("{0} /nologo /verbosity:m /p:OutDir={1}", Path.GetFileName(pathToSolution), outDir);

            Process process = InternalProcessExecutor.Start(cmd, options);
            
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit(1000);
            if (process.HasExited)
            {
                var exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    throw new BuildFailedException(output);
                }
            }

            return true;
        }
    }
}