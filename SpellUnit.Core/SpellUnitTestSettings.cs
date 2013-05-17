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
using System.Xml.Serialization;

namespace SpellUnit
{
    public class SpellUnitTestSettings
    {
        public SpellUnitTestSettings()
        {
            IgnoredWords = new List<string>();
            IgnoredProperties = new List<string>();
            CompositionAssemblies = new List<string>();
        }
        public List<string> IgnoredWords { get; set; }
        public List<string> IgnoredProperties { get; set; }
        public List<string> CompositionAssemblies { get; set; }
        public string AssemblyUnderTest { get; set; }
        public string TypeUnderTest { get; set; }

        private string fileName;
        public static SpellUnitTestSettings FromFile(string FileName)
        {
            //
            var ser = new XmlSerializer(typeof(SpellUnitTestSettings));
            using (var file = System.IO.File.OpenRead(FileName))
            {
                var retv = (SpellUnitTestSettings)ser.Deserialize(file);
                retv.fileName = FileName;
                return retv;
            }
        }

        public void Save()
        {
            Save(fileName);
        }

        public void Save(string FileName)
        {
            var ser = new XmlSerializer(typeof(SpellUnitTestSettings));
            using (var file = System.IO.File.Create(FileName))
            {
                ser.Serialize(file, this);
            }
        }
    }
}
