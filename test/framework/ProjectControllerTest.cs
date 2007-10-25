using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class ProjectControllerTest {
        [Test]
        public void SerializeDeserializeTest() {
            //Load
            ProjectController.NewProject(new Pavel.Plugins.CSVParser(",", new char[] { ';' }).Parse(new StreamReader(@"../..\..\dataFiles\TestFiles\Testdaten-CSV.csv"))); 
            int pointSets = ProjectController.Project.pointSets.Count;
            int points = ProjectController.Project.pointSets[0].Length;
            try {
                ProjectController.ExportProject("testFile.pav"); //Serialize
                ProjectController.OpenSavedProject("testFile.pav"); //Deserialize
                // Basic test
                Assert.AreEqual(pointSets, ProjectController.Project.pointSets.Count);
                Assert.AreEqual(points, ProjectController.Project.pointSets[0].Length);
                }
            catch (Exception e){
                throw new Exception("Serializing broken",e); //Throw inner exception
            }
            finally { File.Delete("testFile.pav"); }
        }
    }
}
