using System;
using Seacrest.Analyser;
using Seacrest.Analyser.Execution;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Parsers.TestExplorer;
using Seacrest.Analyser.Watcher;
using System.IO;

namespace Seacrest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\SourceControl\Seacrest\example\UnitTesting1\UnitTesting1";
            CodeChangeWatcher watcher = new CodeChangeWatcher();
            CoreApp app = new CoreApp();
            
            watcher.CodeChanged += app.CodeChanged;
            watcher.Watch(path);

            Console.WriteLine("Starting...");
            Console.ReadLine();
        }
    }

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
            SolutionBuilder builder = new SolutionBuilder();
            var built = builder.Build(solution, outDir);

            if (built)
            {
                AssemblyDiffer differ = new AssemblyDiffer();
                var findModifiedMethods = differ.FindModifiedMethods(oldAssembly, newAssembly);

                TestFinder finder = new TestFinder();
                var findUsagesViaTests = finder.FindUsagesViaTests(testAssembly);

                ChangedMethodsFilter filter = new ChangedMethodsFilter();
                var findUnitTestsAffectedByChanges = filter.FindUnitTestsAffectedByChanges(findModifiedMethods, findUsagesViaTests);

                GallioTestRunner runner = new GallioTestRunner();
                var testExecutionResults = runner.Execute(findUnitTestsAffectedByChanges);

                if (testExecutionResults != null)
                    OutputResults(testExecutionResults);

                MoveAssemblyToOld(oldOutDir, outDir);
            }
        }

        private void OutputResults(TestExecutionResults testExecutionResults)
        {
            Console.WriteLine(testExecutionResults.ExecutionResult);
            Console.WriteLine(testExecutionResults.Passed);
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
