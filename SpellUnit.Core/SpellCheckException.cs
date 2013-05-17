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
using System.Text;

namespace SpellUnit
{
    public class SpellCheckException :Exception
    {
        public SpellCheckException(List<FailureResult> failures) {
            if (failures.Count == 1)
            {
                this.message = string.Format("Value {0} failed rule {1}", failures[0].Value.Name, failures[0].Rule.Name);
            }
            else
            {
                var results = failures.GroupBy(x=> x.Rule).Select(x=>
                    new
                    {
                        Name = x.Key.Name,
                        Count = x.Count()
                    });
                StringBuilder sb = new StringBuilder();
                foreach (var result in results)
                {
                    sb.Append(string.Format("{0} items failed rule \"{1}\"\n", result.Count, result.Name));
                }
                this.message = sb.ToString();

            }
        }

        private string message;

        public override string Message
        {
            get
            {
                return message;
            }
        }
    }
}
