using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Structures;

namespace TestNumerics
{
    [TestClass]
    public class LandmarkTests
    {
        private Trait _trait = null!;
        private Focal _basisFocal = null!;
        private Focal _limits = null!;
        private Domain _domain = null!;
        private static double _delta = 0.001;

        [TestInitialize]
        public void Init()
        {
            _trait = new Trait("landmarkTests");
            _basisFocal = new Focal(0, 100);
            _limits = new Focal(-10000, 10000);
            _domain = new Domain(_trait, _basisFocal, _limits);
        }

        [TestMethod]
        public void LandmarkCreateTests()
        {
            var num = new Number(_domain, new Focal(0, 1000));
            var lms = new Landmark(num, 0.5);
            Assert.AreEqual(5, lms.Value);
            var lme = new Landmark(num, 1.5);
            Assert.AreEqual(15, lme.Value);

            var lmNum = new Number(_domain, lms, lme);
            Assert.AreEqual(5, lmNum.StartValue);
            Assert.AreEqual(15, lmNum.EndValue);

            var right = new Number(_domain, new Focal(-300, 600));
            num.SetStartValue(8);
            Assert.AreEqual(9, lmNum.StartValue);
            num.SetEndValue(20);
            Assert.AreEqual(26, lmNum.EndValue);
            var r0 = lmNum + right;
            Assert.AreEqual(17, r0.StartValue);
            Assert.AreEqual(32, r0.EndValue);

            num.SetValues(6, 8);
            Assert.AreEqual(7, lmNum.StartValue);
            Assert.AreEqual(9, lmNum.EndValue);
            var r1 = lmNum + right;
            Assert.AreEqual(10, r1.StartValue);
            Assert.AreEqual(15, r1.EndValue);

        }
        [TestMethod]
        public void NumberSetTests()
        {
            var num = new Number(_domain, new Focal(-600, 1000));
            var right = new Number(_domain, new Focal(-300, 600));

            Assert.AreEqual(6, num.StartValue);
            Assert.AreEqual(10, num.EndValue);

            num.SetStartValue(8);
            Assert.AreEqual(8, num.StartValue);
            num.SetEndValue(11);
            Assert.AreEqual(11, num.EndValue);
            var r0 = num + right;
            Assert.AreEqual(11, r0.StartValue);
            Assert.AreEqual(17, r0.EndValue);

            num.SetValues(6, 7);
            Assert.AreEqual(6, num.StartValue);
            Assert.AreEqual(7, num.EndValue);
            var r1 = num + right;
            Assert.AreEqual(9, r1.StartValue);
            Assert.AreEqual(13, r1.EndValue);
        }
    }
}
