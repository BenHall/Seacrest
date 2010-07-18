using System.Collections.Generic;

namespace Seacrest.Analyser.Parsers.TestExplorer
{
    public class MethodUsage
    {
        public string AssemblyName { get; set; }
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public List<Test> TestCoverage { get; set; }
    }
}