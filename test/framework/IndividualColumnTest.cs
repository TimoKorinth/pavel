using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using NUnit.Framework;
using System.Windows.Forms;
using System.Globalization;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class IndividualColumnTest {

        Column[] cols;

        [SetUp]
        public void setUp() {
            ProjectController.ResetProject();
            cols = new Column[] { new Column(), new Column() };
            Application.CurrentCulture = new CultureInfo("en-US");
        }
        

        public PointSet PointSet() {
            ColumnSet columnSet = new ColumnSet(cols);
            Point[] values = new Point[] {
                new Point(columnSet,0,5),
                new Point(columnSet,1,6),
                new Point(columnSet,2,7),
                new Point(columnSet,3,8),
                new Point(columnSet,4,9),
            };
            
            PointList pl = new PointList(columnSet,values);
            PointSet ps = new PointSet("",columnSet);
            ps.Add(pl);
            return ps;
        }

        [Test]
        public void SimpleExtentionTest() {
            PointSet pointset = PointSet();
            ProjectController.Project.pointSets.Add(pointset);
            List<PointSet> ps = new List<PointSet>();
            ps.Add(pointset);
            Assert.AreEqual(2, pointset.ColumnSet.Dimension);

            Column col = IndividualColumns.CreateColumn(1, "$0*2", ps, "newCol");
            pointset = ProjectController.Project.pointSets[0];
            Assert.AreEqual(3, pointset.ColumnSet.Dimension);

            for ( int i = 0; i < pointset.Length-1; i++ ) {
                Assert.AreEqual(pointset[i][0] * 2, pointset[i][2]);
            }
            ProjectController.Project.pointSets.Clear();
        }

        [Test]
        public void TwoPointSetsOnePointListTest( ) {
            PointSet pointset = PointSet();
            PointSet pointset2 = new PointSet("test2", pointset.ColumnSet);
            ProjectController.Project.pointSets.Add(pointset);
            ProjectController.Project.pointSets.Add(pointset2);
            pointset2.Add(pointset.PointLists[0]);
            List<PointSet> ps = new List<PointSet>();
            ps.Add(pointset);

            Assert.AreEqual(2, pointset.ColumnSet.Dimension);

            Column col = IndividualColumns.CreateColumn(1, "$0*2", ps, "newCol");
            pointset = ProjectController.Project.pointSets[1];
            Assert.AreEqual(3, pointset.ColumnSet.Dimension);

            for ( int i = 0; i < pointset.Length - 1; i++ ) {
                Assert.AreEqual(pointset[i][0] * 2, pointset[i][2]);
            }

            Assert.AreNotEqual(pointset.PointLists[0], pointset2.PointLists[0]);

            for ( int i = 0; i < pointset.Length - 1; i++ ) {
                Assert.AreEqual(pointset[i][0], pointset2[i][0]);
                Assert.AreEqual(pointset[i][1], pointset2[i][1]);
                Assert.AreNotEqual(pointset[i], pointset2[i]);
            }

            for ( int i = 0; i < pointset.Length - 1; i++ ) {
                Assert.AreEqual(pointset[i][0] * 2, pointset[i][2]);
            }

            ProjectController.Project.pointSets.Clear();
        }

        [Test]
        public void TwoPointSetsCommonPointListTest( ) {
            PointSet pointset = PointSet();
            PointSet pointset2 = new PointSet("test2", pointset.ColumnSet);
            ProjectController.Project.pointSets.Add(pointset);
            ProjectController.Project.pointSets.Add(pointset2);

            PointList pl = new PointList(new ColumnSet(cols));
            pointset2.Add(pl);
            Point point = new Point(new ColumnSet(cols), new double[] { 20, 30 });
            pl.Add(point);

            pointset.PointLists[0].Add(point);
            List<PointSet> ps = new List<PointSet>();
            ps.Add(pointset);

            Assert.AreEqual(2, pointset.ColumnSet.Dimension);

            Column col = IndividualColumns.CreateColumn(1, "$0*2", ps, "newCol");
            pointset = ProjectController.Project.pointSets[1];
            Assert.AreEqual(3, pointset.ColumnSet.Dimension);

            for ( int i = 0; i < pointset.Length - 1; i++ ) {
                Assert.AreEqual(pointset[i][0] * 2, pointset[i][2]);
            }

            Assert.AreEqual(pointset2[0], point);
            Assert.AreNotEqual(pointset[5], point);

            Assert.AreEqual(pointset[5][0], pointset2[0][0]);
            Assert.AreEqual(pointset[5][1], pointset2[0][1]);

            ProjectController.Project.pointSets.Clear();
        }

        [Test]
        public void SimpleOperationsTest() {
            Assert.AreEqual(3.141592654, IndividualColumns.Evaluate("pi"), 0.00000001);
            Assert.AreEqual(2.718281828, IndividualColumns.Evaluate("e"), 0.00000001);

            Assert.AreEqual(5.02, IndividualColumns.Evaluate("2.51+2.51"));
            Assert.AreEqual(1.23, IndividualColumns.Evaluate("2.33 - 1.1"));
            Assert.AreEqual(6.3001, IndividualColumns.Evaluate("2.51 *2.51"), 0.0000000000001);
            Assert.AreEqual(1.5, IndividualColumns.Evaluate("3/ 2"));
            Assert.AreEqual(6.0, IndividualColumns.Evaluate(" 3!"));
            Assert.AreEqual(27.0, IndividualColumns.Evaluate("3^3 "));
            Assert.AreEqual(2.48490665, IndividualColumns.Evaluate("ln 12"), 0.00000001);
            Assert.AreEqual(1.079181246, IndividualColumns.Evaluate("log 12"), 0.00000001);
            Assert.AreEqual(4, IndividualColumns.Evaluate("-1%5"));
            Assert.AreEqual(84, IndividualColumns.Evaluate("9C3"));
            Assert.AreEqual(504, IndividualColumns.Evaluate("9P3"));
            Assert.AreEqual(2, IndividualColumns.Evaluate("3root8"));
            Assert.AreEqual(2.449489743, IndividualColumns.Evaluate("sqrt6"), 0.00000001);
            Assert.AreEqual(2, IndividualColumns.Evaluate("round2.5"));
            Assert.AreEqual(3, IndividualColumns.Evaluate("round 2.501"));
            Assert.AreEqual(2, IndividualColumns.Evaluate("floor2.5"));
            Assert.AreEqual(3, IndividualColumns.Evaluate("ceiling2.5"));
            Assert.AreEqual(5, IndividualColumns.Evaluate("abs(-5)"));

            Assert.AreEqual(11013.23287, IndividualColumns.Evaluate("sinh10"), 0.00001);
            Assert.AreEqual(1.127625965, IndividualColumns.Evaluate("cosh0.5"), 0.00000001);
            Assert.AreEqual(0.462117157, IndividualColumns.Evaluate("tanh0.5"), 0.00000001);
            Assert.AreEqual(2.163953414, IndividualColumns.Evaluate("coth0.5"), 0.00000001);
            Assert.AreEqual(0.886818884, IndividualColumns.Evaluate("sech0.5"), 0.00000001);
            Assert.AreEqual(1.919034751, IndividualColumns.Evaluate("cosech0.5"), 0.00000001);

            Assert.AreEqual(0.48121182506, IndividualColumns.Evaluate("Arsinh0.5"), 0.00000000001);
            Assert.AreEqual(0.962423650119, IndividualColumns.Evaluate("Arcosh1.5"), 0.00000000001);
            Assert.AreEqual(0.549306144334, IndividualColumns.Evaluate("Artanh0.5"), 0.00000000001);
            Assert.AreEqual(0.202732554, IndividualColumns.Evaluate("Arcoth5"), 0.00000001);
            Assert.AreEqual(1.316957897, IndividualColumns.Evaluate("Arsech0.5"), 0.00000001);
            Assert.AreEqual(1.443635475, IndividualColumns.Evaluate("Arcsch0.5"), 0.00000001);
            Assert.AreEqual(-1.443635475, IndividualColumns.Evaluate("Arcsch-0.5"), 0.00000001);

            //DEG
            IndividualColumns.factor = Math.PI / 180;
            Assert.AreEqual(1, IndividualColumns.Evaluate("sin90"), 0.00000001);
            Assert.AreEqual(0.866025403, IndividualColumns.Evaluate("cos30"), 0.00000001);
            Assert.AreEqual(0.577350269, IndividualColumns.Evaluate("tan30"), 0.00000001);
            Assert.AreEqual(1.732050808, IndividualColumns.Evaluate("cot30"), 0.00000001);
            Assert.AreEqual(1.154700538, IndividualColumns.Evaluate("sec30"), 0.00000001);
            Assert.AreEqual(2, IndividualColumns.Evaluate("cosec30"), 0.00000001);

            Assert.AreEqual(30, IndividualColumns.Evaluate("asin0.5"), 0.00000001);
            Assert.AreEqual(60, IndividualColumns.Evaluate("acos0.5"), 0.00000001);
            Assert.AreEqual(89.36340642, IndividualColumns.Evaluate("atan90"), 0.00000001);
            Assert.AreEqual(0.636593575, IndividualColumns.Evaluate("acot90"), 0.00000001);
            Assert.AreEqual(48.18968511, IndividualColumns.Evaluate("asec1.5"), 0.00000001);
            Assert.AreEqual(41.81031489, IndividualColumns.Evaluate("acosec1.5"), 0.00000001);
            //RAD
            IndividualColumns.factor = 1.0d;
            Assert.AreEqual(0.893996663, IndividualColumns.Evaluate("sin90"), 0.00000001);
            Assert.AreEqual(-0.448073616, IndividualColumns.Evaluate("cos90"), 0.00000001);
            Assert.AreEqual(-1.995200412, IndividualColumns.Evaluate("tan90"), 0.00000001);
            Assert.AreEqual(-0.501202783, IndividualColumns.Evaluate("cot90"), 0.00000001);
            Assert.AreEqual(-2.231776128, IndividualColumns.Evaluate("sec90"), 0.00000001);
            Assert.AreEqual(1.118572407, IndividualColumns.Evaluate("cosec90"), 0.00000001);

            Assert.AreEqual(0.523598775, IndividualColumns.Evaluate("asin0.5"), 0.00000001);
            Assert.AreEqual(1.047197551, IndividualColumns.Evaluate("acos0.5"), 0.00000001);
            Assert.AreEqual(1.559685673, IndividualColumns.Evaluate("atan90"), 0.00000001);
            Assert.AreEqual(0.011110653, IndividualColumns.Evaluate("acot90"), 0.00000001);
            Assert.AreEqual(0.84106867, IndividualColumns.Evaluate("asec1.5"), 0.00000001);
            Assert.AreEqual(-0.729727656, IndividualColumns.Evaluate("acosec-1.5"), 0.00000001);
            //GRAD
            IndividualColumns.factor = Math.PI / 200;
            Assert.AreEqual(0.98768834, IndividualColumns.Evaluate("sin90"), 0.00000001);
            Assert.AreEqual(0.156434465, IndividualColumns.Evaluate("cos90"), 0.00000001);
            Assert.AreEqual(6.313751515, IndividualColumns.Evaluate("tan90"), 0.00000001);
            Assert.AreEqual(0.15838444, IndividualColumns.Evaluate("cot90"), 0.00000001);
            Assert.AreEqual(6.392453222, IndividualColumns.Evaluate("sec90"), 0.00000001);
            Assert.AreEqual(1.012465126, IndividualColumns.Evaluate("cosec90"), 0.00000001);

            Assert.AreEqual(33.33333333, IndividualColumns.Evaluate("asin0.5"), 0.00000001);
            Assert.AreEqual(50.63666222, IndividualColumns.Evaluate("acos0.7"), 0.00000001);
            Assert.AreEqual(99.2926738, IndividualColumns.Evaluate("atan90"), 0.00000001);
            Assert.AreEqual(0.707326195, IndividualColumns.Evaluate("acot90"), 0.0000001);
            Assert.AreEqual(53.54409456, IndividualColumns.Evaluate("asec1.5"), 0.00000001);
            Assert.AreEqual(46.45590544, IndividualColumns.Evaluate("acosec1.5"), 0.00000001);
        }

        [Test]
        public void MultipleOperationsTest() {
            Assert.AreEqual(5, IndividualColumns.Evaluate("2*2+2-2/2"));
            Assert.AreEqual(2, IndividualColumns.Evaluate("2*(2+2-2)/2"));
            Assert.AreEqual(0.779581082, IndividualColumns.Evaluate("sinsin90"), 0.00000001);
            Assert.AreEqual(4.666666667, IndividualColumns.Evaluate("(((3+2)*4)-6)/3"), 0.00000001);
        }

        [Test, ExpectedException(typeof(FormatException))]
        public void OperationErrorsTest() {
            IndividualColumns.Evaluate("2*2+2-2/");
            IndividualColumns.Evaluate("sinsin");
            IndividualColumns.Evaluate("*2");
            IndividualColumns.Evaluate("tan90");
            IndividualColumns.Evaluate("2*(2");
        }

        [Test, ExpectedException(typeof(DivideByZeroException))]
        public void DivisionByZeroTest() {
            IndividualColumns.Evaluate("9/0");
        }
    }
}
