namespace Seacrest.Analyser
{
    public interface ISolutionBuilder
    {
        bool Build(string pathToSolution, string outDir);
    }
}