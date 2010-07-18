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

                Assert.IsTrue(argument.StartsWith("\"Path\\Assembly1.dll\""));
            }

            [Test]
            public void Class_with_two_tests_has_a_joined_together_filter()
            {
                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test
                {
                    ClassName = "Class1",
                    MethodName = "Method1",
                    AssemblyName = "Assembly1",
                    PathToAssembly = "Path\\"
                });
                testsToExecute.Add(new Test
                {
                    ClassName = "Class1",
                    MethodName = "Method2",
                    AssemblyName = "Assembly1",
                    PathToAssembly = "Path\\"
                });

                GallioTestRunner runner = new GallioTestRunner();
                string argument = runner.CreateArguments(testsToExecute);

                Assert.IsTrue(argument.Contains("Type:Class1 AND Member:Method1 OR Member:Method2"));
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
                Assert.IsNotNull(execute);
            }

            [Test]
            public void Can_parse_results_with_passing_tests_into_an_object()
            {
                string output =
                    @"Gallio Echo - Version 3.2 build 517
Get the latest version at http://www.gallio.org/ 


2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped";

                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "TestAssembly1", PathToAssembly = Path.GetDirectoryName(assembly.Path) });

                GallioTestRunner runner = new GallioTestRunner();
                TestExecutionResults results = runner.Parse(output, 0);
                Assert.That(results.Run, Is.EqualTo(2));
                Assert.That(results.Passed, Is.EqualTo(2));
            }

            [Test]
            public void Can_parse_results_with_failing_tests_into_an_object()
            {
                string output =
                    @"Gallio Echo - Version 3.2 build 517
Get the latest version at http://www.gallio.org/ 


2 run, 2 passed, 1 failed, 0 inconclusive, 0 skipped";

                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "TestAssembly1", PathToAssembly = Path.GetDirectoryName(assembly.Path) });

                GallioTestRunner runner = new GallioTestRunner();
                TestExecutionResults results = runner.Parse(output, 0);
                Assert.That(results.Failed, Is.EqualTo(1));
            }

            [Test]
            public void Can_parse_results_with_skipped_and_inconclusive_which_are_added_together()
            {
                string output =
                    @"Gallio Echo - Version 3.2 build 517
Get the latest version at http://www.gallio.org/ 


2 run, 2 passed, 1 failed, 3 inconclusive, 3 skipped";

                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "TestAssembly1", PathToAssembly = Path.GetDirectoryName(assembly.Path) });

                GallioTestRunner runner = new GallioTestRunner();
                TestExecutionResults results = runner.Parse(output, 0);
                Assert.That(results.Skipped, Is.EqualTo(6));
            }

            [Test]
            public void Exit_code_of_non_0_sets_status_to_failed()
            {
                string output =
                    @"Gallio Echo - Version 3.2 build 517
Get the latest version at http://www.gallio.org/ 


2 run, 2 passed, 1 failed, 3 inconclusive, 3 skipped";

                List<Test> testsToExecute = new List<Test>();
                testsToExecute.Add(new Test { ClassName = "Class1", MethodName = "Method1", AssemblyName = "TestAssembly1", PathToAssembly = Path.GetDirectoryName(assembly.Path) });

                GallioTestRunner runner = new GallioTestRunner();
                TestExecutionResults results = runner.Parse(output, 1);
                Assert.That(results.ExecutionResult, Is.EqualTo(TestExecutionResult.Failed));
            }
        }
    }
}