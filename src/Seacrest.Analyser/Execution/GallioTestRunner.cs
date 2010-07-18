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
            if (!testsToExecute.Any())
                return null;

            //TODO: Make this as part of the solution...
            string gallioEchoExe = @"D:\Users\Ben Hall\Downloads\GallioBundle-3.2.517.0\bin - Copy\Gallio.Echo.exe";
            Process process = InternalProcessExecutor.Start(gallioEchoExe, CreateArguments(testsToExecute), Path.GetDirectoryName(gallioEchoExe));

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
            var firstTest = testsToExecute.FirstOrDefault();
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
            List<string> filters = new List<string>();
            Dictionary<string, List<string>> methodsPerClass = MethodsToExecuteInEachClass(testsToExecute);

            foreach (var cls in methodsPerClass)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("(Type:{0} AND ", cls.Key);
                
                var methods = GetMethodsFilterList(cls).ToArray();
                builder.Append(string.Join(" OR ", methods));
                builder.Append(")");

                filters.Add(builder.ToString());
            }
            return filters;
        }

        private List<string> GetMethodsFilterList(KeyValuePair<string, List<string>> cls)
        {
            List<string> methods = new List<string>();
            foreach (var method in cls.Value)
            {
                methods.Add(string.Format("Member:{0}", method));
            }
            return methods;
        }

        private Dictionary<string, List<string>> MethodsToExecuteInEachClass(IEnumerable<Test> testsToExecute)
        {
            Dictionary<string, List<String>> clsFilter = new Dictionary<string, List<string>>();
            foreach (var test in testsToExecute)
            {
                if(!clsFilter.ContainsKey(test.ClassName))
                    clsFilter.Add(test.ClassName, new List<string>());

                List<string> listOfMethods = clsFilter[test.ClassName];
                listOfMethods.Add(test.MethodName);
            }
            return clsFilter;
        }

        public TestExecutionResults Parse(string output, int exitCode)
        {
            string pattern = "(?<run>.+) run, (?<passed>.+) passed, (?<failed>.+) failed (.*), (?<inconclusive>.+) inconclusive, (?<skipped>.+) skipped";
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
                results.Output = output;
                return results;
            }

            return null;
        }
    }
}