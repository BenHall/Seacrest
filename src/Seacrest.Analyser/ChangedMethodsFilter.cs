using System.Collections.Generic;
using System.Linq;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Parsers.UnitTests;

namespace Seacrest.Analyser
{
    public class ChangedMethodsFilter
    {
        public IEnumerable<UnitTest> FindUnitTestsAffectedByChanges(List<ChangedMethod> changedMethods, List<MethodUsage> methodUsages)
        {
            List<UnitTest> tests = new List<UnitTest>();
            foreach (var changedMethod in changedMethods)
            {
                var unitTests = methodUsages.Where(x => x.MethodName == changedMethod.MethodName && x.ClassName == changedMethod.ClassName).SelectMany(x=>x.TestCoverage);
                tests.AddRange(unitTests);
            }

            return tests;
        }
    }
}