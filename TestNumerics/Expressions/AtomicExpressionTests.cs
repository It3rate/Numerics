using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;

namespace TestNumerics.Expressions;

[TestClass]
public class AtomicExpressionTests
{
    public class EquationTests
    {
        private Trait _trait = null!;
        private Focal _basisFocal = null!;
        private Focal _limits = null!;
        private Domain _domain = null!;
        private Number _num2_8 = null!;
        private Number _num3_6 = null!;
        private Number[] _numList = null!;
        private static double _delta = 0.001;

        [TestInitialize]
        public void Init()
        {
            _trait = new Trait("Tests");
            _basisFocal = new Focal(0, 100);
            _limits = new Focal(-10000, 10000);
            _domain = new Domain(_trait, _basisFocal, _limits);
            _num2_8 = new Number(_domain, new(-200, 800)); //   (2i + 8)
            _num3_6 = new Number(_domain, new(-300, 600)); //   (3i + 6)
            _numList = new Number[] { _num2_8, _num3_6 };
        }

        [TestMethod]
        public void UnaryStepTests()
        {
            var step = new AtomicExpression(new ResetOperation(), 2);
            var result = step.Calculate(_num2_8);
            Assert.AreEqual(0, result.StartValue);
            Assert.AreEqual(0, result.EndValue);


            step = new AtomicExpression(new InvertOperation(), 2);
            result = step.Calculate(_num2_8);
            Assert.AreEqual(-2, result.StartValue);
            Assert.AreEqual(-8, result.EndValue);

            step = new AtomicExpression(new IncrementOperation(), 2);
            result = step.Calculate(_num2_8);
            Assert.AreEqual(2, result.StartValue);
            Assert.AreEqual(9, result.EndValue);

            step = new AtomicExpression(new DecrementOperation(), 2);
            result = step.Calculate(_num2_8);
            Assert.AreEqual(2, result.StartValue);
            Assert.AreEqual(7, result.EndValue);

            step = new AtomicExpression(new IncrementEndTickOperation(), 2);
            result = step.Calculate(_num2_8);
            Assert.AreEqual(2, result.StartValue);
            Assert.AreEqual(8.01, result.EndValue);

            step = new AtomicExpression(new DecrementEndTickOperation(), 2);
            result = step.Calculate(_num2_8);
            Assert.AreEqual(2, result.StartValue);
            Assert.AreEqual(7.99, result.EndValue);
        }

        [TestMethod]
        public void BinaryStepTests()
        {
            var expr = new Expression(_numList);

            var step = new AtomicExpression(0, new AddOperation(), 2);
            expr.AddAtomicExpression(step);
            var result = step.Calculate(_num3_6);
            Assert.AreEqual(5, result.StartValue);
            Assert.AreEqual(14, result.EndValue);

            step = new AtomicExpression(0, new SubtractOperation(), 2);
            expr.AddAtomicExpression(step);
            result = step.Calculate(_num3_6);
            Assert.AreEqual(1, result.StartValue);
            Assert.AreEqual(-2, result.EndValue);

            step = new AtomicExpression( 0, new MultiplyOperation(), 2);
            expr.AddAtomicExpression(step);
            result = step.Calculate(_num3_6);
            Assert.AreEqual(36, result.StartValue);
            Assert.AreEqual(42, result.EndValue);

            step = new AtomicExpression(0, new DivideOperation(), 2);
            expr.AddAtomicExpression(step);
            result = step.Calculate(_num3_6);
            Assert.AreEqual(0.17, result.StartValue, _delta);
            Assert.AreEqual(0.79, result.EndValue, _delta);

        }
    }
}