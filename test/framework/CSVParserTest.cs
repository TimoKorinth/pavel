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
            new Pavel.Plugins.CSVParser().Parse(new StreamReader(@"../..\..\dataFiles\TestFiles\Testdaten-CSV.csv", System.Text.Encoding.GetEncoding(1252))); 
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestParseNullString( ) { new Pavel.Plugins.CSVParser().Parse(null); }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestParseWrongParameter1() {
            new Pavel.Plugins.CSVParser().Parse(new StreamReader(@"../..\..\dataFiles\TestFiles\EA_test-dec.txt"));
        }

        [Test]
        public void TestParseTestdaten() {
            StreamReader allReader = new StreamReader(@"../..\..\dataFiles\TestFiles\Testdaten-CSV.csv");
            ParserResult masterData = new Pavel.Plugins.CSVParser().Parse(allReader);
            Assert.AreEqual(5, masterData.Spaces[0].Dimension);
            Assert.AreEqual(49, masterData.MasterPointSet.Length);
        }

        [Test]
        public void TestGenerateSpacesWhenParsing() {
            StreamReader allReader = new StreamReader(@"../..\..\dataFiles\TestFiles\csv_with_spaces.csv");
            ParserResult masterData = new Pavel.Plugins.CSVParser().Parse(allReader);
            Assert.AreEqual(3, masterData.Spaces.Count);
            Assert.IsTrue(masterData.Spaces.Exists(delegate(Space sp) { return sp.Label == "Objective Space"; }));
            Assert.IsTrue(masterData.Spaces.Exists(delegate(Space sp) { return sp.Label == "Decision Space"; }));
            Assert.IsTrue(masterData.Spaces.Exists(delegate(Space sp) { return sp.Label == "Master Space"; }));
            Assert.AreEqual(5, masterData.Spaces.Find(delegate(Space sp) { return sp.Label == "Master Space"; }).Dimension);
            Assert.AreEqual(2, masterData.Spaces.Find(delegate(Space sp) { return sp.Label == "Objective Space"; }).Dimension);
            Assert.AreEqual(2, masterData.Spaces.Find(delegate(Space sp) { return sp.Label == "Decision Space"; }).Dimension);
            Assert.AreEqual("Column 1: Randomvalues",masterData.Spaces.Find(delegate(Space sp) { return sp.Label == "Decision Space"; }).ColumnProperties[0].Label);
        }
    }
}

