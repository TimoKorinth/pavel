using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class ColumnSetTest {

        Column[] cols = new Column[] {
                new Column("Cola"), new Column("Colb"), new Column("Colc")
        };

        [Test]
        public void TestEnumConstructor() {
            ColumnSet sp1 = new ColumnSet(cols);

            ColumnSet sp2 = new ColumnSet(new List<Column>(cols));

            Assert.AreEqual(sp1.Dimension, sp2.Dimension);
            Assert.AreEqual(sp1.Columns, sp2.Columns);
        }

        [Test]
        public void TestContructorSorts() {
            Column[] cols2 = new Column[] { cols[2], cols[1], cols[0] };
            Column[] cols3 = new Column[] { cols[1], cols[2], cols[0] };
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols2);
            ColumnSet sp3 = new ColumnSet(cols3);
            Assert.AreEqual(sp1.Columns, sp2.Columns);
            Assert.AreEqual(sp1.Columns, sp3.Columns);

            Column c = new Column();
            ColumnSet sp = new ColumnSet(c, cols[2], cols[0], cols[1]);
            Assert.AreEqual(0, sp.IndexOf(cols[0]));
            Assert.AreEqual(1, sp.IndexOf(cols[1]));
            Assert.AreEqual(2, sp.IndexOf(cols[2]));
            Assert.AreEqual(3, sp.IndexOf(c));
        }

        [Test]
        public void TestEnumerator() {
            ColumnSet sp = new ColumnSet(cols);
            int i = 0;
            foreach (Column c in sp) {
                Assert.AreEqual(cols[i++], c);
            }
        }

        [Test]
        public void TestDimension() {
            ColumnSet sp = new ColumnSet(cols);
            Assert.AreEqual(cols.Length, sp.Dimension);
        }

        [Test]
        public void TestUnion() {
            ColumnSet sp1 = new ColumnSet(cols[1]);
            ColumnSet sp2 = new ColumnSet(cols[2], cols[0]);
            ColumnSet spu = ColumnSet.Union(sp1, sp2);
            Assert.AreEqual(spu.Columns, new Column[] { cols[0], cols[1], cols[2] });
        }

        [Test]
        public void TestSubtract() {
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols[1]);
            ColumnSet sps = ColumnSet.Subtract(sp1, sp2);
            Assert.AreEqual(sps.Columns, new Column[] { cols[0], cols[2] });

            sp2 = new ColumnSet(cols[2], cols[0]);
            sps = ColumnSet.Subtract(sp1, sp2);
            Assert.AreEqual(sps.Columns, new Column[] { cols[1]});

        }

        [Test]
        public void TestIntersect() {
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols[1]);
            ColumnSet sps = ColumnSet.Intersect(sp1, sp2);
            Assert.AreEqual(sps.Columns, new Column[] { cols[1] });

            sp2 = new ColumnSet(cols[2], cols[0]);
            sps = ColumnSet.Intersect(sp1, sp2);
            Assert.AreEqual(sps.Columns, new Column[] { cols[0], cols[2] });
        }

        [Test]
        public void TestSubColumnSetOf() {
            ColumnSet sp  = new ColumnSet(cols);
            ColumnSet sp1 = new ColumnSet(cols[2], cols[0]);
            Assert.IsTrue(sp1.IsSubSetOf(sp));
            ColumnSet sp2 = new ColumnSet(new Column[] { });
            Assert.IsTrue(sp2.IsSubSetOf(sp));
        }

        [Test]
        public void TestSubColumnSetOfInvalid() {
            ColumnSet sp  = new ColumnSet(cols);
            ColumnSet sp1 = new ColumnSet(cols[2], cols[0]);
            ColumnSet sp2 = new ColumnSet(new Column[] { });
            Assert.IsFalse(sp.IsSubSetOf(sp1));
            Assert.IsFalse(sp.IsSubSetOf(sp2));
        }

        [Test]
        public void TestSuperColumnSetMap() {
            ColumnSet sp  = new ColumnSet(cols);
            ColumnSet sp1 = new ColumnSet(cols[2], cols[0]);
            Assert.AreEqual(new int[] { 0, 2 }, sp1.SuperSetMap(sp));
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void TestSuperColumnSetMapInvalid() {
            ColumnSet sp  = new ColumnSet(cols);
            ColumnSet sp1 = new ColumnSet(cols[2], cols[0]);
            sp.SuperSetMap(sp1);
        }

        [Test]
        public void TestIndexOf() {
            ColumnSet sp = new ColumnSet(cols);
            Assert.AreEqual(0, sp.IndexOf(cols[0]));
            Assert.AreEqual(1, sp.IndexOf(cols[1]));
            Assert.AreEqual(2, sp.IndexOf(cols[2]));
        }

        [Test]
        public void TestIndexOfInvalid() {
            ColumnSet sp = new ColumnSet(cols);
            Assert.IsTrue(sp.IndexOf(new Column()) < 0);
        }

        [Test]
        public void TestEquals() {
            ColumnSet sp1 = new ColumnSet(cols);
            ColumnSet sp2 = new ColumnSet(cols);
            ColumnSet sp3 = new ColumnSet(cols[2], cols[0]);
            ColumnSet sp4 = new ColumnSet(cols[2], cols[0], new Column());

            Assert.IsTrue(sp1.Equals(sp2));
            Assert.IsFalse(sp1.Equals(sp3));
            Assert.IsFalse(sp1.Equals(sp4));
        }
    }
}

