using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class CSVParserTest {

        [Test]
        public void SimpleTest() {
            new Pavel.Framework.CSVParser().Parse(new StreamReader(@"../..\..\dataFiles\TestFiles\Testdaten-CSV.csv")); 
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestParseNullString() { new Pavel.Framework.CSVParser().Parse(null); }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestParseWrongParameter1() {
            new Pavel.Framework.CSVParser().Parse(new StreamReader(@"../..\..\dataFiles\TestFiles\EA_test-dec.txt"));
        }

        [Test]
        public void TestParseTestdaten() {
            StreamReader allReader = new StreamReader(@"../..\..\dataFiles\TestFiles\Testdaten-CSV.csv");
            ParserResult masterData = new Pavel.Framework.CSVParser().Parse(allReader);
            Assert.AreEqual(5, masterData.Spaces[0].Dimension);
            Assert.AreEqual(49, masterData.MasterPointSet.Length);
        }
    }
}

