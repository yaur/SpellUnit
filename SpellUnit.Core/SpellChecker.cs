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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SpellUnit.Interfaces;

namespace SpellUnit
{
    public class SpellChecker
    {
        [ImportMany(typeof(IStringRule))]
        List<IStringRule> rules;

        [ImportMany(typeof(IStringExtractor))]
        List<IStringExtractor> extractors;

        [Import(typeof(IIgnoredProperty))]
        IIgnoredProperty propertyIgnorer;

        public void Compose()
        {
            Compose(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Compose(params Assembly[] assemblies)
        {
            var catalog = new AggregateCatalog(assemblies.Select(x => new AssemblyCatalog(x)));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        public void Validate(Action<FragmentDescriptor, IStringRule> onValidationFailed, params Assembly[] Assemblies)
        {
            var types = new List<Type>();
            var assemblies = new List<Assembly>();
            var fragments = new List<FragmentDescriptor>();
            foreach (var assembly in Assemblies)
            {
                types.AddRange(assembly.GetTypes());
                assemblies.Add(assembly);
            }

            Parallel.ForEach(assemblies, x =>
            {
                var lclStrings = extractors.SelectMany(extract => extract.Extract(x)).Where(y=>!propertyIgnorer.IsIgnored(y.Name));
                if (lclStrings.Any())
                {
                    lock (fragments)
                    {
                        fragments.AddRange(lclStrings);
                    }
                }
            });

            List<FailureResult> results = new List<FailureResult>();
            Parallel.ForEach(fragments,
            x =>
            {
                var lclResults = rules
                            .Where(rule => rule.RunOnType(x.FragementType))
                            .Where(rule => !rule.Validate(x.Value))
                            .Select(rule => new FailureResult { Value = x, Rule = rule }).ToList(); ;
                if (lclResults.Any())
                {
                    lock (results)
                    {
                        results.AddRange(lclResults);
                    }
                }

            });
            foreach (var result in results)
            {
                onValidationFailed(result.Value, result.Rule);
            }
            Validate(onValidationFailed, types.ToArray());
        }

        public void Validate(Action<FragmentDescriptor, IStringRule> onValidationFailed, params Type[] Types)
        {
            var fragments = new List<FragmentDescriptor>();
            Parallel.ForEach(Types, x =>
            {
                var lclStrings = extractors.SelectMany(extract => extract.Extract(x)).Where(y => !propertyIgnorer.IsIgnored(y.Name));
                if (lclStrings.Any())
                {
                    lock (fragments)
                    {
                        fragments.AddRange(lclStrings);
                    }
                }
            });
            List<FailureResult> results = new List<FailureResult>();
            Parallel.ForEach(fragments,
#if DEBUG
            new ParallelOptions { MaxDegreeOfParallelism = 1 },
#endif
            x =>
            {
                var lclResults = rules
                        .Where(rule => rule.RunOnType(x.FragementType))
                        .Where(rule => !rule.Validate(x.Value))
                        .Select(rule => new FailureResult { Value = x, Rule = rule }).ToList(); ;
                if (lclResults.Any())
                {
                    lock (results)
                    {
                        results.AddRange(lclResults);
                    }
                }

            });
            foreach (var result in results)
            {
                onValidationFailed(result.Value, result.Rule);
            }
        }
    }
}
