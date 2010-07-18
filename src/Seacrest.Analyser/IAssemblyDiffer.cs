using System.Collections.Generic;

namespace Seacrest.Analyser
{
    public interface IAssemblyDiffer
    {
        IEnumerable<ChangedMethod> FindModifiedMethods(string baseAssemblyPath, string compareToAssemblyPath);
    }
}