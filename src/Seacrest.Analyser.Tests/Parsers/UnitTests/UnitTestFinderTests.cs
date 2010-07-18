using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Parsers.UnitTests;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests.Parsers.UnitTests
{
    [TestFixture]
    public class UnitTestFinderTests
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
        public class UnitTestFinderTests_OneDeep : UnitTestFinderTests
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
                UnitTestFinder finder = new UnitTestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                Assert.IsNotNull(usages.Where(x=>x.MethodName == "Method1" && x.ClassName == "Class1"));
            }

            [Test]
            public void Returns_Method1_with_information_about_test_calling_it()
            {
                UnitTestFinder finder = new UnitTestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                var methodUsages = usages.Single(x => x.MethodName == "Method1" && x.ClassName == "Class1");
                Assert.That(methodUsages.TestCoverage.First().MethodName, Is.EqualTo("Method1Test"));
            }

            [Test]
            public void Returns_both_unit_tests_for_single_usage()
            {
                UnitTestFinder finder = new UnitTestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyThree.Path);
                var methodUsages = usages.Single(x => x.MethodName == "Method1" && x.ClassName == "Class1");
                Assert.That(methodUsages.TestCoverage.Count(), Is.EqualTo(2));
            }
        }
    }
}