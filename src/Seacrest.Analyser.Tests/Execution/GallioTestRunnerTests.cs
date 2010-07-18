using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using Seacrest.Analyser.Execution;
using Seacrest.Analyser.Parsers.TestExplorer;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests.Execution
{
    public class GallioTestRunnerTests
    {
        [TestFixture]
        public class GallioTestRunnerTests_Arguments
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

        [TestFixture]
        public class GallioTestRunnerTests_Execution
        {
            private AssemblyBuilderResult assembly;
            [SetUp]
            public void Setup()
            {
                AssemblyBuilder builder = new AssemblyBuilder();
                assembly = builder.AssemblyName("TestAssembly1")
                    .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                       {
                                                           {"Method1", "System.Console.WriteLine(\"Test\");"}
                                                       })
                    .Build();
            }

            [TearDown]
            public void Teardown()
            {
                File.Delete(assembly.Path);
            }

            [Test]
            public void Can_execute_tests()
            {
                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "TestAssembly1", PathToAssembly = Path.GetDirectoryName(assembly.Path) });

                GallioTestRunner runner = new GallioTestRunner();
                var execute = runner.Execute(testsToExecute);
                Assert.IsTrue(execute);
            }
        }
    }
}