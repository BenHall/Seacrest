using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Seacrest.Analyser.Parsers.UnitTests
{
    public class UnitTestFinder
    {
        public IEnumerable<MethodUsage> FindUsagesViaTests(string assembly)
        {
            List<MethodUsage> methodUsages = new List<MethodUsage>();
            ModuleDefinition testAssembly = ModuleDefinition.ReadModule(assembly);

            foreach (var type in testAssembly.Types)
            {
                foreach (var method in type.Methods)
                {
                    foreach (var instruction in method.Body.Instructions.Where(IsMethodCall))
                    {
                        var methodUsage = CreateUsage(instruction.Operand as MemberReference, testAssembly, type, method);
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

        private MethodUsage CreateUsage(MemberReference operand, ModuleDefinition assembly, TypeDefinition type, MethodDefinition method)
        {
            if (operand == null)
                return null;

            UnitTest test = new UnitTest
                {
                    AssemblyName = assembly.Assembly.Name.FullName,
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
                    TestCoverage = new List<UnitTest> {test}
                };

            return instructionCall;
        }
    }
}