﻿namespace Seacrest.Analyser.Parsers.UnitTests
{
    public class MethodUsage
    {
        public string AssemblyName { get; set; }
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public UnitTest Test { get; set; }
    }
}