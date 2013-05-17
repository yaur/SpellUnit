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
using System.Linq;
using System.Reflection;

namespace SpellUnit
{
    class Program
    {
        static int failCount = 0;
        static void Main(string[] args)
        {
            Assembly[] toTest = args.Select(x => Assembly.LoadFrom(x)).ToArray();

            Assembly.LoadFrom("SpellUnit.ServiceStackExtractor.dll");
            Assembly.LoadFrom("SpellUnit.NHunspell.dll");
            var checker = new SpellChecker();
            checker.Compose();
            DefaultWordDictionary.AddIgnoredWords(new String[] { "IP", "NOC" });
            checker.Validate(ValidationFailed, toTest);
            Console.WriteLine("{0} failures", failCount);
        }
        static void ValidationFailed(FragmentDescriptor desc,Interfaces.IStringRule rule)
        {
            Console.WriteLine("check on value \"{0}\" failed rule \"{1}\"\nvalue:{2}", desc.Name, rule.Name, desc.Value);
            failCount++;
        }
    }
}
