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

        [TearDown]
        public void Teardown()
        {
            File.Delete(assemblyOne.Path);
            File.Delete(assemblyTwo.Path);
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
            }

            [Test]
            public void Returns_Method1_as_a_method_used_by_the_test()
            {
                UnitTestFinder finder = new UnitTestFinder();
                IEnumerable<MethodUsage> usages = finder.FindUsagesViaTests(assemblyTwo.Path);
                Assert.IsNotNull(usages.Where(x=>x.MethodName == "Method1" && x.ClassName == "Class1"));
            }
        }
    }
}