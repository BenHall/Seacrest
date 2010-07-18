using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.Generic;
using Seacrest.Analyser.Parsers.TestExplorer;

namespace Seacrest.Analyser.Tests.Execution
{
    [TestFixture]
    public class GallioTestRunnerTests
    {
        [Test]
        public void Create_argument_list_based_on_list_of_tests_to_execute()
        {
            List<Test> testsToExecute = new List<Test>();
            testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "Assembly1", PathToAssembly = "Path\\" });

            GallioTestRunner runner = new GallioTestRunner();
            string argument = runner.CreateArguments(testsToExecute);

            Assert.IsTrue(argument.Contains("Type:Class1 AND Member:Method1"));
        }

        [Test]
        public void Create_argument_list_includes_assembly_to_execute()
        {
            List<Test> testsToExecute = new List<Test>();
            testsToExecute.Add(new Test
                                   {
                                       ClassName = "Class1",
                                       MethodName = "Method1",
                                       AssemblyName = "Assembly1",
                                       PathToAssembly = "Path\\"
                                   });

            GallioTestRunner runner = new GallioTestRunner();
            string argument = runner.CreateArguments(testsToExecute);

            Assert.IsTrue(argument.StartsWith("Path\\Assembly1.dll"));
        }
    }

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
                builder.Append(String.Join(" AND ", filters.ToArray()));
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