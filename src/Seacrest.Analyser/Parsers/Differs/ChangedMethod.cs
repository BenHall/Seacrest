namespace Seacrest.Analyser.Parsers.Differs
{
    public class ChangedMethod
    {
        public ChangeReason ChangeReason { get; set; }
        public string AssemblyName { get; set; }
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}