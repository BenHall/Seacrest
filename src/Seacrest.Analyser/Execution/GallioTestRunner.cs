using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Seacrest.Analyser.Exceptions;
using Seacrest.Analyser.Parsers.TestExplorer;

namespace Seacrest.Analyser.Execution
{
    public class GallioTestRunner
    {
        public bool Execute(List<Test> testsToExecute)
        {
            string gallioEchoExe = @"D:\Users\Ben Hall\Downloads\GallioBundle-3.2.517.0\bin - Copy\Gallio.Echo.exe";
            Process process = InternalProcessExecutor.Start(gallioEchoExe, CreateArguments(testsToExecute));

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.HasExited)
            {
                var exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    var substring = output;
                    throw new TestFailedException(substring.Trim());
                }
                return true;
            }

            return false;
        }

        public string CreateArguments(List<Test> testsToExecute)
        {
            StringBuilder builder = new StringBuilder();
            var firstTest = testsToExecute.First();
            if (firstTest != null)
            {
                List<string> filters = BuildFilterList(testsToExecute);

                builder.Append("\"" + Path.Combine(firstTest.PathToAssembly, firstTest.AssemblyName + ".dll") + "\"");
                builder.Append(" ");
                builder.Append("/filter:\"" + String.Join(" AND ", filters.ToArray()) + "\"");
                builder.Append(" /np /v:Quiet /no-echo-results");
            }

            return builder.ToString();
        }

        private List<string> BuildFilterList(IEnumerable<Test> testsToExecute)
        {
            return testsToExecute.Select(test => string.Format("(Type:{0} AND Member:{1})", test.ClassName, test.MethodName)).ToList();
        }
    }
}