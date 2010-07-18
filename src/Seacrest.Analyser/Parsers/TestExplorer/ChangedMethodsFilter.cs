using System.Collections.Generic;
using System.Linq;
using Seacrest.Analyser.Parsers.Differs;

namespace Seacrest.Analyser.Parsers.TestExplorer
{
    public class ChangedMethodsFilter
    {
        public IEnumerable<Test> FindUnitTestsAffectedByChanges(IEnumerable<ChangedMethod> changedMethods, IEnumerable<MethodUsage> methodUsages)
        {
            List<Test> tests = new List<Test>();
            foreach (var changedMethod in changedMethods)
            {
                var unitTests = FindUnitTestsAffectedByChange(changedMethod, methodUsages);
                tests.AddRange(unitTests);
            }

            return tests;
        }

        public IEnumerable<Test> FindUnitTestsAffectedByChange(ChangedMethod changedMethod, IEnumerable<MethodUsage> methodUsages)
        {
            return methodUsages.Where(x => x.MethodName == changedMethod.MethodName && x.ClassName == changedMethod.ClassName).SelectMany(x=>x.TestCoverage);
        }
    }
}