using System.IO;
using NUnit.Framework;

namespace Seacrest.Analyser.Tests
{
    [TestFixture]
    public class SolutionBuilderTests
    {
        private string _outDir;

        [SetUp]
        public void Setup()
        {
            _outDir = @"C:\temp\SeacrestTestBuild\";
            Directory.CreateDirectory(_outDir);
        }


        [TearDown]
        public void Teardown()
        {
            Directory.Delete(_outDir, true);
        }

        [Test]
        public void Builds_solution_to_a_particular_directory()
        {
            SolutionBuilder builder = new SolutionBuilder();
            bool result = builder.Build(@"D:\SourceControl\Seacrest\src\Seacrest.sln", _outDir);

            Assert.IsTrue(result);
        }
    }
}