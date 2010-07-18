using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Parsers.UnitTests;
using System.Collections.Generic;

namespace Seacrest.Analyser.Tests.Parsers.UnitTests
{
    [TestFixture]
    public class ChangedMethodsFilterTests
    {
        [Test]
        public void Given_a_list_of_method_usages_it_filters_down_based_on_changed_methods_list()
        {
            ChangedMethod method = new ChangedMethod {
                                                         ClassName = "Class1", MethodName = "Method1"
                                                     };

            MethodUsage usage = new MethodUsage {ClassName = "Class1", MethodName = "Method1"};

            var expectedTestToExecuted = new UnitTest{ClassName = "Class1Tests", MethodName = "Method1Tests"};
            usage.TestCoverage = new List<UnitTest>();
            usage.TestCoverage.Add(expectedTestToExecuted);

            ChangedMethodsFilter filter = new ChangedMethodsFilter();
            IEnumerable<UnitTest> testsToExecuted = filter.FindUnitTestsAffectedByChanges(new List<ChangedMethod> {method}, new List<MethodUsage> {usage});

            Assert.That(testsToExecuted.Count(), Is.EqualTo(1));
            Assert.IsTrue(testsToExecuted.Contains(expectedTestToExecuted));
        }
    }
}