using System;
using System.Collections.Generic;
using System.Text;
using Pavel.GUI.Visualizations;

using NUnit.Framework;

namespace Pavel.Test.GUI.Visualizations {
    [TestFixture]
    public class VectorFTest {
        [Test]
        public void VectorFAddition() {
            VectorF a = VectorF.Unit;
            VectorF b = new VectorF(1.5f, 5.3f, 6.4f, 3.5f);
            VectorF c = a + b;
            Assert.AreEqual(2.5f, c.X);
            Assert.AreEqual(6.3f, c.Y);
            Assert.AreEqual(7.4f, c.Z);
            Assert.AreEqual(4.5f, c.W);
            VectorF d = c + b;
            Assert.AreEqual(4f, d.X);
            Assert.AreEqual(11.6f, d.Y);
            Assert.AreEqual(13.8f, d.Z);
            Assert.AreEqual(8f, d.W);
        }

        [Test]
        public void VectorFSubstraction() {
            VectorF a = VectorF.Unit;
            VectorF b = new VectorF(1.5f, 5.3f, 6.4f, 3.5f);
            VectorF c = a - b;
            Assert.AreEqual(1f-1.5f, c.X);
            Assert.AreEqual(1f-5.3f, c.Y);
            Assert.AreEqual(1f-6.4f, c.Z);
            Assert.AreEqual(1f-3.5f, c.W);
            VectorF d = c - b;
            Assert.AreEqual(-0.5f-1.5f, d.X);
            Assert.AreEqual(-4.3f-5.3f, d.Y);
            Assert.AreEqual(-5.4f-6.4f, d.Z);
            Assert.AreEqual(-2.5f-3.5f, d.W);
        }

        [Test]
        public void VectorFMultiply() {
            VectorF a = new VectorF(2f, 3f, 4f, 5f);
            VectorF b = new VectorF(1.5f, 5.3f, 6.4f, 3.5f);
            VectorF c = a * b;
            Assert.AreEqual(2f * 1.5f, c.X);
            Assert.AreEqual(3f * 5.3f, c.Y);
            Assert.AreEqual(4f * 6.4f, c.Z);
            Assert.AreEqual(5f * 3.5f, c.W);
            VectorF d = a * 2;
            Assert.AreEqual(4f, d.X);
            Assert.AreEqual(6f, d.Y);
            Assert.AreEqual(8f, d.Z);
            Assert.AreEqual(10f, d.W);
            VectorF e = 2 * a;
            Assert.AreEqual(4f, e.X);
            Assert.AreEqual(6f, e.Y);
            Assert.AreEqual(8f, e.Z);
            Assert.AreEqual(10f, e.W);
        }

        [Test]
        public void VectorFDivision() {
            VectorF a = new VectorF(2f, 3f, 4f, 5f);
            VectorF b = new VectorF(2f, 2f, 4f, 2.5f);
            VectorF c = a / b;
            Assert.AreEqual(1f, c.X);
            Assert.AreEqual(1.5f, c.Y);
            Assert.AreEqual(1f, c.Z);
            Assert.AreEqual(2f, c.W);
            VectorF d = a / 2;
            Assert.AreEqual(1f, d.X);
            Assert.AreEqual(1.5f, d.Y);
            Assert.AreEqual(2f, d.Z);
            Assert.AreEqual(2.5f, d.W);
            
        }
    }
}
