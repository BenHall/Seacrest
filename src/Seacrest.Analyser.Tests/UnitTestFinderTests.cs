using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests
{
    [TestFixture]
    public class UnitTestFinderTests
    {
        public class AssemblyDifferTests : UnitTestFinderTests
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
            public class AssemblyDifferTests_NewMethods : AssemblyDifferTests
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
                public void x()
                {
                    
                }
            }
        }
    }
}