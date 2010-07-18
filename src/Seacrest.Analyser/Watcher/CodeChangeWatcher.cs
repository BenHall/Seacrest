using System;
using System.IO;

namespace Seacrest.Analyser.Watcher
{
    public class CodeChangeWatcher
    {
        private FileSystemWatcher watcher;

        public delegate void CodeChangedHandler(object sender, CodeChangedEventArgs e);
        public event CodeChangedHandler CodeChanged;

        public void Watch(string path)
        {
            watcher = new FileSystemWatcher(path);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite 
                                    | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.cs";

            watcher.Changed += fileChanged;
            watcher.Renamed += fileRenamed;

            watcher.EnableRaisingEvents = true;
        }

        private void OnCodeChanged(CodeChangedEventArgs e)
        {
            CodeChangedHandler handler = CodeChanged;
            if (handler != null) 
                handler(this, e);
        }

        private void fileRenamed(object sender, RenamedEventArgs e)
        {
            BlockEvents(() => OnCodeChanged(new CodeChangedEventArgs{FullPath = e.FullPath, FileName = e.Name}));
        }

        private void fileChanged(object sender, FileSystemEventArgs e)
        {
            BlockEvents(() => OnCodeChanged(new CodeChangedEventArgs { FullPath = e.FullPath, FileName = e.Name }));
        }

        private void BlockEvents(Action action)
        {
            watcher.EnableRaisingEvents = false;
            action();
            watcher.EnableRaisingEvents = true;
        }
    }
}