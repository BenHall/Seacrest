using System;

namespace Seacrest.Analyser.Exceptions
{
    internal class BuildFailedException : Exception
    {
        public BuildFailedException(string output) : base(output)
        { }
    }
}