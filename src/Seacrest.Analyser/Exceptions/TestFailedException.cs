using System;

namespace Seacrest.Analyser.Exceptions
{
    internal class TestFailedException : Exception
    {
        public TestFailedException(string message) : base(message) { }
    }
}