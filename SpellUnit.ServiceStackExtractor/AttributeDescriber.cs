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
using System.Text;
using System.Threading.Tasks;
using ServiceStack.ServiceHost;
using SpellUnit;

namespace SpellUnit.ServiceStackExtractor
{
    public static class AttributeDescriber
    {
        internal static IEnumerable<FragmentDescriptor> DescribeServiceStackAttribute(Type type, PropertyInfo info, object attribute)
        {
            var attrtype = attribute.GetType().Name;

            var baseName = info == null ?
                string.Format("{0}|{1}.", type.Name, attrtype) :
                string.Format("{0}.{1}|{2}.", type.Name, info.Name, attrtype);

            var propertyNames = new string[] { "Summary", "Description", "Notes" };
            var props = attribute.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => propertyNames.Contains(x.Name));

            foreach (var prop in props)
            {
                string value = (string)prop.GetValue(attribute);
                if (!string.IsNullOrEmpty(value))
                {
                    yield return new FragmentDescriptor
                    {
                        Name = baseName + prop.Name,
                        FragementType = FragmentType.Sentence,
                        Value = value
                    };

                }

            }
        }
    }
}
