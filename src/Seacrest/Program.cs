using System;
using Seacrest.Analyser;
using Seacrest.Analyser.Execution;
using Seacrest.Analyser.Parsers.Differs;
using Seacrest.Analyser.Parsers.TestExplorer;
using Seacrest.Analyser.Watcher;

namespace Seacrest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "";
            CodeChangeWatcher watcher = new CodeChangeWatcher();
            CoreApp app = new CoreApp(path);
            
            watcher.CodeChanged += app.CodeChanged;
            watcher.Watch(path);
        }
    }

    public class CoreApp
    {
        private readonly string _pathBeingWatched;
        string outDir = null;
        string oldAssembly = null;
        string newAssembly = null;
        private string solution = null;

        public CoreApp(string pathBeingWatched)
        {
            _pathBeingWatched = pathBeingWatched;
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
                var findUsagesViaTests = finder.FindUsagesViaTests(newAssembly);

                ChangedMethodsFilter filter = new ChangedMethodsFilter();
                var findUnitTestsAffectedByChanges = filter.FindUnitTestsAffectedByChanges(findModifiedMethods, findUsagesViaTests);

                GallioTestRunner runner = new GallioTestRunner();
                var testExecutionResults = runner.Execute(findUnitTestsAffectedByChanges);

                Console.WriteLine(testExecutionResults.ExecutionResult);
                Console.WriteLine(testExecutionResults.Passed);
            }
        }
    }
}
