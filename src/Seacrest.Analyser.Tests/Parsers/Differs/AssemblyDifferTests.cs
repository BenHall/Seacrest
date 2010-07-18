using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Tests.Builders;

namespace Seacrest.Analyser.Tests.Parsers.Differs
{
    public class AssemblyDifferTests
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
                assemblyTwo = builder2.AssemblyName("TestAssembly2")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"},
                                                                            {"Method2", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .Build();
            }

            [Test]
            public void FindModifiedMethods_should_return_the_the_additional_method_added_to_assembly2()
            {
                var newMethodName = "Method2";

                AssemblyDiffer differ = new AssemblyDiffer();
                IEnumerable<ChangedMethod> newMethods = differ.FindModifiedMethods(assemblyOne.Path, assemblyTwo.Path);

                Assert.That(newMethods.Count(), Is.EqualTo(1));
                Assert.That(newMethods.First().MethodName, Is.EqualTo(newMethodName));
            }

            [Test]
            public void New_methods_have_changereason_set_to_true()
            {
                AssemblyDiffer differ = new AssemblyDiffer();
                IEnumerable<ChangedMethod> newMethods = differ.FindModifiedMethods(assemblyOne.Path, assemblyTwo.Path);

                Assert.That(newMethods.First().ChangeReason, Is.EqualTo(ChangeReason.New));
            }

            [Test]
            public void New_methods_have_correct_assembly_name()
            {
                AssemblyDiffer differ = new AssemblyDiffer();
                IEnumerable<ChangedMethod> newMethods = differ.FindModifiedMethods(assemblyOne.Path, assemblyTwo.Path);

                Assert.That(newMethods.First().AssemblyName, Is.EqualTo("TestAssembly2.dll"));
            }
        }

        [TestFixture]
        public class AssemblyDifferTests_UpdatedMethods : AssemblyDifferTests
        {
          
            [SetUp]
            public void Setup()
            {
                AssemblyBuilder builder = new AssemblyBuilder();
                assemblyOne = builder.AssemblyName("TestAssembly1")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"},
                                                                            {"Method2", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .Build();

                AssemblyBuilder builder2 = new AssemblyBuilder();
                assemblyTwo = builder2.AssemblyName("TestAssembly2")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"My value has changed\");"},
                                                                            {"Method2", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .Build();
            }

            [Test]
            public void FindModifiedMethods_should_return_method_one_as_changed_due_to_code_change()
            {
                var newMethodName = "Method1";

                AssemblyDiffer differ = new AssemblyDiffer();
                IEnumerable<ChangedMethod> newMethods = differ.FindModifiedMethods(assemblyOne.Path, assemblyTwo.Path);

                Assert.That(newMethods.Count(), Is.EqualTo(1));
                Assert.That(newMethods.First().MethodName, Is.EqualTo(newMethodName));
                Assert.That(newMethods.First().ChangeReason, Is.EqualTo(ChangeReason.Updated));
            }
        }

        [TestFixture]
        public class AssemblyDifferTests_NewClasses : AssemblyDifferTests
        {

            [SetUp]
            public void Setup()
            {
                AssemblyBuilder builder = new AssemblyBuilder();
                assemblyOne = builder.AssemblyName("TestAssembly1")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"},
                                                                        })
                                        .Build();

                AssemblyBuilder builder2 = new AssemblyBuilder();
                assemblyTwo = builder2.AssemblyName("TestAssembly2")
                                        .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .WithClassAndMethods("NewClass", new Dictionary<string, string>
                                                                        {
                                                                            {"Method1", "System.Console.WriteLine(\"Test\");"}
                                                                        })
                                        .Build();
            }

            [Test]
            public void FindModifiedMethods_should_return_method_one_as_changed_due_to_code_change()
            {
                var newMethodName = "Method1";
                var newClassName = "NewClass";

                AssemblyDiffer differ = new AssemblyDiffer();
                IEnumerable<ChangedMethod> newMethods = differ.FindModifiedMethods(assemblyOne.Path, assemblyTwo.Path);

                Assert.That(newMethods.Count(), Is.EqualTo(1));
                Assert.That(newMethods.First().ClassName, Is.EqualTo(newClassName));
                Assert.That(newMethods.First().MethodName, Is.EqualTo(newMethodName));
                Assert.That(newMethods.First().ChangeReason, Is.EqualTo(ChangeReason.New));
            }
        }
    }
}