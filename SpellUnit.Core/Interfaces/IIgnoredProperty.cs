using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellUnit.Interfaces
{
    public interface IIgnoredProperty
    {
        bool IsIgnored(string word);
    }
}
