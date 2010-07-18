using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Seacrest.Analyser.Parsers.TestExplorer;

namespace Seacrest.Analyser.Execution
{
    public class GallioTestRunner
    {
        public TestExecutionResults Execute(IEnumerable<Test> testsToExecute)
        {
            string gallioEchoExe = @"D:\Users\Ben Hall\Downloads\GallioBundle-3.2.517.0\bin - Copy\Gallio.Echo.exe";
            Process process = InternalProcessExecutor.Start(gallioEchoExe, CreateArguments(testsToExecute));

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.HasExited)
            {
                var exitCode = process.ExitCode;
                var results = Parse(output, exitCode);
                return results;
            }

            return null;
        }

        public string CreateArguments(IEnumerable<Test> testsToExecute)
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

        public TestExecutionResults Parse(string output, int exitCode)
        {
            string pattern = "(?<run>.+) run, (?<passed>.+) passed, (?<failed>.+) failed, (?<inconclusive>.+) inconclusive, (?<skipped>.+) skipped";
            Regex regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            Match m = regex.Match(output);
            if (m.Success)
            {
                TestExecutionResults results = new TestExecutionResults();
                results.Run = Convert.ToInt32(m.Groups["run"].Value.Trim());
                results.Passed = Convert.ToInt32(m.Groups["passed"].Value.Trim());
                results.Failed = Convert.ToInt32(m.Groups["failed"].Value.Trim());
                results.Skipped = Convert.ToInt32(m.Groups["inconclusive"].Value.Trim()) + Convert.ToInt32(m.Groups["skipped"].Value.Trim());

                results.ExecutionResult = exitCode == 0 ? TestExecutionResult.Passed : TestExecutionResult.Failed;

                return results;
            }

            return null;
        }
    }
}