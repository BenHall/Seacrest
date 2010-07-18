using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Seacrest.Analyser.Parsers.Differs;

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
                    methodUsages.AddRange(from instruction in method.Body.Instructions
                                          where IsMethodCall(instruction)
                                          select CreateUsage(instruction.Operand as MemberReference, testAssembly, type, method));
                }
            }

            return methodUsages;
        }

        //private IEnumerable<InstructionCall> FindAllMethodsCalledByUnitTests(string path)
        //{
        //    List<InstructionCall> instructionsExecuted = new List<InstructionCall>();

        //    var assembly = path + @"\UnitTesting1.Tests.dll";
        //    ModuleDefinition testAssembly = ModuleDefinition.ReadModule(assembly);

        //    foreach (var type in testAssembly.Types)
        //    {
        //        foreach (var method in type.Methods)
        //        {
        //            foreach (var instruction in method.Body.Instructions)
        //            {
        //                if (!IsMethodCall(instruction))
        //                    continue;

        //                InstructionCall instructionCall = GetInstructionCall(instruction.Operand as MemberReference);
        //                InstructionCall existingInstruction = instructionsExecuted.SingleOrDefault(x => x.Equals(instructionCall));

        //                UnitTest test = new UnitTest
        //                {
        //                    AssemblyPath = assembly,
        //                    AssemblyName = testAssembly.Assembly.Name.FullName,
        //                    MethodName = method.Name,
        //                    ClassName = type.Name,
        //                    NamespaceName = type.Namespace
        //                };

        //                if (existingInstruction == null)
        //                {
        //                    instructionCall.UsedInTests.Add(test);
        //                    instructionsExecuted.Add(instructionCall);
        //                }
        //                else
        //                {
        //                    existingInstruction.UsedInTests.Add(test);
        //                }
        //            }
        //        }
        //    }
        //    return instructionsExecuted;
        //}

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
                    Test = test
                };

            return instructionCall;
        }
    }
}