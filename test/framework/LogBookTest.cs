using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class LogBookTest {

        [Test]
        public void TestAddLog() {
            LogBook lb = new LogBook();
            lb.Message("Message1", false);
            // Count of the Messages should be 1
            Assert.AreEqual(1,(lb.GetLogs(LogLevelEnum.Message)).Count);
            lb.Message("Message2", false);
            // Count of the Messages should be 2
            Assert.AreEqual(2, (lb.GetLogs(LogLevelEnum.Message)).Count);
            lb.Warning("Warning1", false);
            // Count of the Warnings should be 1
            Assert.AreEqual(1, (lb.GetLogs(LogLevelEnum.Warning)).Count);
            lb.Warning("Warning2", false);
            // Count of the Warnings should be 2
            Assert.AreEqual(2, (lb.GetLogs(LogLevelEnum.Warning)).Count);
            lb.Error("Error1", false);
            // Count of the Errors should be 1
            Assert.AreEqual(1, (lb.GetLogs(LogLevelEnum.Error)).Count);
            lb.Error("Error2", false);
            // Count of the Errors should be 2
            Assert.AreEqual(2, (lb.GetLogs(LogLevelEnum.Error)).Count);
            // Total count should be 6
            Assert.AreEqual(6, (lb.GetLogs(LogLevelEnum.Error | LogLevelEnum.Warning | LogLevelEnum.Message)).Count);
        }
    }
}
