namespace Seacrest.Analyser.Execution
{
    public class TestExecutionResults
    {
        public int Run { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public TestExecutionResult ExecutionResult { get; set; }
        public string Output { get; set; }
    }
}