using System;
using System.Collections.Generic;
using System.Text;
using Pavel.Framework;
using Pavel.GUI;

using NUnit.Framework;

namespace Pavel.Test.Framework {
    [TestFixture]
    public class SpaceTest {

        Column[] cols = new Column[] { new Column("Cola"), new Column("Colb"), new Column("Colc") };
        ColumnSet columnSet;
        Space mSpace;

        [SetUp]
        public void SetUp() {
            columnSet = new ColumnSet(cols);
            foreach (Column c in cols) {
                c.DefaultColumnProperty = new ColumnProperty(c, 0, 0);
            }
            mSpace = new Space(columnSet,"");
            //Space sv2 = new Space(columnSet,"TestView");
        }

        [Test]
        public void CalculateMapTest() {
            int[] map = mSpace.CalculateMap(columnSet);
            Assert.AreEqual(map[0], 0);
            Assert.AreEqual(map[1], 1);
            Assert.AreEqual(map[2], 2);
        }

        [Test]
        public void IsViewOfSpaceTest() {
            ColumnSet cs1 = new ColumnSet(cols[1], cols[0], cols[2], cols[1]);
            ColumnSet cs2 = new ColumnSet(cols[0], new Column("Foo"), cols[2]);
            Assert.IsTrue(mSpace.IsViewOfColumnSet(columnSet));
            Assert.IsTrue(mSpace.IsViewOfColumnSet(cs1));
            Assert.IsFalse(mSpace.IsViewOfColumnSet(cs2));
        }

        [Test]
        public void MoveColumnTest() {
            Space space1 = new Space(columnSet,"");
            space1.MoveColumn(2, 0);
            space1.MoveColumn(1, 2);
            space1.MoveColumn(0, 2);
            Assert.AreEqual(space1[0], cols[1]);
            Assert.AreEqual(space1[1], cols[0]);
            Assert.AreEqual(space1[2], cols[2]);
        }

        [Test]
        public void SwapColumnsTest() {
            Space space1 = new Space(columnSet,"");
            space1.SwapColumns(1, 2);
            space1.SwapColumns(0, 2);
            space1.SwapColumns(1, 0);
            Assert.AreEqual(space1[0], cols[2]);
            Assert.AreEqual(space1[1], cols[1]);
            Assert.AreEqual(space1[2], cols[0]);
        }

        [Test]
        public void ToSpaceTest() {
            Space space1 = new Space(columnSet,"");
            ColumnSet cs1 = space1.ToColumnSet();
            Assert.AreEqual(space1[0], cs1.Columns[0]);
            Assert.AreEqual(space1[1], cs1.Columns[1]);
            Assert.AreEqual(space1[2], cs1.Columns[2]);

            ColumnProperty[] cols2 = new ColumnProperty[] {
                cols[1].DefaultColumnProperty.Clone(),
                cols[0].DefaultColumnProperty.Clone(),
                cols[1].DefaultColumnProperty.Clone(),
                cols[2].DefaultColumnProperty.Clone(),
                cols[1].DefaultColumnProperty.Clone(),
                cols[2].DefaultColumnProperty.Clone(),
                cols[1].DefaultColumnProperty.Clone(),
                cols[0].DefaultColumnProperty.Clone(),
                cols[0].DefaultColumnProperty.Clone()
            };
            Space space2 = new Space(cols2,"");
            cs1 = space1.ToColumnSet();
            Assert.AreEqual(columnSet.Columns[0], cs1.Columns[0]);
            Assert.AreEqual(columnSet.Columns[1], cs1.Columns[1]);
            Assert.AreEqual(columnSet.Columns[2], cs1.Columns[2]);
        }

        [Test]
        public void CopyAndWriteBackTest() {
            Space spaceParent = new Space(columnSet, "");
            Space spaceChild = spaceParent.CreateLocalCopy();
            Assert.AreEqual(spaceParent[0], spaceChild[0]);
            Assert.AreEqual(spaceParent[1], spaceChild[1]);
            Assert.AreEqual(spaceParent[2], spaceChild[2]);
            spaceChild.SwapColumns(1, 2);
            Assert.AreEqual(spaceParent[0], spaceChild[0]);
            Assert.AreEqual(spaceParent[2], spaceChild[1]);
            Assert.AreEqual(spaceParent[1], spaceChild[2]);
            spaceChild.WriteBack();
            Assert.AreEqual(spaceParent[0], spaceChild[0]);
            Assert.AreEqual(spaceParent[1], spaceChild[1]);
            Assert.AreEqual(spaceParent[2], spaceChild[2]);
        }
    }
}
