using System;
using System.IO;
using Seacrest.Analyser;
using Seacrest.Analyser.Execution;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Parsers.TestExplorer;
using Seacrest.Analyser.Watcher;

namespace Seacrest
{
    public class CoreApp
    {
        string outDir = @"C:\temp\seacrest\outDir\_latest\\";
        string oldOutDir = @"C:\temp\seacrest\outDir\_old\\";
        private string solution = @"D:\SourceControl\Seacrest\example\UnitTesting1\UnitTesting1.sln";
        string oldAssembly;
        string newAssembly;
        private string testAssembly;

        public CoreApp()
        {
            newAssembly = Path.Combine(outDir, "UnitTesting1.dll");
            oldAssembly = Path.Combine(oldOutDir, "UnitTesting1.dll");
            testAssembly = Path.Combine(outDir, "UnitTesting1.Tests.dll");
        }

        public void CodeChanged(object sender, CodeChangedEventArgs e)
        {
            Console.WriteLine("Detected a code change");
            SolutionBuilder builder = new SolutionBuilder();
            var built = builder.Build(solution, outDir);

            if (built)
            {
                var testExecutionResults = ProcessNewlyBuiltAssembly();

                if (testExecutionResults != null)
                    OutputResults(testExecutionResults);
                else
                    Console.WriteLine("Nothing has changed...");
            }
        }

        private TestExecutionResults ProcessNewlyBuiltAssembly()
        {
            AssemblyDiffer differ = new AssemblyDiffer();
            var findModifiedMethods = differ.FindModifiedMethods(oldAssembly, newAssembly);

            TestFinder finder = new TestFinder();
            var findUsagesViaTests = finder.FindUsagesViaTests(testAssembly);

            ChangedMethodsFilter filter = new ChangedMethodsFilter();
            var findUnitTestsAffectedByChanges = filter.FindUnitTestsAffectedByChanges(findModifiedMethods, findUsagesViaTests);

            GallioTestRunner runner = new GallioTestRunner();
            var testExecutionResults = runner.Execute(findUnitTestsAffectedByChanges);

            MoveAssemblyToOld(oldOutDir, outDir);

            return testExecutionResults;
        }

        private void OutputResults(TestExecutionResults testExecutionResults)
        {
            Console.ResetColor();
            switch (testExecutionResults.ExecutionResult)
            {
                case TestExecutionResult.Passed:
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case TestExecutionResult.Failed:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }

            Console.Write("Result of execution: " + testExecutionResults.ExecutionResult);
            Console.Write(" Run: " + testExecutionResults.Run);
            Console.Write(" Passed: " + testExecutionResults.Passed);
            Console.Write(" Failed: " + testExecutionResults.Failed);
            Console.Write(" Skipped: " + testExecutionResults.Skipped);
            Console.WriteLine();

            Console.ResetColor();
        }

        private void MoveAssemblyToOld(string oldDir, string latestDir)
        {
            foreach (var file in Directory.GetFiles(oldDir))
            {
                File.Delete(file);
            }

            foreach (var file in Directory.GetFiles(latestDir))
            {
                File.Copy(file, Path.Combine(oldDir, Path.GetFileName(file)));
            }
        }
    }
}