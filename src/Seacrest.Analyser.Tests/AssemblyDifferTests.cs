using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests
{
    [TestFixture]
    public class AssemblyDifferTests
    {
        AssemblyBuilderResult assemblyOne;
        AssemblyBuilderResult assemblyTwo;

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

            assemblyTwo = builder.AssemblyName("TestAssembly2")
                                    .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"},
                                                                            {"Method2", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                    .Build();
        }
        [Test]
        public void FindNewMethods_should_return_the_the_additional_method_added_to_assembly2()
        {
            var newMethodName = "Method2";
            
            AssemblyDiffer differ = new AssemblyDiffer();
            IEnumerable<ChangedMethods> newMethods = differ.FindNewMethods(assemblyOne.Path, assemblyTwo.Path);

            Assert.That(newMethods.Count(), Is.EqualTo(1));
            Assert.That(newMethods.First().Name, Is.EqualTo(newMethodName));
        }
    }
}