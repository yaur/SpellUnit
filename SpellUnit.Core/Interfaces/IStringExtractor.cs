using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpellUnit;

namespace SpellUnit.Interfaces
{
    public interface IStringExtractor
    {
        IEnumerable<FragmentDescriptor> Extract(Type type);
        IEnumerable<FragmentDescriptor> Extract(Assembly assembly);
    }
}
