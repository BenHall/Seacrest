using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Seacrest.Analyser.Tests.Builders
{
    public class AssemblyBuilder
    {
        private string _assemblyName;
        private Dictionary<string, Dictionary<string, string>> _classes = new Dictionary<string, Dictionary<string, string>>();
        private string[] _references;

        public AssemblyBuilder AssemblyName(string assembyName)
        {
            _assemblyName = assembyName;
            return this;
        }

        public AssemblyBuilder WithClassAndMethods(string className, Dictionary<string, string> methods)
        {
            _classes.Add(className, methods);
            return this;
        }

        public AssemblyBuilder References(params string[] references)
        {
            _references = references;
            return this;
        }

        public AssemblyBuilderResult Build()
        {
            List<string> files = WriteClasses();
            string assemblyPath;

            try
            {
                assemblyPath = BuildAssembly(files);
            }
            finally
            {
                foreach (var file in files)
                    File.Delete(file);
            }

            return new AssemblyBuilderResult {Path = assemblyPath};
        }

        private List<string> WriteClasses()
        {
            List<string> files = new List<string>();
            foreach (var cls in _classes)
            {
                var path = Path.GetTempFileName() + ".cs";
                files.Add(path);
                StreamWriter writer = new StreamWriter(path);
                writer.WriteLine("public class " + cls.Key);
                writer.WriteLine("{");
                foreach (var method in cls.Value)
                {
                    writer.WriteLine("\tpublic void " + method.Key + "()");
                    writer.WriteLine("\t{");
                    writer.WriteLine("\t\t" + method.Value);
                    writer.WriteLine("\t}");

                }
                writer.WriteLine("}");
                writer.Close();
            }
            return files;
        }

        private string BuildAssembly(List<string> files)
        {
            string csc = @"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe";
            var baseOutputPath = Path.GetTempPath();
            string assembly = Path.Combine(baseOutputPath, _assemblyName + ".dll");

            StringBuilder builder = new StringBuilder();
            if (_references != null)
                foreach (var reference in _references)
                    builder.AppendFormat(" /reference:{0} ", reference);

            foreach (var file in files)
                builder.AppendFormat(" \"{0}\" ", file);

            string args = "/nologo /target:library /out:\"" + assembly + "\"" + builder;

            ProcessStartInfo startInfo = new ProcessStartInfo(csc, args);
            startInfo.WorkingDirectory = baseOutputPath;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);

            if(process.ExitCode != 0)
                throw new InvalidOperationException("Build failed.");

            return assembly;
        }
    }
}
