namespace Seacrest.Analyser
{
    public class ChangedMethod
    {
        public ChangedContentStatus Status { get; set; }
        public string AssemblyName { get; set; }
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }
}