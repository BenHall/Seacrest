using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Seacrest.Analyser.Parsers.TestExplorer
{
    public class TestFinder
    {
        public IEnumerable<MethodUsage> FindUsagesViaTests(string pathToAssembly)
        {
            List<MethodUsage> methodUsages = new List<MethodUsage>();
            ModuleDefinition testAssembly = ModuleDefinition.ReadModule(pathToAssembly);

            foreach (var type in testAssembly.Types)
            {
                foreach (var method in type.Methods)
                {
                    foreach (var instruction in method.Body.Instructions.Where(IsMethodCall))
                    {
                        var methodUsage = CreateUsage(instruction.Operand as MemberReference, testAssembly, type, method, pathToAssembly);
                        var existingUsage = methodUsages.SingleOrDefault(x => x.MethodName == methodUsage.MethodName && x.ClassName == methodUsage.ClassName);

                        if (existingUsage != null)
                            existingUsage.TestCoverage.AddRange(methodUsage.TestCoverage);
                        else
                            methodUsages.Add(methodUsage);
                    }
                }
            }

            return methodUsages;
        }
        
        private bool IsMethodCall(Instruction instruction)
        {
            return instruction.OpCode.FlowControl == FlowControl.Call && instruction.OpCode.Code == Code.Callvirt;
        }

        private MethodUsage CreateUsage(MemberReference operand, ModuleDefinition assembly, TypeDefinition type, MethodDefinition method, string testAssemblyPath)
        {
            if (operand == null)
                return null;

            Test test = new Test
                {
                    PathToAssembly = Path.GetDirectoryName(testAssemblyPath),
                    AssemblyName = assembly.Assembly.Name.Name,
                    NamespaceName = type.Namespace,
                    ClassName = type.Name,
                    MethodName = method.Name
                };

            var instructionCall = new MethodUsage
                {
                    AssemblyName = operand.DeclaringType.Scope.Name + ".dll",
                    NamespaceName = operand.DeclaringType.Namespace,
                    ClassName = operand.DeclaringType.Name,
                    MethodName = operand.Name,
                    TestCoverage = new List<Test> {test}
                };

            return instructionCall;
        }
    }
}