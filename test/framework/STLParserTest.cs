using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class STLParserTest {
        [Test]
        public void SimpleTest() {
            new Pavel.Framework.STLParser().Parse(new StreamReader(@"..\..\..\dataFiles\MouldTemperatures\MouldTemperature-01-ascii.stl")); 
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestParseNullString() { new Pavel.Framework.STLParser().Parse(null); }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestParseWrongParameter1() {
            new Pavel.Framework.STLParser().Parse(new StreamReader(@"..\..\..\dataFiles\MouldTemperatures\MouldTemperature-DLL-Test-Obj.txt"));
        }

        [Test]
        public void TestParseTestdaten() {
            StreamReader allReader = new StreamReader(@"..\..\..\dataFiles\MouldTemperatures\MouldTemperature-01-ascii.stl");
            Pavel.Framework.STLParser stlparser = new Pavel.Framework.STLParser();
            stlparser.Parse(allReader);
            Assert.AreEqual(768, stlparser.NormalArray.Length);
            Assert.AreEqual(2304, stlparser.VertexArray.Length);
        }

        [Test]
        public void TestParseTestdatenExponential() {
            StreamReader allReader = new StreamReader(@"..\..\..\dataFiles\testFiles\Exponential-STL.stl");
            Pavel.Framework.STLParser stlparser = new Pavel.Framework.STLParser();
            stlparser.Parse(allReader);
            Assert.AreEqual(3672, stlparser.NormalArray.Length);
            Assert.AreEqual(11016, stlparser.VertexArray.Length);
        }

        [Test]
        public void TestParseTestdatenBinary() {
            StreamReader allReader = new StreamReader(@"..\..\..\dataFiles\MillingOptimization\Milling-mittel-01-binary.stl");
            Pavel.Framework.STLParser stlparser = new Pavel.Framework.STLParser();
            stlparser.Parse(allReader);
            Assert.AreEqual(59742, stlparser.NormalArray.Length);
            Assert.AreEqual(179226, stlparser.VertexArray.Length);
        }
    }
}

