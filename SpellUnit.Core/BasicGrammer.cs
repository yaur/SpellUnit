/*
 * This file is part of SpellUnit.
 *
 *  SpellUnit is free software: you can redistribute it and/or modify
 *  it under the terms of the Lesser GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Foobar is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with SpellUnit.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SpellUnit.Interfaces;

namespace SpellUnit
{
    [Export(typeof(IStringRule))]
    class BasicGrammer : IStringRule
    {
        #region IStringRule Members

        public string Name
        {
            get { return "Basic Grammer"; }
        }

        public bool RunOnType(FragmentType type)
        {
            return (type == FragmentType.Sentence) || (type == FragmentType.Paragraph);
        }

        public IEnumerable<string> Validate(string value)
        {
            // this is documentation... if you end with a quetion mark or other punctuation you are doing it wrong
            if (char.IsUpper(value.First()) && (value.Last() == '.' || value.Last() == '!'))
            {
                return new string[0];
            }
            return new string[1]{value};
        }

        #endregion
    }
}
