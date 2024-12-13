using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;
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
        private Number _n_m3_7 = null!;
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
            _n_m3_7 = new Number(_domain.DefaultBasisNumber, new(300, 700)); //   (-3i + 7)
            _n_1_m7 = new Number(_domain.DefaultBasisNumber, new(-100, -700)); // (i -7)
            _n_m3_m6 = new Number(_domain.DefaultBasisNumber, new(300, -600));// (-3i -6)
            _nList = new Number[] { _n_m2_8, _n_m3_7, _n_1_m7, _n_m3_m6 };
        }

        [TestMethod]
        public void CreatePointsTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_7);

            Assert.AreEqual(2, wb.QuadNeither[0].StartValue);
            Assert.AreEqual(0, wb.QuadNeither[0].EndValue);
            Assert.AreEqual(3, wb.QuadNeither[1].StartValue);
            Assert.AreEqual(0, wb.QuadNeither[1].EndValue);

            Assert.AreEqual(-2, wb.QuadXOnly[0].StartValue);
            Assert.AreEqual(8, wb.QuadXOnly[0].EndValue);
            Assert.AreEqual(3, wb.QuadXOnly[1].StartValue);
            Assert.AreEqual(0, wb.QuadXOnly[1].EndValue);

            Assert.AreEqual(-2, wb.QuadBoth[0].StartValue);
            Assert.AreEqual(8, wb.QuadBoth[0].EndValue);
            Assert.AreEqual(-3, wb.QuadBoth[1].StartValue);
            Assert.AreEqual(7, wb.QuadBoth[1].EndValue);

            Assert.AreEqual(2, wb.QuadYOnly[0].StartValue);
            Assert.AreEqual(0, wb.QuadYOnly[0].EndValue);
            Assert.AreEqual(-3, wb.QuadYOnly[1].StartValue);
            Assert.AreEqual(7, wb.QuadYOnly[1].EndValue);
        }

        [TestMethod]
        public void IdentityTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_7);
            var result = wb.Identity();
            Assert.AreEqual(2, result[0][0].StartValue);
            Assert.AreEqual(0, result[0][0].EndValue);
            Assert.AreEqual(3, result[0][1].StartValue);
            Assert.AreEqual(0, result[0][1].EndValue);

            Assert.AreEqual(-2, result[1][0].StartValue);
            Assert.AreEqual(8, result[1][0].EndValue);
            Assert.AreEqual(3, result[1][1].StartValue);
            Assert.AreEqual(0, result[1][1].EndValue);

            Assert.AreEqual(2, result[2][0].StartValue);
            Assert.AreEqual(0, result[2][0].EndValue);
            Assert.AreEqual(-3, result[2][1].StartValue);
            Assert.AreEqual(7, result[2][1].EndValue);

            Assert.AreEqual(-2, result[3][0].StartValue);
            Assert.AreEqual(8, result[3][0].EndValue);
            Assert.AreEqual(-3, result[3][1].StartValue);
            Assert.AreEqual(7, result[3][1].EndValue);

        }

        [TestMethod]
        public void AndTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_7);
            var result = wb.And();

            Assert.IsTrue(result[0][0].IsZero);
            Assert.IsTrue(result[0][1].IsZero);

            Assert.IsTrue(result[1][0].IsZero);
            Assert.IsTrue(result[1][1].IsZero);

            Assert.IsTrue(result[2][0].IsZero);
            Assert.IsTrue(result[2][1].IsZero);

            Assert.IsTrue(result[3][0].IsAligned);
            Assert.IsTrue(result[3][1].IsAligned);
            Assert.AreEqual(-2, result[3][0].StartValue);
            Assert.AreEqual(8, result[3][0].EndValue);
            Assert.AreEqual(-3, result[3][1].StartValue);
            Assert.AreEqual(7, result[3][1].EndValue);

        }
        [TestMethod]
        public void OrSumTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_7);

            var resultAnd = wb.Calculate2D(BoolOp.And, WeightedBools.ABS_AREA, WeightedBools.ADD);
            Assert.AreEqual(0, resultAnd.StartValue);
            Assert.AreEqual(24, resultAnd.EndValue);

            var resultXor = wb.Calculate2D(BoolOp.Xor, WeightedBools.ABS_AREA, WeightedBools.ADD);
            Assert.AreEqual(0, resultXor.StartValue);
            Assert.AreEqual(26, resultXor.EndValue);

        }

        [TestMethod]
        public void ScaledBitMultTests()
        {
            var wb = WeightedBools.CreateByPoints(_n_m2_8, _n_m3_7);

            var resultOr = wb.Calculate2D(BoolOp.Or, WeightedBools.ABS_AREA, WeightedBools.ADD);
            Assert.AreEqual(0, resultOr.StartValue);
            Assert.AreEqual(50, resultOr.EndValue);

            var resultNotA = wb.Calculate2D(BoolOp.NotA, WeightedBools.ABS_AREA, WeightedBools.ADD);
            Assert.AreEqual(0, resultNotA.StartValue);
            Assert.AreEqual(14, resultNotA.EndValue);

            var resultNotB = wb.Calculate2D(BoolOp.NotB, WeightedBools.ABS_AREA, WeightedBools.ADD);
            Assert.AreEqual(0, resultNotB.StartValue);
            Assert.AreEqual(24, resultNotB.EndValue);

            var n_i = resultNotA + resultNotB;
            var bmult = new Number(_n_m2_8.BasisNumber, new Landmark(n_i, 1), new Landmark(resultOr, 1));
            var mult = _n_m2_8 * _n_m3_7;

            Assert.AreEqual(mult.StartValue, bmult.StartValue);
            Assert.AreEqual(mult.EndValue, bmult.EndValue);
        }
    }
}
