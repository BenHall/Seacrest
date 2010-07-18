using NUnit.Framework;
using System.Collections.Generic;
using Seacrest.Analyser.Execution;
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
            Assert.IsTrue(argument.Contains("/filter:"));
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
}