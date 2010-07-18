using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Parsers.TestExplorer;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests.Parsers.TestExplorer
{
    [TestFixture]
    public class TestFinderTests
    {
        AssemblyBuilderResult assemblyOne;
        AssemblyBuilderResult assemblyTwo;
        AssemblyBuilderResult assemblyThree;

        [TearDown]
        public void Teardown()
        {
            File.Delete(assemblyOne.Path);
            File.Delete(assemblyTwo.Path);
            File.Delete(assemblyThree.Path);
        }

        [TestFixture]
        public class TestFinderTestsOneDeep : TestFinderTests
        {
            [SetUp]
            public void Setup()
            {
                AssemblyBuilder builder = new AssemblyBuilder();
                assemblyOne = builder.AssemblyName("TestAssembly1")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .Build();

                AssemblyBuilder builder2 = new AssemblyBuilder();
                assemblyTwo = builder2.AssemblyName("TestAssembly1.Tests")
                                        .References(Path.GetFileName(assemblyOne.Path))
                                        .WithClassAndMethods("Class1Tests", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1Test", "var c = new Class1();" + Environment.NewLine + 
                                                                                            "c.Method1();"},
                                                                        })
                                        .Build();

                AssemblyBuilder builder3 = new AssemblyBuilder();
                assemblyThree = builder3.AssemblyName("TestAssembly1.Tests")
                                        .References(Path.GetFileName(assemblyOne.Path))
                                        .WithClassAndMethods("Class1Tests", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1Test", "var c = new Class1();" + Environment.NewLine + 
                                                                                            "c.Method1();"},
                                                                            {"Method1Test2", "var c = new Class1();" + Environment.NewLine + 
                                                                            "c.Method1();"},
                                                                        })
                                        .Build();
            }

            [Test]
            public void Returns_Method1_as_a_method_used_by_the_test()
            {
                TestFinder finder = new TestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                Assert.IsNotNull(usages.Where(x=>x.MethodName == "Method1" && x.ClassName == "Class1"));
            }

            [Test]
            public void Returns_Method1_with_information_about_test_calling_it()
            {
                TestFinder finder = new TestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                var methodUsages = usages.Single(x => x.MethodName == "Method1" && x.ClassName == "Class1");
                Assert.That(methodUsages.TestCoverage.First().MethodName, Is.EqualTo("Method1Test"));
            }

            [Test]
            public void Returns_both_unit_tests_for_single_usage()
            {
                TestFinder finder = new TestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyThree.Path);
                var methodUsages = usages.Single(x => x.MethodName == "Method1" && x.ClassName == "Class1");
                Assert.That(methodUsages.TestCoverage.Count(), Is.EqualTo(2));
            }

            [Test]
            public void Path_and_assembly_name_set_correctly_on_test_objects()
            {
                TestFinder finder = new TestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                var methodUsages = usages.Single(x => x.MethodName == "Method1" && x.ClassName == "Class1");
                Assert.That(methodUsages.TestCoverage.First().AssemblyName, Is.EqualTo("TestAssembly1.Tests"));
                Assert.That(methodUsages.TestCoverage.First().PathToAssembly, Is.EqualTo(Path.GetDirectoryName(Path.GetTempPath())));
            }
        }
    }
}