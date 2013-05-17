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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceHost;

namespace SpellUnitBasicTests
{

    public class BadlySpelledClass
    {
        [ApiMember(Description="This property is missspelled.")]
        public string AProperty { get; set; }
    }

    public class GoodlySpelledClass
    {
        [ApiMember(Description = "This property is not.")]
        public string AProperty { get; set; }
    }

    public class BadGrammerClass
    {
        [ApiMember(Description = "This property is missing something")]
        public string AProperty { get; set; }
    }

    [TestClass]
    [DeploymentItem("en_US.aff")]
    [DeploymentItem("en_US.dic")]
    [DeploymentItem("Hunspellx64.dll")]
    [DeploymentItem("Hunspellx86.dll")]
    [DeploymentItem("DefaultSpellUnit.testsettings.xml")]
    [DeploymentItem("IgnoreProperty.testsettings.xml")]
    public class UsageDemo
    {
        public UsageDemo()
        {
            var force = new SpellUnit.ServiceStackExtractor.ServiceStackExtractor();
            var force2 = new NHunspellChecker.NHunspell();
        }
        [TestMethod]
        
        public void SunnyDay()
        {
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(GoodlySpelledClass);

            // shouldn't throw
            runner.Run();
        }

        [TestMethod]
        public void BadlySpelledClass()
        {
            bool exceptionThrown = false;
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadlySpelledClass);

            try
            {
                runner.Run();
            }
            catch (SpellUnit.SpellCheckException ex)
            {
                Assert.AreEqual("Value BadlySpelledClass.AProperty|ApiMemberAttribute.Description failed rule NHunspell spell checker", ex.Message);
                exceptionThrown = true;
            }
            Assert.AreEqual(true, exceptionThrown);
        }

        [TestMethod]
        public void BadGrammerClass()
        {
            bool exceptionThrown = false;
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadGrammerClass);

            try
            {
                runner.Run();
            }
            catch (SpellUnit.SpellCheckException ex)
            {
                Assert.AreEqual("Value BadGrammerClass.AProperty|ApiMemberAttribute.Description failed rule Basic Grammer", ex.Message);
                exceptionThrown = true;
            }
            Assert.AreEqual(true, exceptionThrown);
        }

        [TestMethod]
        public void IgnoredPropertyTest()
        {
            // in this test we have manually validate a property and added it to the ignore list
            bool exceptionThrown = false;
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("IgnoreProperty.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadlySpelledClass);
            runner.Run();
            
        }

    }
}
