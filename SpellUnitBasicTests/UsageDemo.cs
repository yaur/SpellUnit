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

    class BadExceptionClass
    {
        private static T DataAccessMethod<T>(T value) where T: new()
        {
            if (value == null)
            {
                throw new System.InvalidOperationException("I canr spell.");
            }
            return value;
        }
    }

    class GoodExceptionClass
    {
        private static T DataAccessMethod<T>(T value) where T : new()
        {
            if (value == null)
            {
                throw new System.InvalidOperationException("I can spell.");
            }
            return value;
        }
    }

    class BadExceptionEvenInClosuresClass
    {
        private static  int DataAccessMethod(int value)
        {
            int r = 0;
            System.Threading.ManualResetEvent evt = new System.Threading.ManualResetEvent(false);
            var workItem = System.Threading.ThreadPool.QueueUserWorkItem((_) =>
                {
                    if (r == 0)
                    {
                        throw new System.InvalidOperationException("I canr spell.");
                    }
                    r = value * value;
                    evt.Set();
                });
            evt.WaitOne();
            return r;
        }
    }

    class BadExceptionInPropertyClass
    {
        public bool CanSpell
        {
            get
            {
                throw new System.InvalidOperationException("I canr spell.");
            }
        }
    }
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
            // these are needed so that we don't outsmart MEF
            var force = new SpellUnit.ServiceStackExtractor.ServiceStackExtractor();
            var force2 = new NHunspellChecker.NHunspell();
            var force3 = new SpellUnit.ExceptionLiteralsExtractor.ExceptionLiteralsExtractor();
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
            // bool exceptionThrown = false; // will throw and die on exception
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("IgnoreProperty.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadlySpelledClass);
            runner.Run();
            
        }
#if false
        // this fails for now
        [TestMethod]
        public void BadlySpelledExceptionInClosure()
        {
            bool exceptionThrown = false;
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadExceptionEvenInClosuresClass);

            try
            {
                runner.Run();
            }
            catch (SpellUnit.SpellCheckException ex)
            {
                Assert.AreEqual("Value T SpellUnitBasicTests.BadExceptionClass::DataAccessMethod(T) exception string:0 failed rule NHunspell spell checker", ex.Message);
                exceptionThrown = true;
            }
            Assert.AreEqual(true, exceptionThrown);
        }
#endif
        [TestMethod]
        public void SunnyDayException()
        {
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(GoodExceptionClass);

            // shouldn't throw
            runner.Run();
        }
        [TestMethod]
        public void BadlySpelledException()
        {
            bool exceptionThrown = false;
            var settings = SpellUnit.SpellUnitTestSettings.FromFile("DefaultSpellUnit.testsettings.xml");
            var runner = new SpellUnit.SpellUnitTest(settings);
            runner.TypeUnderTest = typeof(BadExceptionClass);

            try
            {
                runner.Run();
            }
            catch (SpellUnit.SpellCheckException ex)
            {
                Assert.AreEqual("Value T SpellUnitBasicTests.BadExceptionClass::DataAccessMethod(T) exception string:0 failed rule NHunspell spell checker", ex.Message);
                exceptionThrown = true;
            }
            Assert.AreEqual(true, exceptionThrown);
        }

    }
}
