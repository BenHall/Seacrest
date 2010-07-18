using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Seacrest.Analyser.Parsers.TestExplorer;

namespace Seacrest.Analyser.Execution
{
    public class GallioTestRunner
    {
        public string CreateArguments(List<Test> testsToExecute)
        {
            StringBuilder builder = new StringBuilder();
            var firstTest = testsToExecute.First();
            if (firstTest != null)
            {
                List<string> filters = BuildFilterList(testsToExecute);

                builder.Append(Path.Combine(firstTest.PathToAssembly, firstTest.AssemblyName + ".dll"));
                builder.Append(" ");
                builder.Append(String.Join(" AND ", filters.ToArray()));
                builder.Append(" /np /v:Quiet /no-echo-results");
            }

            return builder.ToString();
        }

        private List<string> BuildFilterList(List<Test> testsToExecute)
        {
            List<string> filters = new List<string>();
            foreach (var test in testsToExecute)
            {
                filters.Add(string.Format("(Type:{0} AND Member:{1})", test.ClassName, test.MethodName));
            }
            return filters;
        }
    }
}