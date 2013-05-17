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
using SpellUnit.Interfaces;

namespace SpellUnit
{
    [Export(typeof(IIgnoredWords))]
    public class DefaultWordDictionary : IIgnoredWords
    {
        #region IIgnoredProperty Members

        static Dictionary<string, bool> ignored = new Dictionary<string, bool>();
        public static void AddIgnoredWords(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                ignored[word] = true;
            }
        }

        public static void Clear()
        {
            ignored = new Dictionary<string, bool>();
        }

        public bool IsIgnored(string word)
        {
            return ignored.ContainsKey(word);
        }

        #endregion
    }
}
