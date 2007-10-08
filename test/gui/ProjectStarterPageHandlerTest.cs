using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pavel.Framework;

using NUnit.Framework;
using Pavel.GUI;

namespace Pavel.Test.gui {
    [TestFixture]
    public class ProjectStarterPageTest {

        [Test]
        public void CorrectInitializingTest() {
            Pavel.GUI.ProjectStarterPageHandler wtpc = new Pavel.GUI.ProjectStarterPageHandler();
            Assert.AreEqual(false, wtpc.HasStarted());
        }

        //[Test, Ignore("ProjectstarterPage has become abstract")]
        //public void ForwardAndBackwardTest() {
            //Pavel.GUI.ProjectStarterPageHandler wtpc = new Pavel.GUI.ProjectStarterPageHandler();
            //ProjectStarterPage wp1 = new ProjectStarterPage();
            //ProjectStarterPage wp2 = new ProjectStarterPage();
            //wtpc.Add(wp1);
            //wtpc.Add(wp2);
            //Assert.AreEqual(false, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.nextPageControl();
            //Assert.AreEqual(true, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.nextPageControl();
            //Assert.AreEqual(true, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.nextPageControl();
            //Assert.AreEqual(true, wtpc.HasStarted());
            //Assert.AreEqual(true, wtpc.IsLastPage());
            //wtpc.previousPageControl();
            //Assert.AreEqual(true, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.previousPageControl();
            //Assert.AreEqual(true, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.previousPageControl();
            //Assert.AreEqual(false, wtpc.HasStarted());
            //Assert.AreEqual(false, wtpc.IsLastPage());
            //wtpc.previousPageControl();
            //Assert.AreEqual(false, wtpc.HasStarted());
            
        //}
    }
}