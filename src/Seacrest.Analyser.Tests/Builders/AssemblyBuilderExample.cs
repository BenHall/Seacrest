using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Seacrest.Analyser.Tests.Builders
{
    [TestFixture]
    public class AssemblyBuilderExample
    {
        [Test]
        public void Example_of_how_to_build_an_assembly()
        {
            AssemblyBuilder builder = new AssemblyBuilder();
            AssemblyBuilderResult result = builder.AssemblyName("TestAssembly1")
                .WithClassAndMethods("Class1", new Dictionary<string, string>
                                                   {
                                                       { "Method1", "System.Console.WriteLine(\"Test\");" }
                                                   })
                .Build();

            Assert.IsTrue(File.Exists(result.Path));

        }
    }
}