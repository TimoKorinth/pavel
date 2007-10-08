using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;
using System.Reflection;
using Pavel.GUI;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class PluginManagerTest {

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorPluginPathNull() {
            new PluginManager(null);
        }

        [Test]
        public void TestFindsUseCases() {
            PluginManager pm = new PluginManager(@"../../../bin/debug/plugins");
            bool foundDefault = false;
            foreach (IUseCase u in pm.AvailableUseCases) {
                if ( u.Label == "Default" ) { foundDefault = true; }
            }
            Assert.IsTrue(foundDefault);
        }

    }
}
