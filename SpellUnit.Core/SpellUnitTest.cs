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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpellUnit
{
    public class SpellUnitTest
    {
        public Assembly AssemblyUnderTest { get; set; }
        public Type TypeUnderTest { get; set; }
        public IEnumerable<string> IgnoredWords { get; set; }
        public IEnumerable<string> IgnoredProperties { get; set; }
        public IEnumerable<Assembly> CompositionSources { get; set; }

        public SpellUnitTest() { }
        public SpellUnitTest(SpellUnitTestSettings settings)
        {
            if (settings.AssemblyUnderTest != null)
            {
                this.AssemblyUnderTest = Assembly.LoadFrom(settings.AssemblyUnderTest);
            }
            if (settings.TypeUnderTest != null)
            {
                this.TypeUnderTest = Type.GetType(settings.TypeUnderTest);
            }
            if (settings.CompositionAssemblies != null)
            {
                this.CompositionSources = settings.CompositionAssemblies.Select(x => Assembly.LoadFrom(x));
            }
            this.IgnoredWords = settings.IgnoredWords;
            this.IgnoredProperties = settings.IgnoredProperties;
        }
        public void Run()
        {
            DefaultWordDictionary.Clear();
            if (IgnoredWords != null)
            {
                DefaultWordDictionary.AddIgnoredWords(IgnoredWords);
            }
            DefaultPropertyDictionary.Clear();
            if (IgnoredProperties != null)
            {
                DefaultPropertyDictionary.AddIgnoredWords(IgnoredProperties);
            }
            SpellChecker chkr = new SpellChecker();
            if (CompositionSources == null)
            {
                chkr.Compose();
            }
            else
            {
                chkr.Compose(CompositionSources.ToArray());
            }
            List<FailureResult> results = new List<FailureResult>();
            if (AssemblyUnderTest != null)
            {
                chkr.Validate((d, r) => results.Add(new FailureResult { Value = d, Rule = r }), AssemblyUnderTest);
            }
            if (TypeUnderTest != null)
            {
                chkr.Validate((d, r) => results.Add(new FailureResult { Value = d, Rule = r }), TypeUnderTest);
            }
            if (results.Any())
            {
                throw new SpellCheckException(results);
            }

        }
    }
}
