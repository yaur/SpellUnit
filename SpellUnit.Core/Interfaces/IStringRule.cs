using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellUnit.Interfaces
{
    public interface IStringRule
    {
        string Name { get; }
        bool RunOnType(FragmentType type);
        IEnumerable<string> Validate(string value);
    }
}
