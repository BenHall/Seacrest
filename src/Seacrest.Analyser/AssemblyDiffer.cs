using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace Seacrest.Analyser
{
    public class AssemblyDiffer : IAssemblyDiffer
    {
        public IEnumerable<ChangedMethod> FindModifiedMethods(string baseAssemblyPath, string compareToAssemblyPath)
        {
            List<ChangedMethod> changedMethods = new List<ChangedMethod>();
            var classesInNewAssembly = ModuleDefinition.ReadModule(compareToAssemblyPath);
            var classesInOldAssembly = ModuleDefinition.ReadModule(baseAssemblyPath);

            foreach (TypeDefinition classInUpdatedAssembly in classesInNewAssembly.Types)
            {
                TypeDefinition oldClass = FindOldClass(classesInOldAssembly, classInUpdatedAssembly);
                foreach (var updatedMethod in classInUpdatedAssembly.Methods)
                {
                    var method = DetermineIfMethodHasChanged(oldClass, updatedMethod);
                    if(method != null && method.MethodName != ".ctor")
                        changedMethods.Add(method);
                }
            }

            return changedMethods;
        }

        private ChangedMethod DetermineIfMethodHasChanged(TypeDefinition oldClass, MethodDefinition updatedMethod)
        {
            ChangedMethod changedMethod = null;

            MethodDefinition oldMethodBody = FindOldMethodBody(oldClass, updatedMethod);
            if (oldMethodBody == null)
                changedMethod = GetChangedMethod(updatedMethod, ChangeReason.New);

            else if (HasChanged(updatedMethod, oldMethodBody))
            {
                changedMethod = GetChangedMethod(updatedMethod, ChangeReason.Updated);
            }

            return changedMethod;
        }

        private TypeDefinition FindOldClass(ModuleDefinition oldModules, TypeDefinition typeDefinition)
        {
            return oldModules.Types.SingleOrDefault(t => t.Name == typeDefinition.Name);
        }

        private MethodDefinition FindOldMethodBody(TypeDefinition oldType, MethodDefinition methodDefinition)
        {
            MethodDefinition oldMethodBody = null;
            if (oldType != null)
                oldMethodBody = oldType.Methods.SingleOrDefault(m => m.Name == methodDefinition.Name);
            return oldMethodBody;
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

                if (newInstruction.Operand != null && oldInstruction.Operand != null)
                {
                    if (!newInstruction.Operand.ToString().Equals(oldInstruction.Operand.ToString()))
                        return true;
                }
            }

            return false;
        }
    }
}