using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Utils;

namespace TestNumerics.Expressions
{
    [TestClass]
    public class WeightedBoolsTests
    {
        private Trait _trait = null!;
        private Focal _basisFocal = null!;
        private Focal _limits = null!;
        private Domain _domain = null!;
        private Number _n_m2_8 = null!;
        private Number _n_m3_6 = null!;
        private Number _n_1_m7 = null!;
        private Number _n_m3_m6 = null!;
        private Number[] _nList = null!;
        private static double _delta = 0.001;

        [TestInitialize]
        public void Init()
        {
            _trait = new Trait("WeightedBools Tests");
            _basisFocal = new Focal(0, 100);
            _limits = new Focal(-10000, 10000);
            _domain = new Domain(_trait, _basisFocal, _limits);
            _n_m2_8 = new Number(_domain.DefaultBasisNumber, new(200, 800)); //   (-2i + 8)
            _n_m3_6 = new Number(_domain.DefaultBasisNumber, new(300, 600)); //   (-3i + 6)
            _n_1_m7 = new Number(_domain.DefaultBasisNumber, new(-100, -700)); // (i -7)
            _n_m3_m6 = new Number(_domain.DefaultBasisNumber, new(300, -600));// (-3i -6)
            _nList = new Number[] { _n_m2_8, _n_m3_6, _n_1_m7, _n_m3_m6 };
        }

        [TestMethod]
        public void CreatePointsTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_6);
            Assert.AreEqual(-2, wb.QuadBoth[0].StartValue);
            Assert.AreEqual(0,  wb.QuadBoth[0].EndValue);
            Assert.AreEqual(-3, wb.QuadBoth[1].StartValue);
            Assert.AreEqual(0,  wb.QuadBoth[1].EndValue);

            Assert.AreEqual(-2, wb.QuadYOnly[0].StartValue);
            Assert.AreEqual(8,  wb.QuadYOnly[0].EndValue);
            Assert.AreEqual(-3, wb.QuadYOnly[1].StartValue);
            Assert.AreEqual(0,  wb.QuadYOnly[1].EndValue);

            Assert.AreEqual(-2, wb.QuadXOnly[0].StartValue);
            Assert.AreEqual(0,  wb.QuadXOnly[0].EndValue);
            Assert.AreEqual(-3, wb.QuadXOnly[1].StartValue);
            Assert.AreEqual(6,  wb.QuadXOnly[1].EndValue);

            Assert.AreEqual(-2, wb.QuadNeither[0].StartValue);
            Assert.AreEqual(8,  wb.QuadNeither[0].EndValue);
            Assert.AreEqual(-3, wb.QuadNeither[1].StartValue);
            Assert.AreEqual(6,  wb.QuadNeither[1].EndValue);
        }

        [TestMethod]
        public void IdentityTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_6);
            var result = wb.Identity();
            Assert.AreEqual(-2, result[0][0].StartValue);
            Assert.AreEqual(0, result[0][0].EndValue);
            Assert.AreEqual(-3, result[0][1].StartValue);
            Assert.AreEqual(0, result[0][1].EndValue);

            Assert.AreEqual(-2, result[1][0].StartValue);
            Assert.AreEqual(8, result[1][0].EndValue);
            Assert.AreEqual(-3, result[1][1].StartValue);
            Assert.AreEqual(0, result[1][1].EndValue);

            Assert.AreEqual(-2, result[2][0].StartValue);
            Assert.AreEqual(0, result[2][0].EndValue);
            Assert.AreEqual(-3, result[2][1].StartValue);
            Assert.AreEqual(6, result[2][1].EndValue);

            Assert.AreEqual(-2, result[3][0].StartValue);
            Assert.AreEqual(8, result[3][0].EndValue);
            Assert.AreEqual(-3, result[3][1].StartValue);
            Assert.AreEqual(6, result[3][1].EndValue);

        }

        [TestMethod]
        public void AndTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_6);
            var result = wb.And();

            Assert.IsTrue(result[0][0].IsAligned);
            Assert.IsTrue(result[0][1].IsAligned);
            Assert.AreEqual(-2, result[0][0].StartValue);
            Assert.AreEqual(0, result[0][0].EndValue);
            Assert.AreEqual(-3, result[0][1].StartValue);
            Assert.AreEqual(0, result[0][1].EndValue);

            Assert.IsTrue(result[1][0].IsInverted);
            Assert.IsTrue(result[1][1].IsInverted);
            Assert.AreEqual(2, result[1][0].StartValue);
            Assert.AreEqual(-8, result[1][0].EndValue);
            Assert.AreEqual(3, result[1][1].StartValue);
            Assert.AreEqual(0, result[1][1].EndValue);

            Assert.IsTrue(result[2][0].IsInverted);
            Assert.IsTrue(result[2][1].IsInverted);
            Assert.AreEqual(2, result[2][0].StartValue);
            Assert.AreEqual(0, result[2][0].EndValue);
            Assert.AreEqual(3, result[2][1].StartValue);
            Assert.AreEqual(-6, result[2][1].EndValue);

            Assert.IsTrue(result[3][0].IsInverted);
            Assert.IsTrue(result[3][1].IsInverted);
            Assert.AreEqual(2, result[3][0].StartValue);
            Assert.AreEqual(-8, result[3][0].EndValue);
            Assert.AreEqual(3, result[3][1].StartValue);
            Assert.AreEqual(-6, result[3][1].EndValue);

        }
        [TestMethod]
        public void OrSumTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_6);
            var result = wb.Calculate2D(BoolOp.Or, WeightedBools.MULTIPLY, WeightedBools.ADD);

            Assert.AreEqual(0, result.StartValue);
            Assert.AreEqual(-54, result.EndValue);

        }
    }
}
