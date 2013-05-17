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
using System.Linq;
using System.Reflection;
using ServiceStack.ServiceHost;
using SpellUnit.Interfaces;

namespace SpellUnit.ServiceStackExtractor
{
    [Export(typeof(IStringExtractor))]
    public class ServiceStackExtractor : IStringExtractor
    {

        #region IStringExtractor Members

        public IEnumerable<FragmentDescriptor> Extract(Type type)
        {
            var ssTypes = new Type[] { typeof(ApiMemberAttribute), typeof(ApiResponseAttribute), typeof(RouteAttribute), typeof(ApiAttribute) };
            {
                var attributes = type.GetCustomAttributes(true).Where(x => ssTypes.Contains(x.GetType()));
                foreach (var attribute in attributes)
                {
                    foreach (var desc in AttributeDescriber.DescribeServiceStackAttribute(type, null, attribute))
                    {
                        yield return desc;
                    }
                }
            }
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var attributes = prop.GetCustomAttributes(true).Where(x => ssTypes.Contains(x.GetType()));
                foreach (var attribute in attributes)
                {
                    foreach (var desc in AttributeDescriber.DescribeServiceStackAttribute(type, prop, attribute))
                    {
                        yield return desc;
                    }
                }
            }
        }

        public IEnumerable<FragmentDescriptor> Extract(System.Reflection.Assembly assembly)
        {
            return new List<FragmentDescriptor>();
        }

        #endregion
    }
}
