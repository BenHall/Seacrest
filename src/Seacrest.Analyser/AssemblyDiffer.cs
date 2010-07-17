using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace Seacrest.Analyser
{
    public class AssemblyDiffer
    {
        public IEnumerable<ChangedMethod> FindModifiedMethods(string baseAssemblyPath, string compareToAssemblyPath)
        {
            List<ChangedMethod> changedMethods = new List<ChangedMethod>();
            var newModules = ModuleDefinition.ReadModule(compareToAssemblyPath);
            var oldModules = ModuleDefinition.ReadModule(baseAssemblyPath);

            foreach (TypeDefinition typeDefinition in newModules.Types)
            {
                var oldType = oldModules.Types.SingleOrDefault(t => t.Name == typeDefinition.Name);
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    var oldMethodBody = oldType.Methods.SingleOrDefault(m => m.Name == methodDefinition.Name);

                    if (oldMethodBody == null)
                        changedMethods.Add(GetChangedMethod(methodDefinition, ChangeReason.New));
                    else if (HasChanged(methodDefinition, oldMethodBody))
                        changedMethods.Add(GetChangedMethod(methodDefinition, ChangeReason.Updated));
                }
            }

            return changedMethods;
        }

        private static ChangedMethod GetChangedMethod(MemberReference operand, ChangeReason changeStatus)
        {
            var changedMethod = new ChangedMethod();
            changedMethod.ChangeReason = changeStatus;
            changedMethod.AssemblyName = operand.DeclaringType.Scope.Name;
            changedMethod.NamespaceName = operand.DeclaringType.Namespace;
            changedMethod.ClassName = operand.DeclaringType.Name;
            changedMethod.MethodName = operand.Name;
            return changedMethod;
        }

        private bool HasChanged(MethodDefinition methodDefinition, MethodDefinition oldMethodBody)
        {
            for (int index = 0; index < methodDefinition.Body.Instructions.Count; index++)
            {
                var newInstruction = methodDefinition.Body.Instructions[index];
                if (oldMethodBody.Body.Instructions.Count() == index)
                    return true;

                var oldInstruction = oldMethodBody.Body.Instructions[index];

                if (newInstruction.Operand != null || oldInstruction.Operand != null)
                {
                    if (!newInstruction.Operand.ToString().Equals(oldInstruction.Operand.ToString()))
                        return true;
                }
            }

            return false;
        }
    }
}