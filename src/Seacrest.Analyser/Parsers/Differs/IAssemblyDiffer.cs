using System.Collections.Generic;

namespace Seacrest.Analyser.Parsers.Differs
{
    public interface IAssemblyDiffer
    {
        IEnumerable<ChangedMethod> FindModifiedMethods(string baseAssemblyPath, string compareToAssemblyPath);
    }
}