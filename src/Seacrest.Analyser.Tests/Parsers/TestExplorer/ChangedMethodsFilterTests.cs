using System.Linq;
using NUnit.Framework;
using Seacrest.Analyser.Parsers.Differs;
using System.Collections.Generic;
using Seacrest.Analyser.Parsers.TestExplorer;

namespace Seacrest.Analyser.Tests.Parsers.TestExplorer
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

            var expectedTestToExecuted = new Test{ClassName = "Class1Tests", MethodName = "Method1Tests"};
            usage.TestCoverage = new List<Test>();
            usage.TestCoverage.Add(expectedTestToExecuted);

            ChangedMethodsFilter filter = new ChangedMethodsFilter();
            IEnumerable<Test> testsToExecuted = filter.FindUnitTestsAffectedByChanges(new List<ChangedMethod> {method}, new List<MethodUsage> {usage});

            Assert.That(testsToExecuted.Count(), Is.EqualTo(1));
            Assert.IsTrue(testsToExecuted.Contains(expectedTestToExecuted));
        }

        [Test]
        public void Given_a_list_of_method_usages_it_filters_down_based_on_single_changed_method()
        {
            ChangedMethod method = new ChangedMethod
            {
                ClassName = "Class1",
                MethodName = "Method1"
            };

            MethodUsage usage = new MethodUsage { ClassName = "Class1", MethodName = "Method1" };

            var expectedTestToExecuted = new Test { ClassName = "Class1Tests", MethodName = "Method1Tests" };
            usage.TestCoverage = new List<Test>();
            usage.TestCoverage.Add(expectedTestToExecuted);

            ChangedMethodsFilter filter = new ChangedMethodsFilter();
            IEnumerable<Test> testsToExecuted = filter.FindUnitTestsAffectedByChange(method, new List<MethodUsage> { usage });

            Assert.That(testsToExecuted.Count(), Is.EqualTo(1));
            Assert.IsTrue(testsToExecuted.Contains(expectedTestToExecuted));
        }
    }
}