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

using System;
using System.ComponentModel.Composition;
using System.Linq;
using NHunspell;
using SpellUnit;
using SpellUnit.Interfaces;

namespace NHunspellChecker
{
    [Export(typeof(IStringRule))]
    public class NHunspell :IStringRule
    {
        [Import(typeof(IIgnoredWords))]
        IIgnoredWords IgnoredWords { get; set; }

        private static string locale = "en_us";

        public static string Locale {get{ return locale;}set{locale = value;}}

        Hunspell checker = null;

        public NHunspell()
        {
            checker = new Hunspell(locale + ".aff", locale + ".dic");
        }

        #region IStringRule Members

        public string Name
        {
            get { return "NHunspell spell checker"; }
        }

        public bool RunOnType(FragmentType type)
        {
            return true;
        }

        public bool Validate(string value)
        {
            var wordSplitTokens = " \n  .!,\"'".ToArray();
            var words = value.Split(wordSplitTokens, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (IgnoredWords.IsIgnored(word)) continue;
                if(!(checker.Spell(word)))return false;
            }
            return true;
        }

        #endregion
    }
}
