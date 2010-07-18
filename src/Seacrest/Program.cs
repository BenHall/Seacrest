using System;
using Seacrest.Analyser.Watcher;

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
}
