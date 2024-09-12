using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Expressions;

namespace TestNumerics.Operations;

[TestClass]
public class EquationTests
{
    private Trait _trait = null!;
    private Focal _basisFocal = null!;
    private Focal _limits = null!;
    private Domain _domain = null!;
    private Number _num2_8 = null!;
    private Number _num3_6 = null!;
    private Number _num1_m7 = null!;
    private Number _num_m3_m6 = null!;
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
        _num1_m7 = new Number(_domain, new(-100, -700)); // (i -7)
        _num_m3_m6 = new Number(_domain, new(300, -600));// (-3i -6)
        _numList = new Number[] { _num2_8, _num3_6, _num1_m7, _num_m3_m6 };
    }

    [TestMethod]
    public void EquationTest1()
    {
        var equation = new Expression(_numList);
        var step1 = new AtomicExpression(1, new AddOperation(), 1);
        var step2 = new AtomicExpression(2, new MultiplyOperation(), 1);
        equation.AddAtomicExpression(step1, step2);

        var result = equation.Calculate(_num2_8);
        Assert.AreEqual(-21, result.StartValue);
        Assert.AreEqual(-103, result.EndValue);
    }
    [TestMethod]
    public void InvertTest()
    {
        var offset = new Number(_domain, new(0, 0));
        var path = new Number(_domain, new(0, 300));
        var lm1 = new Landmark(path, 0.2);
        var lm2 = new Landmark(path, 0.8);
        var lmn = new Number(_domain, lm1, lm2);
        var seedList = new List<Number> { offset};

        var equation = new Expression(seedList, 10, NumericsCore.Interfaces.TileMode.Ignore, true);
        var step1 = new AtomicExpression(0, new AddOperation(), 1);
        var step2 = new AtomicExpression(new InvertOperation(), 1);
        equation.AddAtomicExpression(step1, step2);

    }

    [TestMethod]
    public void PowTest()
    {
        var seedList = new List<Number> { new Number(_domain, new(0, 300)) };
        var equation = new Expression(seedList, 10, NumericsCore.Interfaces.TileMode.Ignore, true);
        var step1 = new AtomicExpression(0, new MultiplyOperation(), 1);
        equation.AddAtomicExpression(step1);
        equation.SetInput(seedList[0]);

        var results = new List<int> { 9, 27, 81, 243, 729, 2187 };
        for (int i = 0; i < 6; i++)
        {
            var result = equation.Next();
            Assert.IsNotNull(result);
            Assert.AreEqual(results[i], (int)result.EndValue);
        }
    }
    [TestMethod]
    public void FibbonaciTest()
    {
        var seedList = new List<Number> { new Number(_domain, new(0, 100)), new Number(_domain, new Focal(0, 100)) };
        var equation = new Expression(seedList, 10, NumericsCore.Interfaces.TileMode.Ignore, true);
        var step1 = new AtomicExpression(-2, new AddOperation(), 1);
        equation.AddAtomicExpression(step1);
        equation.SetInput(seedList[1]);
        var results = new List<int> { 2, 3, 5, 8, 13, 21 };
        for (int i = 0; i < 6; i++)
        {
            var result = equation.Next();
            Assert.IsNotNull(result);
            Assert.AreEqual(results[i], (int)result.EndValue);
        }
    }
    [TestMethod]
    public void FibbonaciTest2()
    {
        // using the aligned and inverted sides of a number (total length is new endpoint, alternate polarity)
        var seedList = new List<Number> { new Number(_domain, new(-100, 100)) };
        var equation = new Expression(seedList, 10, NumericsCore.Interfaces.TileMode.Ignore, true);
        var step1 = new AtomicExpression(-1, new AddOperation(), 1);
        var step2 = new AtomicExpression(new InvertNegateOperation(), 1); // use stack?
        //equation.AddAtomicExpression(step1);
        //equation.SetInput(seedList[1]);
        //var results = new List<int> { 2, 3, 5, 8, 13, 21 };
        //for (int i = 0; i < 6; i++)
        //{
        //    var result = equation.Next();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(results[i], (int)result.EndValue);
        //}
    }
    [TestMethod]
    public void SineGravityTest()
    {
        // attractor is encoded as center point with amount to change passing object velocities (always towards its center)
        // orbiter is encoded as an object moving a length velocity each step, which is variable.
        var seedList = new List<Number> { new Number(_domain, new(50, 50)), new Number(_domain, new Focal(1000, -1000)) };
        // curpoint is endpoint, previous point is startpoint.
        // Add aligned or inverted to length depending on side the endpoint is on.
        // append this to endpoint, and put startpoint at previous endpoint.
        // will need to split the speed when passing through zero, and when turning around.
        // probably it should be positive in the direction it is going (inverted if going left, aligned if going right)
    }
}