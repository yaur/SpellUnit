using SpellUnit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.ComponentModel.Composition;

namespace SpellUnit.ExceptionLiteralsExtractor
{
    [Export(typeof(IStringExtractor))]
    public class ExceptionLiteralsExtractor : IStringExtractor
    {
        public IEnumerable<FragmentDescriptor> Extract(Type type)
        {
            // this is dumb but if cecil will let us get from BCL reflection primitives
            // straight to their cecil equivilants it isn't well documented
            AssemblyDefinition monoAssembly = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            
            var monoType = monoAssembly.Modules.SelectMany(x => x.GetTypes()).Where(x => x.FullName == type.FullName).Single();
            var methods = monoType.Methods;
            List<FragmentDescriptor> retv = new List<FragmentDescriptor>();
            foreach (var method in methods)
            {
                var item = ValidateMethod(method);
                retv.AddRange(item);
            }
            return retv;
        }

        private IEnumerable<FragmentDescriptor> ValidateMethod(MethodDefinition method)
        {
            var body = method.Body;
            string arg = string.Empty;
            var formatString = method.FullName + " exception string:{0}";
            int count = 0;
            foreach (var instr in body.Instructions)
            {
                if (instr.OpCode.Code == Code.Ldstr)
                {
                    // we have just loaded a string literal
                    arg = (string)instr.Operand;
                    continue;
                }
                if (instr.OpCode.Code == Code.Newobj)
                {
                    // we are newing up some object, we are only interested in
                    // new objects if they derive from System.Exception
                    var methRef = (Mono.Cecil.MethodReference)instr.Operand;
                    var type = Type.GetType(methRef.DeclaringType.FullName);
                    
                    if (arg != String.Empty && typeof(System.Exception).IsAssignableFrom(type))
                    {
                        yield return new FragmentDescriptor
                        {
                            FragementType = FragmentType.Sentence,
                            Name = string.Format(formatString, count++),
                            Value = arg
                        };
                    }
                    // at this point we have either:
                    // returned our result
                    // loaded a string and not newed an exception
                    // not loaded a string
                    // in any case we want to reset and continue
                    arg = string.Empty;
                }
            }
        }

        public IEnumerable<FragmentDescriptor> Extract(System.Reflection.Assembly assembly)
        {
            // not DRY, but the alternative is to rescan the assembly on each type, blech
            AssemblyDefinition monoAssembly = AssemblyDefinition.ReadAssembly(assembly.Location);
            var methods = monoAssembly.Modules.SelectMany(x => x.GetTypes()).SelectMany(x=>x.Methods);
            List<FragmentDescriptor> retv = new List<FragmentDescriptor>();
            foreach (var method in methods)
            {
                var item = ValidateMethod(method);
                if (item != null) retv.AddRange(item);
            }
            return retv;
        }
    }
}
