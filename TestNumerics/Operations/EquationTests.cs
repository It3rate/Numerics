using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numerics.Primitives;
using NumericsCore.Structures;

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
    private List<Number> _numList = null!;
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
        _numList = new List<Number> { _num2_8, _num3_6, _num1_m7, _num_m3_m6 };
    }

    [TestMethod]
    public void EquationTest1()
    {
        var equation = new Equation(_numList);
        var step1 = new EquationStep(_numList, 1, new AddOperation(), 1);
        var step2 = new EquationStep(_numList, 2, new MultiplyOperation(), 1);
        equation.AddEquationSteps(step1, step2);

        var result = equation.Calculate(_num2_8);
        Assert.AreEqual(-21, result.StartValue);
        Assert.AreEqual(-103, result.EndValue);
    }

    [TestMethod]
    public void UnaryStepTests()
    {
        var step = new EquationStep(new ResetOperation(), 2);
        var result = step.Calculate(_num2_8);
        Assert.AreEqual(0, result.StartValue);
        Assert.AreEqual(0, result.EndValue);


        step = new EquationStep(new FlipPolarityOperation(), 2);
        result = step.Calculate(_num2_8);
        Assert.AreEqual(-2, result.StartValue);
        Assert.AreEqual(-8, result.EndValue);

        step = new EquationStep(new IncrementOperation(), 2);
        result = step.Calculate(_num2_8);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(9, result.EndValue);

        step = new EquationStep(new DecrementOperation(), 2);
        result = step.Calculate(_num2_8);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(7, result.EndValue);

        step = new EquationStep(new IncrementEndTickOperation(), 2);
        result = step.Calculate(_num2_8);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(8.01, result.EndValue);

        step = new EquationStep(new DecrementEndTickOperation(), 2);
        result = step.Calculate(_num2_8);
        Assert.AreEqual(2, result.StartValue);
        Assert.AreEqual(7.99, result.EndValue);
    }

    [TestMethod]
    public void BinaryStepTests()
    {
        var step = new EquationStep(_numList, 0, new AddOperation(), 2);
        var result = step.Calculate(_num3_6);
        Assert.AreEqual(5, result.StartValue);
        Assert.AreEqual(14, result.EndValue);

        step = new EquationStep(_numList, 0, new SubtractOperation(), 2);
        result = step.Calculate(_num3_6);
        Assert.AreEqual(1, result.StartValue);
        Assert.AreEqual(-2, result.EndValue);

        step = new EquationStep(_numList, 0, new MultiplyOperation(), 2);
        result = step.Calculate(_num3_6);
        Assert.AreEqual(36, result.StartValue);
        Assert.AreEqual(42, result.EndValue);

        step = new EquationStep(_numList, 0, new DivideOperation(), 2);
        result = step.Calculate(_num3_6);
        Assert.AreEqual(0.17, result.StartValue, _delta);
        Assert.AreEqual(0.79, result.EndValue, _delta);

    }
}