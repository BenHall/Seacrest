namespace Seacrest.Analyser.Watcher
{
    public class CodeChangedEventArgs
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
    }
}